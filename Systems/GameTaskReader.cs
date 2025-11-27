using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace cvbhhnClassLibrary1.Systems
{
    public class GameTaskInfo
    {
        public int id;
        public string name;
        public string status;
        public string progress;
        public string objectives;
        public string rewards;
        public bool canComplete;
        public object originalTask;
    }

    public class GameTaskReader
    {
        private List<GameTaskInfo> cachedTasks = new List<GameTaskInfo>();
        private bool isLoading = false;
        private bool needsReload = false;
        private List<(object quest, string status)> pendingQuests = new List<(object, string)>();
        private const int RETRY_FRAME_INTERVAL = 60;
        private const int QUESTS_PER_FRAME = 20;
        private const int AUTO_REFRESH_INTERVAL = 300;
        private int autoRefreshCounter = 0;
        private int retryFrameCounter = 0;
        private int currentQuestIndex = 0;

        public void Initialize()
        {
            Debug.Log("[GameTaskReader] Initialized");
        }

        public List<GameTaskInfo> GetCachedTasks()
        {
            return new List<GameTaskInfo>(cachedTasks);
        }

        public bool IsLoading => isLoading;

        public void RequestReload()
        {
            needsReload = true;
            retryFrameCounter = RETRY_FRAME_INTERVAL;
        }

        public void UpdateOnMainThread()
        {
            if (!isLoading)
            {
                autoRefreshCounter++;
                if (autoRefreshCounter >= AUTO_REFRESH_INTERVAL)
                {
                    autoRefreshCounter = 0;
                    needsReload = true;
                }
            }

            if (needsReload && !isLoading)
            {
                retryFrameCounter++;
                if (retryFrameCounter >= RETRY_FRAME_INTERVAL)
                {
                    retryFrameCounter = 0;
                    needsReload = false;
                    StartLoadingTasks();
                }
            }

            if (isLoading)
            {
                if (pendingQuests.Count > 0)
                {
                    ProcessBatchQuests();
                }
                else
                {
                    isLoading = false;
                }
            }
        }

        private void StartLoadingTasks()
        {
            if (!SceneDetector.IsInGame())
            {
                Debug.Log("[GameTaskReader] Not in game scene, skipping task read");
                needsReload = true;
                return;
            }

            isLoading = true;
            cachedTasks.Clear();
            pendingQuests.Clear();
            collectedQuestIds.Clear();
            currentQuestIndex = 0;

            try
            {
                Type questManagerType = FindType("Duckov.Quests.QuestManager");
                if (questManagerType == null)
                {
                    Debug.LogWarning("[GameTaskReader] QuestManager type not found");
                    isLoading = false;
                    return;
                }

                PropertyInfo instanceProp = questManagerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                if (instanceProp == null)
                {
                    Debug.LogWarning("[GameTaskReader] QuestManager.Instance property not found");
                    isLoading = false;
                    return;
                }

                object questManagerInstance = instanceProp.GetValue(null);
                if (questManagerInstance == null)
                {
                    Debug.Log("[GameTaskReader] QuestManager.Instance is null, will retry later");
                    isLoading = false;
                    needsReload = true;
                    return;
                }
                
                Debug.Log("[GameTaskReader] QuestManager.Instance found, loading quests...");

                CollectQuestsFromProperty(questManagerType, questManagerInstance, "ActiveQuests", "进行中");
                CollectQuestsFromProperty(questManagerType, questManagerInstance, "HistoryQuests", "已完成");
                
                CollectAllQuestsFromCollection();

                Debug.Log($"[GameTaskReader] Collected {pendingQuests.Count} quests for processing");
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameTaskReader] Error starting task load: {e.Message}");
                isLoading = false;
            }
        }

        private HashSet<int> collectedQuestIds = new HashSet<int>();

        private void CollectAllQuestsFromCollection()
        {
            try
            {
                Type questCollectionType = FindType("Duckov.Quests.QuestCollection");
                if (questCollectionType == null)
                {
                    Debug.Log("[GameTaskReader] QuestCollection type not found");
                    return;
                }

                PropertyInfo instanceProp = questCollectionType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                if (instanceProp == null)
                {
                    Debug.Log("[GameTaskReader] QuestCollection.Instance not found");
                    return;
                }

                object collectionInstance = instanceProp.GetValue(null);
                if (collectionInstance == null)
                {
                    Debug.Log("[GameTaskReader] QuestCollection instance is null");
                    return;
                }

                System.Collections.IEnumerable quests = collectionInstance as System.Collections.IEnumerable;
                if (quests == null)
                {
                    Debug.Log("[GameTaskReader] QuestCollection is not enumerable");
                    return;
                }

                Type questManagerType = FindType("Duckov.Quests.QuestManager");
                MethodInfo isAvailableMethod = questManagerType?.GetMethod("IsQuestAvaliable", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                Debug.Log($"[GameTaskReader] IsQuestAvaliable method found: {isAvailableMethod != null}");
                
                int addedCount = 0;
                int availableCount = 0;
                foreach (var quest in quests)
                {
                    if (quest == null) continue;
                    
                    Type questType = quest.GetType();
                    PropertyInfo idProp = questType.GetProperty("ID", BindingFlags.Public | BindingFlags.Instance);
                    if (idProp == null) continue;
                    
                    int questId = (int)idProp.GetValue(quest);
                    
                    if (!collectedQuestIds.Contains(questId))
                    {
                        string status = "未激活";
                        if (isAvailableMethod != null)
                        {
                            try
                            {
                                bool isAvailable = (bool)isAvailableMethod.Invoke(null, new object[] { questId });
                                if (isAvailable)
                                {
                                    status = "可接取";
                                    availableCount++;
                                }
                            }
                            catch { }
                        }
                        pendingQuests.Add((quest, status));
                        collectedQuestIds.Add(questId);
                        addedCount++;
                    }
                }
                Debug.Log($"[GameTaskReader] Available quests: {availableCount}");
                
                Debug.Log($"[GameTaskReader] Added {addedCount} quests from QuestCollection");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[GameTaskReader] Error collecting from QuestCollection: {e.Message}");
            }
        }

        private void CollectQuestsFromProperty(Type questManagerType, object instance, string propertyName, string statusLabel)
        {
            try
            {
                PropertyInfo questsProp = questManagerType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (questsProp == null)
                {
                    Debug.Log($"[GameTaskReader] {propertyName} property not found");
                    return;
                }

                object questsObj = questsProp.GetValue(instance);
                if (questsObj == null)
                {
                    Debug.Log($"[GameTaskReader] {propertyName} is null");
                    return;
                }

                System.Collections.IList quests = questsObj as System.Collections.IList;
                if (quests == null || quests.Count == 0)
                {
                    Debug.Log($"[GameTaskReader] {propertyName}: 0 quests");
                    return;
                }

                int addedCount = 0;
                foreach (var quest in quests)
                {
                    if (quest != null)
                    {
                        Type questType = quest.GetType();
                        PropertyInfo idProp = questType.GetProperty("ID", BindingFlags.Public | BindingFlags.Instance);
                        int questId = idProp != null ? (int)idProp.GetValue(quest) : -1;
                        
                        if (!collectedQuestIds.Contains(questId))
                        {
                            pendingQuests.Add((quest, statusLabel));
                            collectedQuestIds.Add(questId);
                            addedCount++;
                        }
                    }
                }
                Debug.Log($"[GameTaskReader] {propertyName}: {addedCount} quests added");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[GameTaskReader] Error collecting {propertyName}: {e.Message}");
            }
        }

        private void ProcessBatchQuests()
        {
            int processed = 0;
            while (currentQuestIndex < pendingQuests.Count && processed < QUESTS_PER_FRAME)
            {
                try
                {
                    var (quest, status) = pendingQuests[currentQuestIndex];
                    ExtractQuestInfo(quest, status);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[GameTaskReader] Error extracting quest: {e.Message}");
                }
                currentQuestIndex++;
                processed++;
            }

            if (currentQuestIndex >= pendingQuests.Count)
            {
                isLoading = false;
                pendingQuests.Clear();
                Debug.Log($"[GameTaskReader] Completed loading {cachedTasks.Count} quests");
            }
        }

        public List<GameTaskInfo> ReadAllGameTasks()
        {
            RequestReload();
            return new List<GameTaskInfo>(cachedTasks);
        }

        private Type FindType(string fullName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var type = assembly.GetType(fullName);
                    if (type != null)
                    {
                        return type;
                    }
                }
                catch
                {
                }
            }
            return null;
        }

        private void ListQuestTypes()
        {
            int count = 0;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (type.FullName != null && 
                            (type.FullName.Contains("Quest") || type.FullName.Contains("Mission")))
                        {
                            if (count < 20)
                            {
                                Debug.Log($"[GameTaskReader] Found type: {type.FullName} in {assembly.GetName().Name}");
                            }
                            count++;
                        }
                    }
                }
                catch
                {
                }
            }
            Debug.Log($"[GameTaskReader] Total Quest/Mission types found: {count}");
        }

        private void ExtractQuestInfo(object quest, string statusOverride = null)
        {
            try
            {
                Type questType = quest.GetType();
                
                PropertyInfo displayNameProp = questType.GetProperty("DisplayName", BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo descriptionProp = questType.GetProperty("Description", BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo completeProp = questType.GetProperty("Complete", BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo tasksProp = questType.GetProperty("Tasks", BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo idProp = questType.GetProperty("ID", BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo rewardsProp = questType.GetProperty("Rewards", BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo questGiverProp = questType.GetProperty("QuestGiverID", BindingFlags.Public | BindingFlags.Instance);
                
                string displayName = displayNameProp?.GetValue(quest) as string ?? "Unknown Quest";
                string description = descriptionProp?.GetValue(quest) as string ?? "";
                bool isComplete = completeProp != null && (bool)completeProp.GetValue(quest);
                int questId = idProp != null ? (int)idProp.GetValue(quest) : 0;
                
                string questGiver = "";
                if (questGiverProp != null)
                {
                    var giverValue = questGiverProp.GetValue(quest);
                    if (giverValue != null) questGiver = giverValue.ToString();
                }
                
                object tasksObj = tasksProp?.GetValue(quest);
                string progress = "0/0";
                string objectives = "";
                
                if (tasksObj is System.Collections.IList tasks)
                {
                    int finishedCount = 0;
                    int totalCount = tasks.Count;
                    System.Text.StringBuilder objBuilder = new System.Text.StringBuilder();
                    
                    foreach (var task in tasks)
                    {
                        if (task != null)
                        {
                            Type taskType = task.GetType();
                            MethodInfo isFinishedMethod = taskType.GetMethod("IsFinished", BindingFlags.Public | BindingFlags.Instance);
                            bool taskFinished = isFinishedMethod != null && (bool)isFinishedMethod.Invoke(task, null);
                            
                            if (taskFinished) finishedCount++;
                            
                            string taskDetail = ExtractTaskDetail(task, taskType, taskFinished);
                            if (!string.IsNullOrEmpty(taskDetail))
                            {
                                if (objBuilder.Length > 0) objBuilder.Append("\n");
                                objBuilder.Append(taskDetail);
                            }
                        }
                    }
                    
                    progress = $"{finishedCount}/{totalCount}";
                    objectives = objBuilder.Length > 0 ? objBuilder.ToString() : description;
                    if (!string.IsNullOrEmpty(questGiver)) objectives = $"[{questGiver}]\n{objectives}";
                }
                
                string rewardsText = "";
                object rewardsObj = rewardsProp?.GetValue(quest);
                if (rewardsObj is System.Collections.IList rewards && rewards.Count > 0)
                {
                    System.Text.StringBuilder rewBuilder = new System.Text.StringBuilder();
                    foreach (var reward in rewards)
                    {
                        if (reward != null && rewBuilder.Length < 150)
                        {
                            Type rewardType = reward.GetType();
                            PropertyInfo descProp = rewardType.GetProperty("Description", BindingFlags.Public | BindingFlags.Instance);
                            if (descProp != null)
                            {
                                string rewardDesc = descProp.GetValue(reward) as string;
                                if (!string.IsNullOrEmpty(rewardDesc) && rewardDesc != "未定义奖励描述")
                                {
                                    if (rewBuilder.Length > 0) rewBuilder.Append("; ");
                                    rewBuilder.Append(rewardDesc);
                                }
                            }
                        }
                    }
                    rewardsText = rewBuilder.Length > 0 ? rewBuilder.ToString() : "未知奖励";
                }
                else
                {
                    rewardsText = "无奖励";
                }
                
                MethodInfo areTasksFinishedMethod = questType.GetMethod("AreTasksFinished", BindingFlags.Public | BindingFlags.Instance);
                bool canComplete = !isComplete && areTasksFinishedMethod != null && (bool)areTasksFinishedMethod.Invoke(quest, null);
                
                string status = statusOverride ?? (isComplete ? "已完成" : "进行中");
                
                var taskInfo = new GameTaskInfo
                {
                    id = questId,
                    name = displayName,
                    status = status,
                    progress = progress,
                    objectives = objectives,
                    rewards = rewardsText,
                    canComplete = canComplete && status == "进行中",
                    originalTask = quest
                };
                
                cachedTasks.Add(taskInfo);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[GameTaskReader] Error extracting quest info: {e.Message}");
            }
        }

        private string ExtractTaskDetail(object task, Type taskType, bool isFinished)
        {
            try
            {
                string checkMark = isFinished ? "[✓]" : "[ ]";
                string typeName = taskType.Name;
                
                PropertyInfo descProp = taskType.GetProperty("Description", BindingFlags.Public | BindingFlags.Instance);
                string desc = descProp?.GetValue(task) as string ?? "";
                
                if (typeName.Contains("Submit") || typeName.Contains("Deliver"))
                {
                    PropertyInfo itemIdProp = taskType.GetProperty("ItemID", BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo quantityProp = taskType.GetProperty("Quantity", BindingFlags.Public | BindingFlags.Instance);
                    MethodInfo getOwnedMethod = taskType.GetMethod("GetOwnedQuantity", BindingFlags.Public | BindingFlags.Instance);
                    
                    if (itemIdProp != null && quantityProp != null)
                    {
                        int itemId = (int)itemIdProp.GetValue(task);
                        int quantity = (int)quantityProp.GetValue(task);
                        int owned = 0;
                        if (getOwnedMethod != null)
                        {
                            owned = (int)getOwnedMethod.Invoke(task, null);
                        }
                        string itemName = GetItemName(itemId);
                        return $"{checkMark} 提交物品: {itemName} x{quantity} (持有: {owned})";
                    }
                }
                else if (typeName.Contains("Kill") || typeName.Contains("Hunt"))
                {
                    PropertyInfo targetProp = taskType.GetProperty("TargetID", BindingFlags.Public | BindingFlags.Instance) 
                                           ?? taskType.GetProperty("Target", BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo countProp = taskType.GetProperty("RequiredCount", BindingFlags.Public | BindingFlags.Instance)
                                          ?? taskType.GetProperty("Count", BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo currentProp = taskType.GetProperty("CurrentCount", BindingFlags.Public | BindingFlags.Instance)
                                            ?? taskType.GetProperty("KillCount", BindingFlags.Public | BindingFlags.Instance);
                    
                    string target = targetProp?.GetValue(task)?.ToString() ?? "目标";
                    int required = countProp != null ? (int)countProp.GetValue(task) : 1;
                    int current = currentProp != null ? (int)currentProp.GetValue(task) : 0;
                    if (isFinished) current = required;
                    return $"{checkMark} 击杀: {target} ({current}/{required})";
                }
                else if (typeName.Contains("Collect") || typeName.Contains("Gather"))
                {
                    PropertyInfo itemIdProp = taskType.GetProperty("ItemID", BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo quantityProp = taskType.GetProperty("Quantity", BindingFlags.Public | BindingFlags.Instance)
                                             ?? taskType.GetProperty("RequiredAmount", BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo currentProp = taskType.GetProperty("CurrentAmount", BindingFlags.Public | BindingFlags.Instance);
                    
                    if (itemIdProp != null)
                    {
                        int itemId = (int)itemIdProp.GetValue(task);
                        int quantity = quantityProp != null ? (int)quantityProp.GetValue(task) : 1;
                        int current = currentProp != null ? (int)currentProp.GetValue(task) : 0;
                        if (isFinished) current = quantity;
                        string itemName = GetItemName(itemId);
                        return $"{checkMark} 收集: {itemName} ({current}/{quantity})";
                    }
                }
                else if (typeName.Contains("Talk") || typeName.Contains("Speak"))
                {
                    PropertyInfo npcProp = taskType.GetProperty("NPCID", BindingFlags.Public | BindingFlags.Instance)
                                        ?? taskType.GetProperty("TargetNPC", BindingFlags.Public | BindingFlags.Instance);
                    string npc = npcProp?.GetValue(task)?.ToString() ?? "NPC";
                    return $"{checkMark} 对话: {npc}";
                }
                else if (typeName.Contains("Go") || typeName.Contains("Travel") || typeName.Contains("Visit"))
                {
                    PropertyInfo locProp = taskType.GetProperty("Location", BindingFlags.Public | BindingFlags.Instance)
                                        ?? taskType.GetProperty("Destination", BindingFlags.Public | BindingFlags.Instance);
                    string location = locProp?.GetValue(task)?.ToString() ?? "目的地";
                    return $"{checkMark} 前往: {location}";
                }
                
                if (!string.IsNullOrEmpty(desc))
                {
                    return $"{checkMark} {desc}";
                }
                
                return $"{checkMark} {typeName}";
            }
            catch
            {
                return "";
            }
        }

        private Type cachedItemAssetsCollectionType;
        private MethodInfo cachedGetMetaDataMethod;
        
        private string GetItemName(int itemId)
        {
            try
            {
                if (cachedItemAssetsCollectionType == null)
                {
                    cachedItemAssetsCollectionType = FindType("ItemStatsSystem.ItemAssetsCollection");
                }
                if (cachedItemAssetsCollectionType != null)
                {
                    if (cachedGetMetaDataMethod == null)
                    {
                        cachedGetMetaDataMethod = cachedItemAssetsCollectionType.GetMethod("GetMetaData", BindingFlags.Public | BindingFlags.Static);
                    }
                    if (cachedGetMetaDataMethod != null)
                    {
                        object metaData = cachedGetMetaDataMethod.Invoke(null, new object[] { itemId });
                        if (metaData != null)
                        {
                            PropertyInfo displayNameProp = metaData.GetType().GetProperty("DisplayName", BindingFlags.Public | BindingFlags.Instance);
                            if (displayNameProp != null)
                            {
                                string name = displayNameProp.GetValue(metaData) as string;
                                if (!string.IsNullOrEmpty(name)) return name;
                            }
                        }
                    }
                }
            }
            catch { }
            return $"Item#{itemId}";
        }
        
        public object GetItemInstance(int itemId)
        {
            try
            {
                if (cachedItemAssetsCollectionType == null)
                {
                    cachedItemAssetsCollectionType = FindType("ItemStatsSystem.ItemAssetsCollection");
                }
                if (cachedItemAssetsCollectionType != null)
                {
                    MethodInfo instantiateSync = cachedItemAssetsCollectionType.GetMethod("InstantiateSync", BindingFlags.Public | BindingFlags.Static);
                    if (instantiateSync != null)
                    {
                        return instantiateSync.Invoke(null, new object[] { itemId });
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[GameTaskReader] GetItemInstance error: {e.Message}");
            }
            return null;
        }
        
        public bool SendItemToPlayer(int itemId, int count = 1)
        {
            try
            {
                Type itemUtilitiesType = FindType("ItemStatsSystem.ItemUtilities");
                if (itemUtilitiesType == null) return false;
                
                MethodInfo sendToPlayer = itemUtilitiesType.GetMethod("SendToPlayer", BindingFlags.Public | BindingFlags.Static);
                if (sendToPlayer == null) return false;
                
                for (int i = 0; i < count; i++)
                {
                    var item = GetItemInstance(itemId);
                    if (item != null)
                    {
                        sendToPlayer.Invoke(null, new object[] { item, false, true });
                    }
                }
                Debug.Log($"[GameTaskReader] Sent {count}x Item#{itemId} to player");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[GameTaskReader] SendItemToPlayer error: {e.Message}");
                return false;
            }
        }
        
        public bool TryCompleteGameQuest(int questId)
        {
            try
            {
                foreach (var taskInfo in cachedTasks)
                {
                    if (taskInfo.id == questId && taskInfo.originalTask != null)
                    {
                        Type questType = taskInfo.originalTask.GetType();
                        MethodInfo tryCompleteMethod = questType.GetMethod("TryComplete", BindingFlags.Public | BindingFlags.Instance);
                        
                        if (tryCompleteMethod != null)
                        {
                            bool result = (bool)tryCompleteMethod.Invoke(taskInfo.originalTask, null);
                            Debug.Log($"[GameTaskReader] TryComplete quest {questId}: {result}");
                            return result;
                        }
                    }
                }
                Debug.LogWarning($"[GameTaskReader] Quest {questId} not found in cached tasks");
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameTaskReader] Error completing quest: {e.Message}");
            }
            return false;
        }

        public bool TryActivateQuest(int questId)
        {
            try
            {
                Type questManagerType = FindType("Duckov.Quests.QuestManager");
                if (questManagerType == null) return false;

                var instanceProp = questManagerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                if (instanceProp == null) return false;

                object questManager = instanceProp.GetValue(null);
                if (questManager == null) return false;

                var isAvailableMethod = questManagerType.GetMethod("IsQuestAvaliable", BindingFlags.NonPublic | BindingFlags.Static);
                if (isAvailableMethod != null)
                {
                    bool isAvailable = (bool)isAvailableMethod.Invoke(null, new object[] { questId });
                    if (!isAvailable)
                    {
                        Debug.LogWarning($"[GameTaskReader] Quest {questId} is not available (prerequisites not met or already active/finished)");
                    }
                }

                var activateMethod = questManagerType.GetMethod("ActivateQuest", BindingFlags.Public | BindingFlags.Instance);
                if (activateMethod != null)
                {
                    activateMethod.Invoke(questManager, new object[] { questId, null });
                    Debug.Log($"[GameTaskReader] Activated quest {questId}");
                    RequestReload();
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameTaskReader] Error activating quest: {e.Message}");
            }
            return false;
        }

        public bool TryForceCompleteQuest(int questId)
        {
            try
            {
                foreach (var taskInfo in cachedTasks)
                {
                    if (taskInfo.id == questId && taskInfo.originalTask != null)
                    {
                        Type questType = taskInfo.originalTask.GetType();
                        
                        var completeProp = questType.GetProperty("Complete", BindingFlags.Public | BindingFlags.Instance);
                        if (completeProp != null && completeProp.CanWrite)
                        {
                            completeProp.SetValue(taskInfo.originalTask, true);
                            Debug.Log($"[GameTaskReader] Force completed quest {questId}");
                            RequestReload();
                            return true;
                        }
                    }
                }
                Debug.LogWarning($"[GameTaskReader] Quest {questId} not found in active quests");
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameTaskReader] Error force completing quest: {e.Message}");
            }
            return false;
        }

        public bool TryResetGameQuest(int questId)
        {
            try
            {
                Type questManagerType = FindType("Duckov.Quests.QuestManager");
                if (questManagerType == null) return false;

                var instanceProp = questManagerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                if (instanceProp == null) return false;

                object questManager = instanceProp.GetValue(null);
                if (questManager == null) return false;

                var historyQuestsField = questManagerType.GetField("historyQuests", BindingFlags.NonPublic | BindingFlags.Instance);
                if (historyQuestsField == null)
                {
                    Debug.LogWarning("[GameTaskReader] historyQuests field not found");
                    return false;
                }

                var historyQuests = historyQuestsField.GetValue(questManager) as System.Collections.IList;
                if (historyQuests == null) return false;

                object questToRemove = null;
                foreach (var quest in historyQuests)
                {
                    var idProp = quest.GetType().GetProperty("ID", BindingFlags.Public | BindingFlags.Instance);
                    if (idProp != null)
                    {
                        int id = (int)idProp.GetValue(quest);
                        if (id == questId)
                        {
                            questToRemove = quest;
                            break;
                        }
                    }
                }

                if (questToRemove != null)
                {
                    historyQuests.Remove(questToRemove);
                    Debug.Log($"[GameTaskReader] Removed quest {questId} from history");

                    var activateMethod = questManagerType.GetMethod("ActivateQuest", BindingFlags.Public | BindingFlags.Instance);
                    if (activateMethod != null)
                    {
                        activateMethod.Invoke(questManager, new object[] { questId, null });
                        Debug.Log($"[GameTaskReader] Reactivated quest {questId}");
                        RequestReload();
                        return true;
                    }
                }
                else
                {
                    Debug.LogWarning($"[GameTaskReader] Quest {questId} not found in history");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameTaskReader] Error resetting quest: {e.Message}");
            }
            return false;
        }
    }
}

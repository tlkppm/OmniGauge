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

        public void Initialize()
        {
            Debug.Log("[GameTaskReader] Initialized");
        }

        public List<GameTaskInfo> GetCachedTasks()
        {
            return new List<GameTaskInfo>(cachedTasks);
        }

        public List<GameTaskInfo> ReadAllGameTasks()
        {
            cachedTasks.Clear();
            
            try
            {
                Type questManagerType = FindType("Duckov.Quests.QuestManager");
                if (questManagerType == null)
                {
                    Debug.LogWarning("[GameTaskReader] QuestManager type not found in any assembly");
                    Debug.Log("[GameTaskReader] Searching for Quest-related types...");
                    ListQuestTypes();
                    return new List<GameTaskInfo>(cachedTasks);
                }
                
                Debug.Log($"[GameTaskReader] Found QuestManager in assembly: {questManagerType.Assembly.GetName().Name}");

                PropertyInfo instanceProp = questManagerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                if (instanceProp == null)
                {
                    Debug.LogWarning("[GameTaskReader] QuestManager.Instance property not found");
                    return new List<GameTaskInfo>(cachedTasks);
                }

                object questManagerInstance = instanceProp.GetValue(null);
                if (questManagerInstance == null)
                {
                    Debug.LogWarning("[GameTaskReader] QuestManager instance is null");
                    return new List<GameTaskInfo>(cachedTasks);
                }

                PropertyInfo activeQuestsProp = questManagerType.GetProperty("ActiveQuests", BindingFlags.Public | BindingFlags.Instance);
                if (activeQuestsProp == null)
                {
                    Debug.LogWarning("[GameTaskReader] ActiveQuests property not found");
                    return new List<GameTaskInfo>(cachedTasks);
                }

                object activeQuestsObj = activeQuestsProp.GetValue(questManagerInstance);
                if (activeQuestsObj == null)
                {
                    Debug.Log("[GameTaskReader] No active quests");
                    return new List<GameTaskInfo>(cachedTasks);
                }

                System.Collections.IList activeQuests = activeQuestsObj as System.Collections.IList;
                if (activeQuests != null)
                {
                    foreach (var quest in activeQuests)
                    {
                        if (quest != null)
                        {
                            ExtractQuestInfo(quest);
                        }
                    }
                }
                
                Debug.Log($"[GameTaskReader] Found {cachedTasks.Count} real game quests");
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameTaskReader] Error reading tasks: {e.Message}\n{e.StackTrace}");
            }
            
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

        private void ExtractQuestInfo(object quest)
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
                
                string displayName = displayNameProp?.GetValue(quest) as string ?? "Unknown Quest";
                string description = descriptionProp?.GetValue(quest) as string ?? "";
                bool isComplete = completeProp != null && (bool)completeProp.GetValue(quest);
                int questId = idProp != null ? (int)idProp.GetValue(quest) : 0;
                
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
                            PropertyInfo descProp = taskType.GetProperty("Description", BindingFlags.Public | BindingFlags.Instance);
                            
                            if (isFinishedMethod != null && (bool)isFinishedMethod.Invoke(task, null))
                            {
                                finishedCount++;
                            }
                            
                            if (descProp != null && objBuilder.Length < 200)
                            {
                                string taskDesc = descProp.GetValue(task) as string;
                                if (!string.IsNullOrEmpty(taskDesc))
                                {
                                    if (objBuilder.Length > 0) objBuilder.Append("; ");
                                    objBuilder.Append(taskDesc);
                                }
                            }
                        }
                    }
                    
                    progress = $"{finishedCount}/{totalCount}";
                    objectives = objBuilder.Length > 0 ? objBuilder.ToString() : description;
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
                
                var taskInfo = new GameTaskInfo
                {
                    id = questId,
                    name = displayName,
                    status = isComplete ? "已完成" : "进行中",
                    progress = progress,
                    objectives = objectives,
                    rewards = rewardsText,
                    canComplete = canComplete,
                    originalTask = quest
                };
                
                cachedTasks.Add(taskInfo);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[GameTaskReader] Error extracting quest info: {e.Message}");
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
    }
}

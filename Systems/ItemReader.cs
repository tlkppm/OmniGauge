using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Threading;
using ItemStatsSystem;

namespace cvbhhnClassLibrary1.Systems
{
    public class ItemInfo
    {
        public int id;
        public string name;
        public string displayName;
        public string description;
        public string category;
        public int quality;
        public int price;
        public int maxStack;
        public string iconBase64;
        [System.NonSerialized]
        public Sprite iconSprite;
    }

    public class ItemReader
    {
        private List<ItemInfo> cachedItems = new List<ItemInfo>();
        private bool isLoaded = false;
        private bool isLoading = false;
        private bool needsReload = false;
        
        private Queue<(int itemId, int quantity)> giveItemQueue = new Queue<(int, int)>();
        
        private List<object> pendingEntries = new List<object>();
        private int currentEntryIndex = 0;
        private const int ITEMS_PER_FRAME = 50;
        private int totalEntries = 0;
        private int retryFrameCounter = 0;
        private const int RETRY_FRAME_INTERVAL = 60;

        public void Initialize()
        {
            Debug.Log("[ItemReader] Initialized");
            StartIconConvertThread();
        }
        
        private void StartIconConvertThread()
        {
            if (iconConvertThread != null && iconConvertThread.IsAlive) return;
            
            threadRunning = true;
            iconConvertThread = new Thread(IconConvertThreadLoop);
            iconConvertThread.IsBackground = true;
            iconConvertThread.Start();
            Debug.Log("[ItemReader] Icon convert thread started");
        }
        
        private void IconConvertThreadLoop()
        {
            while (threadRunning)
            {
                (int itemId, byte[] pngData) work = default;
                bool hasWork = false;
                
                lock (queueLock)
                {
                    if (pngConvertQueue.Count > 0)
                    {
                        work = pngConvertQueue.Dequeue();
                        hasWork = true;
                    }
                }
                
                if (hasWork)
                {
                    try
                    {
                        string base64 = Convert.ToBase64String(work.pngData);
                        lock (queueLock)
                        {
                            iconCache[work.itemId] = base64;
                            foreach (var item in cachedItems)
                            {
                                if (item.id == work.itemId)
                                {
                                    item.iconBase64 = base64;
                                    break;
                                }
                            }
                        }
                    }
                    catch { }
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
        }
        
        public void Shutdown()
        {
            threadRunning = false;
            if (iconConvertThread != null && iconConvertThread.IsAlive)
            {
                iconConvertThread.Join(1000);
            }
        }
        
        public int LoadProgress => totalEntries > 0 ? (currentEntryIndex * 100 / totalEntries) : 0;
        
        public void RequestGiveItem(int itemId, int quantity = 1)
        {
            giveItemQueue.Enqueue((itemId, quantity));
        }
        
        public void ProcessGiveItemQueue()
        {
            while (giveItemQueue.Count > 0)
            {
                var request = giveItemQueue.Dequeue();
                GiveItemOnMainThread(request.itemId, request.quantity);
            }
        }
        
        private void GiveItemOnMainThread(int itemId, int quantity)
        {
            if (!SceneDetector.IsInGame())
            {
                Debug.LogWarning("[ItemReader] Cannot give items in main menu");
                return;
            }

            try
            {
                var cheatingManagerType = Type.GetType("CheatingManager, Assembly-CSharp");
                if (cheatingManagerType == null)
                {
                    foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        cheatingManagerType = asm.GetType("CheatingManager");
                        if (cheatingManagerType != null) break;
                    }
                }
                
                if (cheatingManagerType != null)
                {
                    var instanceProp = cheatingManagerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                    if (instanceProp != null)
                    {
                        var instance = instanceProp.GetValue(null);
                        if (instance != null)
                        {
                            var createMethod = cheatingManagerType.GetMethod("CreateItem", BindingFlags.Public | BindingFlags.Instance);
                            if (createMethod != null)
                            {
                                createMethod.Invoke(instance, new object[] { itemId, quantity });
                                Debug.Log($"[ItemReader] Gave item {itemId} x{quantity}");
                                return;
                            }
                        }
                    }
                }
                Debug.LogWarning("[ItemReader] CheatingManager not available");
            }
            catch (Exception e)
            {
                Debug.LogError($"[ItemReader] Failed to give item: {e.Message}");
            }
        }

        public List<ItemInfo> GetCachedItems()
        {
            return new List<ItemInfo>(cachedItems);
        }

        public bool IsLoaded => isLoaded;
        public bool IsLoading => isLoading;

        private bool forceReloadIcons = false;
        private Dictionary<int, string> iconCache = new Dictionary<int, string>();
        private int iconConvertIndex = 0;
        private int iconConvertDelay = 0;
        private const int ICON_CONVERT_DELAY_FRAMES = 120;
        private int iconConvertFrameSkip = 0;
        private const int ICON_CONVERT_FRAME_INTERVAL = 2;
        
        private Queue<(int itemId, byte[] pngData)> pngConvertQueue = new Queue<(int, byte[])>();
        private Thread iconConvertThread;
        private bool threadRunning = false;
        private object queueLock = new object();

        public void RequestReload(bool reloadIcons = false)
        {
            needsReload = true;
            forceReloadIcons = reloadIcons;
            retryFrameCounter = RETRY_FRAME_INTERVAL;
        }

        public void UpdateOnMainThread()
        {
            ProcessGiveItemQueue();
            
            if (needsReload && !isLoading)
            {
                retryFrameCounter++;
                if (retryFrameCounter >= RETRY_FRAME_INTERVAL)
                {
                    retryFrameCounter = 0;
                    needsReload = false;
                    StartLoadingItems();
                }
            }
            
            if (isLoading && pendingEntries.Count > 0)
            {
                ProcessBatchItems();
            }
            
            if (isLoaded && !isLoading && iconConvertIndex < cachedItems.Count)
            {
                if (iconConvertDelay < ICON_CONVERT_DELAY_FRAMES)
                {
                    iconConvertDelay++;
                }
                else
                {
                    iconConvertFrameSkip++;
                    if (iconConvertFrameSkip >= ICON_CONVERT_FRAME_INTERVAL)
                    {
                        iconConvertFrameSkip = 0;
                        ConvertIconsBatch();
                    }
                }
            }
        }
        
        private void ConvertIconsBatch()
        {
            int converted = 0;
            while (iconConvertIndex < cachedItems.Count && converted < 1)
            {
                var item = cachedItems[iconConvertIndex];
                bool needsConvert = false;
                
                lock (queueLock)
                {
                    needsConvert = string.IsNullOrEmpty(item.iconBase64) && 
                                   item.iconSprite != null && 
                                   !iconCache.ContainsKey(item.id);
                }
                
                if (needsConvert)
                {
                    try
                    {
                        byte[] pngData = SpriteToPngBytes(item.iconSprite);
                        if (pngData != null && pngData.Length > 0)
                        {
                            lock (queueLock)
                            {
                                pngConvertQueue.Enqueue((item.id, pngData));
                            }
                        }
                    }
                    catch { }
                    converted++;
                }
                else if (!string.IsNullOrEmpty(item.iconBase64))
                {
                }
                else
                {
                    lock (queueLock)
                    {
                        if (iconCache.TryGetValue(item.id, out string cached))
                        {
                            item.iconBase64 = cached;
                        }
                    }
                }
                iconConvertIndex++;
            }
        }
        
        private byte[] SpriteToPngBytes(Sprite sprite)
        {
            if (sprite == null || sprite.texture == null) return null;

            Texture2D texture = sprite.texture;
            Rect rect = sprite.rect;
            int x = (int)rect.x;
            int y = (int)rect.y;
            int width = (int)rect.width;
            int height = (int)rect.height;

            Texture2D readableTexture;
            
            if (texture.isReadable)
            {
                Color[] pixels = texture.GetPixels(x, y, width, height);
                if (IsEmptyTexture(pixels)) return null;

                readableTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
                readableTexture.SetPixels(pixels);
                readableTexture.Apply();
            }
            else
            {
                RenderTexture rt = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                Graphics.Blit(texture, rt);
                RenderTexture previous = RenderTexture.active;
                RenderTexture.active = rt;
                
                Texture2D fullTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
                fullTexture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
                fullTexture.Apply();
                
                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(rt);
                
                Color[] pixels = fullTexture.GetPixels(x, y, width, height);
                UnityEngine.Object.Destroy(fullTexture);
                
                if (IsEmptyTexture(pixels)) return null;

                readableTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
                readableTexture.SetPixels(pixels);
                readableTexture.Apply();
            }

            byte[] pngData = readableTexture.EncodeToPNG();
            UnityEngine.Object.Destroy(readableTexture);

            return pngData;
        }
        
        private bool IsEmptyTexture(Color[] pixels)
        {
            for (int i = 0; i < pixels.Length; i += 10)
            {
                if (pixels[i].a > 0.05f) return false;
            }
            return true;
        }
        
        private void StartLoadingItems()
        {
            if (!SceneDetector.IsInGame())
            {
                Debug.Log("[ItemReader] Not in game scene, skipping item load");
                needsReload = true;
                return;
            }

            isLoading = true;
            isLoaded = false;
            cachedItems.Clear();
            pendingEntries.Clear();
            currentEntryIndex = 0;
            totalEntries = 0;
            iconConvertIndex = 0;
            iconConvertDelay = 0;

            try
            {
                if (ItemAssetsCollection.Instance == null)
                {
                    Debug.Log("[ItemReader] ItemAssetsCollection.Instance is null, will retry later");
                    isLoading = false;
                    needsReload = true;
                    return;
                }

                var entriesField = typeof(ItemAssetsCollection).GetField("entries", BindingFlags.Public | BindingFlags.Instance);
                if (entriesField == null)
                {
                    Debug.LogWarning("[ItemReader] entries field not found");
                    isLoading = false;
                    return;
                }

                var entries = entriesField.GetValue(ItemAssetsCollection.Instance) as System.Collections.IList;
                if (entries == null)
                {
                    Debug.LogWarning("[ItemReader] entries is null");
                    isLoading = false;
                    return;
                }

                foreach (var entry in entries)
                {
                    pendingEntries.Add(entry);
                }
                
                totalEntries = pendingEntries.Count;
                Debug.Log($"[ItemReader] Starting batch load of {totalEntries} items");
            }
            catch (Exception e)
            {
                Debug.LogError($"[ItemReader] Error starting item load: {e.Message}");
                isLoading = false;
            }
        }
        
        private void ProcessBatchItems()
        {
            int processed = 0;
            while (currentEntryIndex < pendingEntries.Count && processed < ITEMS_PER_FRAME)
            {
                try
                {
                    ExtractItemInfo(pendingEntries[currentEntryIndex]);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[ItemReader] Error extracting item: {e.Message}");
                }
                currentEntryIndex++;
                processed++;
            }
            
            if (currentEntryIndex >= pendingEntries.Count)
            {
                isLoaded = true;
                isLoading = false;
                pendingEntries.Clear();
                Debug.Log($"[ItemReader] Completed loading {cachedItems.Count} items");
            }
        }

        public List<ItemInfo> ReadAllItems()
        {
            if (!isLoaded)
            {
                RequestReload();
            }
            return new List<ItemInfo>(cachedItems);
        }

        public List<ItemInfo> GetItemsWithIcons(int startIndex, int count)
        {
            var result = new List<ItemInfo>();
            int endIndex = Math.Min(startIndex + count, cachedItems.Count);
            
            for (int i = startIndex; i < endIndex; i++)
            {
                var item = cachedItems[i];
                lock (queueLock)
                {
                    if (string.IsNullOrEmpty(item.iconBase64) && iconCache.TryGetValue(item.id, out string cached))
                    {
                        item.iconBase64 = cached;
                    }
                }
                result.Add(item);
            }
            return result;
        }

        public void ConvertIconsForItems(List<ItemInfo> items)
        {
            foreach (var item in items)
            {
                lock (queueLock)
                {
                    if (string.IsNullOrEmpty(item.iconBase64) && iconCache.TryGetValue(item.id, out string cached))
                    {
                        item.iconBase64 = cached;
                    }
                }
            }
        }

        private void ExtractItemInfo(object entry)
        {
            Type entryType = entry.GetType();

            var typeIDField = entryType.GetField("typeID", BindingFlags.Public | BindingFlags.Instance);
            var metaDataField = entryType.GetField("metaData", BindingFlags.Public | BindingFlags.Instance);

            if (typeIDField == null || metaDataField == null) return;

            int typeID = (int)typeIDField.GetValue(entry);
            object metaDataObj = metaDataField.GetValue(entry);

            if (metaDataObj == null) return;

            ItemMetaData metaData = (ItemMetaData)metaDataObj;

            if (metaData.id <= 0) return;

            var itemInfo = new ItemInfo
            {
                id = metaData.id,
                name = metaData.Name ?? "Unknown",
                displayName = metaData.DisplayName ?? "Unknown",
                description = metaData.Description ?? "",
                category = metaData.Catagory ?? "None",
                quality = metaData.quality,
                price = metaData.priceEach,
                maxStack = metaData.maxStackCount,
                iconBase64 = "",
                iconSprite = metaData.icon
            };

            cachedItems.Add(itemInfo);
        }

        public ItemInfo GetItemById(int id)
        {
            foreach (var item in cachedItems)
            {
                if (item.id == id) return item;
            }

            ItemMetaData metaData = ItemAssetsCollection.GetMetaData(id);
            if (metaData.id > 0)
            {
                return new ItemInfo
                {
                    id = metaData.id,
                    name = metaData.Name ?? "Unknown",
                    displayName = metaData.DisplayName ?? "Unknown",
                    description = metaData.Description ?? "",
                    category = metaData.Catagory ?? "None",
                    quality = metaData.quality,
                    price = metaData.priceEach,
                    maxStack = metaData.maxStackCount
                };
            }

            return null;
        }

        public List<ItemInfo> SearchItems(string keyword)
        {
            var results = new List<ItemInfo>();
            string lowerKeyword = keyword.ToLower();

            foreach (var item in cachedItems)
            {
                if (item.name.ToLower().Contains(lowerKeyword) ||
                    item.displayName.ToLower().Contains(lowerKeyword) ||
                    item.id.ToString().Contains(keyword))
                {
                    results.Add(item);
                }
            }

            return results;
        }
    }
}

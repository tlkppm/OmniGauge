using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
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
        private const int ITEMS_PER_FRAME = 20;
        private int totalEntries = 0;

        public void Initialize()
        {
            Debug.Log("[ItemReader] Initialized");
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

        public void RequestReload()
        {
            needsReload = true;
        }

        public void UpdateOnMainThread()
        {
            ProcessGiveItemQueue();
            
            if (needsReload && !isLoading)
            {
                needsReload = false;
                StartLoadingItems();
            }
            
            if (isLoading && pendingEntries.Count > 0)
            {
                ProcessBatchItems();
            }
        }
        
        private void StartLoadingItems()
        {
            isLoading = true;
            isLoaded = false;
            cachedItems.Clear();
            pendingEntries.Clear();
            currentEntryIndex = 0;
            totalEntries = 0;

            try
            {
                if (ItemAssetsCollection.Instance == null)
                {
                    Debug.LogWarning("[ItemReader] ItemAssetsCollection.Instance is null");
                    isLoading = false;
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

            string iconBase64 = "";
            if (metaData.icon != null)
            {
                try
                {
                    iconBase64 = SpriteToBase64(metaData.icon);
                }
                catch
                {
                }
            }

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
                iconBase64 = iconBase64
            };

            cachedItems.Add(itemInfo);
        }

        private string SpriteToBase64(Sprite sprite)
        {
            if (sprite == null || sprite.texture == null) return "";

            try
            {
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
                    if (IsEmptyTexture(pixels)) return "";

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
                    
                    if (IsEmptyTexture(pixels)) return "";

                    readableTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
                    readableTexture.SetPixels(pixels);
                    readableTexture.Apply();
                }

                byte[] pngData = readableTexture.EncodeToPNG();
                UnityEngine.Object.Destroy(readableTexture);

                return Convert.ToBase64String(pngData);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ItemReader] Failed to convert sprite to base64: {e.Message}");
                return "";
            }
        }
        
        private bool IsEmptyTexture(Color[] pixels)
        {
            if (pixels == null || pixels.Length == 0) return true;
            
            int emptyCount = 0;
            int sampleCount = 0;
            int step = Math.Max(1, pixels.Length / 200);
            float minR = 1f, maxR = 0f, minG = 1f, maxG = 0f, minB = 1f, maxB = 0f;
            
            for (int i = 0; i < pixels.Length; i += step)
            {
                sampleCount++;
                Color c = pixels[i];
                
                if (c.a < 0.1f)
                {
                    emptyCount++;
                }
                else if (c.r > 0.85f && c.g > 0.85f && c.b > 0.85f)
                {
                    emptyCount++;
                }
                
                if (c.a > 0.1f)
                {
                    minR = Math.Min(minR, c.r); maxR = Math.Max(maxR, c.r);
                    minG = Math.Min(minG, c.g); maxG = Math.Max(maxG, c.g);
                    minB = Math.Min(minB, c.b); maxB = Math.Max(maxB, c.b);
                }
            }
            
            float emptyRatio = (float)emptyCount / sampleCount;
            float colorVariance = (maxR - minR) + (maxG - minG) + (maxB - minB);
            
            return emptyRatio > 0.90f || colorVariance < 0.1f;
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

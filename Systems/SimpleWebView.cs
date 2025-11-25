using UnityEngine;
using System.Text;
using System.IO;

namespace cvbhhnClassLibrary1.Systems
{
    public class SimpleWebView : MonoBehaviour
    {
        private static SimpleWebView instance;
        private int localPort = 8080;
        private SimpleHTTPServer server;
        private bool isVisible = false;
        private GameTaskReader gameTaskReader;
        private DamageTracker damageTracker;
        private TaskManager taskManager;
        private ItemReader itemReader;

        public static SimpleWebView Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("SimpleWebView");
                    instance = go.AddComponent<SimpleWebView>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void Initialize(GameTaskReader taskReader, DamageTracker tracker, TaskManager manager, ItemReader items)
        {
            Debug.Log("[SimpleWebView] Initializing");
            
            gameTaskReader = taskReader;
            damageTracker = tracker;
            taskManager = manager;
            itemReader = items;
            server = new SimpleHTTPServer(localPort, gameTaskReader, damageTracker, taskManager, itemReader);
            server.Start();
            
            Debug.Log($"[SimpleWebView] Server started on port {localPort}");
        }

        public void Show()
        {
            isVisible = true;
            string url = $"http://localhost:{localPort}";
            Application.OpenURL(url);
            Debug.Log($"[SimpleWebView] Opening browser to {url}");
        }

        public void Hide()
        {
            isVisible = false;
            Debug.Log("[SimpleWebView] Hide requested");
        }

        public void Toggle()
        {
            if (isVisible)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        public void Destroy()
        {
            if (server != null)
            {
                server.Stop();
            }
            
            if (instance == this)
            {
                instance = null;
            }
            
            Destroy(gameObject);
        }
    }

    public class SimpleHTTPServer
    {
        private int port;
        private bool running;
        private System.Net.HttpListener listener;
        private System.Threading.Thread listenerThread;
        private GameTaskReader gameTaskReader;
        private DamageTracker damageTracker;
        private TaskManager taskManager;
        private ItemReader itemReader;

        public SimpleHTTPServer(int port, GameTaskReader taskReader, DamageTracker tracker, TaskManager manager, ItemReader items)
        {
            this.port = port;
            this.gameTaskReader = taskReader;
            this.damageTracker = tracker;
            this.taskManager = manager;
            this.itemReader = items;
        }

        public void Start()
        {
            running = true;
            
            try
            {
                listener = new System.Net.HttpListener();
                listener.Prefixes.Add($"http://localhost:{port}/");
                listener.Start();
                
                listenerThread = new System.Threading.Thread(Listen);
                listenerThread.IsBackground = true;
                listenerThread.Start();
                
                Debug.Log($"[SimpleHTTPServer] Started on port {port}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SimpleHTTPServer] Failed to start: {e.Message}");
            }
        }

        private void Listen()
        {
            while (running)
            {
                try
                {
                    var context = listener.GetContext();
                    ProcessRequest(context);
                }
                catch (System.Exception e)
                {
                    if (running)
                    {
                        Debug.LogError($"[SimpleHTTPServer] Error: {e.Message}");
                    }
                }
            }
        }

        private void ProcessRequest(System.Net.HttpListenerContext context)
        {
            try
            {
                string path = context.Request.Url.AbsolutePath;
                Debug.Log($"[SimpleHTTPServer] Request received: {path}");
                
                if (path == "/api/tasks")
                {
                    HandleTasksAPI(context);
                }
                else if (path == "/api/tasks/complete-game")
                {
                    HandleCompleteGameTaskAPI(context);
                }
                else if (path == "/api/damage")
                {
                    HandleDamageAPI(context);
                }
                else if (path == "/api/damage/stats")
                {
                    HandleDamageStatsAPI(context);
                }
                else if (path == "/api/items")
                {
                    HandleItemsAPI(context);
                }
                else if (path == "/api/items/search")
                {
                    HandleItemsSearchAPI(context);
                }
                else if (path == "/api/items/give")
                {
                    HandleGiveItemAPI(context);
                }
                else
                {
                    string html = WebUIContent.GetHTML();
                    byte[] buffer = Encoding.UTF8.GetBytes(html);
                    
                    context.Response.ContentType = "text/html; charset=utf-8";
                    context.Response.ContentLength64 = buffer.Length;
                    context.Response.StatusCode = 200;
                    
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    context.Response.OutputStream.Close();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SimpleHTTPServer] Error processing request: {e.Message}");
            }
        }

        private void HandleTasksAPI(System.Net.HttpListenerContext context)
        {
            try
            {
                var tasks = gameTaskReader.GetCachedTasks();
                Debug.Log($"[SimpleHTTPServer] Tasks API called, returning {tasks.Count} tasks");
                
                StringBuilder json = new StringBuilder();
                json.Append("[");
                
                for (int i = 0; i < tasks.Count; i++)
                {
                    if (i > 0) json.Append(",");
                    json.Append("{");
                    json.Append($"\"id\":{tasks[i].id},");
                    json.Append($"\"name\":\"{EscapeJson(tasks[i].name)}\",");
                    json.Append($"\"status\":\"{EscapeJson(tasks[i].status)}\",");
                    json.Append($"\"progress\":\"{EscapeJson(tasks[i].progress)}\",");
                    json.Append($"\"objectives\":\"{EscapeJson(tasks[i].objectives ?? "")}\",");
                    json.Append($"\"rewards\":\"{EscapeJson(tasks[i].rewards ?? "")}\",");
                    json.Append($"\"canComplete\":{tasks[i].canComplete.ToString().ToLower()}");
                    json.Append("}");
                }
                
                json.Append("]");
                
                byte[] buffer = Encoding.UTF8.GetBytes(json.ToString());
                context.Response.ContentType = "application/json; charset=utf-8";
                context.Response.ContentLength64 = buffer.Length;
                context.Response.StatusCode = 200;
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
                
                Debug.Log($"[SimpleHTTPServer] Tasks API response sent successfully");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SimpleHTTPServer] Error handling tasks API: {e.Message}");
                SendErrorResponse(context, e.Message);
            }
        }

        private void HandleDamageAPI(System.Net.HttpListenerContext context)
        {
            try
            {
                var records = damageTracker.GetAllRecords();
                StringBuilder json = new StringBuilder();
                json.Append("[");
                
                for (int i = 0; i < records.Count; i++)
                {
                    if (i > 0) json.Append(",");
                    json.Append("{");
                    json.Append($"\"damage\":{records[i].value},");
                    json.Append($"\"weapon\":\"{EscapeJson(records[i].source)}\",");
                    json.Append($"\"time\":\"{System.DateTime.Now.ToString("HH:mm:ss")}\",");
                    json.Append($"\"damageType\":\"{EscapeJson(records[i].damageType)}\",");
                    json.Append($"\"target\":\"{EscapeJson(records[i].targetName)}\",");
                    json.Append($"\"isCrit\":{records[i].isCrit.ToString().ToLower()}");
                    json.Append("}");
                }
                
                json.Append("]");
                
                byte[] buffer = Encoding.UTF8.GetBytes(json.ToString());
                context.Response.ContentType = "application/json; charset=utf-8";
                context.Response.ContentLength64 = buffer.Length;
                context.Response.StatusCode = 200;
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SimpleHTTPServer] Error handling damage API: {e.Message}");
                SendErrorResponse(context, e.Message);
            }
        }

        private void HandleDamageStatsAPI(System.Net.HttpListenerContext context)
        {
            try
            {
                var weaponDamage = damageTracker.GetDamageByWeapon();
                float total = damageTracker.GetTotalDamage();
                int hitCount = damageTracker.GetHitCount();
                float maxDamage = damageTracker.GetMaxDamage();
                
                StringBuilder json = new StringBuilder();
                json.Append("{");
                json.Append($"\"totalDamage\":{total},");
                json.Append($"\"maxDamage\":{maxDamage},");
                json.Append($"\"hitCount\":{hitCount},");
                json.Append($"\"avgDamage\":{(hitCount > 0 ? total / hitCount : 0)},");
                json.Append("\"weaponDamage\":{");
                
                bool first = true;
                foreach (var kvp in weaponDamage)
                {
                    if (!first) json.Append(",");
                    json.Append($"\"{EscapeJson(kvp.Key)}\":{kvp.Value}");
                    first = false;
                }
                
                json.Append("}");
                json.Append("}");
                
                byte[] buffer = Encoding.UTF8.GetBytes(json.ToString());
                context.Response.ContentType = "application/json; charset=utf-8";
                context.Response.ContentLength64 = buffer.Length;
                context.Response.StatusCode = 200;
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SimpleHTTPServer] Error handling damage stats API: {e.Message}");
                SendErrorResponse(context, e.Message);
            }
        }

        private void HandleCompleteGameTaskAPI(System.Net.HttpListenerContext context)
        {
            try
            {
                using (var reader = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    string body = reader.ReadToEnd();
                    var parts = System.Web.HttpUtility.ParseQueryString(body);
                    
                    int id = int.Parse(parts["id"] ?? "0");
                    bool result = gameTaskReader.TryCompleteGameQuest(id);
                    
                    string json = $"{{\"success\":{result.ToString().ToLower()}}}";
                    byte[] buffer = Encoding.UTF8.GetBytes(json);
                    context.Response.ContentType = "application/json; charset=utf-8";
                    context.Response.ContentLength64 = buffer.Length;
                    context.Response.StatusCode = 200;
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    context.Response.OutputStream.Close();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SimpleHTTPServer] Error handling complete game task API: {e.Message}");
                SendErrorResponse(context, e.Message);
            }
        }

        private void HandleItemsAPI(System.Net.HttpListenerContext context)
        {
            try
            {
                if (!itemReader.IsLoaded)
                {
                    if (!itemReader.IsLoading)
                    {
                        itemReader.RequestReload();
                    }
                    int progress = itemReader.LoadProgress;
                    string loadingJson = $"{{\"loading\":true,\"progress\":{progress},\"message\":\"正在加载物品数据 ({progress}%)...\"}}";
                    byte[] loadingBuffer = Encoding.UTF8.GetBytes(loadingJson);
                    context.Response.ContentType = "application/json; charset=utf-8";
                    context.Response.ContentLength64 = loadingBuffer.Length;
                    context.Response.StatusCode = 200;
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    context.Response.OutputStream.Write(loadingBuffer, 0, loadingBuffer.Length);
                    context.Response.OutputStream.Close();
                    return;
                }

                var items = itemReader.GetCachedItems();
                Debug.Log($"[SimpleHTTPServer] Items API called, returning {items.Count} cached items");

                StringBuilder json = new StringBuilder();
                json.Append("[");

                for (int i = 0; i < items.Count; i++)
                {
                    if (i > 0) json.Append(",");
                    json.Append("{");
                    json.Append($"\"id\":{items[i].id},");
                    json.Append($"\"name\":\"{EscapeJson(items[i].name)}\",");
                    json.Append($"\"displayName\":\"{EscapeJson(items[i].displayName)}\",");
                    json.Append($"\"description\":\"{EscapeJson(items[i].description)}\",");
                    json.Append($"\"category\":\"{EscapeJson(items[i].category)}\",");
                    json.Append($"\"quality\":{items[i].quality},");
                    json.Append($"\"price\":{items[i].price},");
                    json.Append($"\"maxStack\":{items[i].maxStack},");
                    json.Append($"\"icon\":\"{EscapeJson(items[i].iconBase64)}\"");
                    json.Append("}");
                }

                json.Append("]");

                byte[] buffer = Encoding.UTF8.GetBytes(json.ToString());
                context.Response.ContentType = "application/json; charset=utf-8";
                context.Response.ContentLength64 = buffer.Length;
                context.Response.StatusCode = 200;
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SimpleHTTPServer] Error handling items API: {e.Message}");
                SendErrorResponse(context, e.Message);
            }
        }

        private void HandleItemsSearchAPI(System.Net.HttpListenerContext context)
        {
            try
            {
                string query = context.Request.QueryString["q"] ?? "";
                var items = itemReader.SearchItems(query);

                StringBuilder json = new StringBuilder();
                json.Append("[");

                for (int i = 0; i < items.Count; i++)
                {
                    if (i > 0) json.Append(",");
                    json.Append("{");
                    json.Append($"\"id\":{items[i].id},");
                    json.Append($"\"name\":\"{EscapeJson(items[i].name)}\",");
                    json.Append($"\"displayName\":\"{EscapeJson(items[i].displayName)}\",");
                    json.Append($"\"category\":\"{EscapeJson(items[i].category)}\",");
                    json.Append($"\"quality\":{items[i].quality},");
                    json.Append($"\"price\":{items[i].price}");
                    json.Append("}");
                }

                json.Append("]");

                byte[] buffer = Encoding.UTF8.GetBytes(json.ToString());
                context.Response.ContentType = "application/json; charset=utf-8";
                context.Response.ContentLength64 = buffer.Length;
                context.Response.StatusCode = 200;
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SimpleHTTPServer] Error handling items search API: {e.Message}");
                SendErrorResponse(context, e.Message);
            }
        }

        private void HandleGiveItemAPI(System.Net.HttpListenerContext context)
        {
            try
            {
                string idStr = "";
                string quantityStr = "1";
                
                if (context.Request.HttpMethod == "POST")
                {
                    using (var reader = new System.IO.StreamReader(context.Request.InputStream))
                    {
                        string body = reader.ReadToEnd();
                        var pairs = body.Split('&');
                        foreach (var pair in pairs)
                        {
                            var kv = pair.Split('=');
                            if (kv.Length == 2)
                            {
                                if (kv[0] == "id") idStr = System.Uri.UnescapeDataString(kv[1]);
                                else if (kv[0] == "quantity") quantityStr = System.Uri.UnescapeDataString(kv[1]);
                            }
                        }
                    }
                }
                else
                {
                    idStr = context.Request.QueryString["id"] ?? "";
                    quantityStr = context.Request.QueryString["quantity"] ?? "1";
                }
                
                if (!int.TryParse(idStr, out int itemId) || itemId <= 0)
                {
                    SendErrorResponse(context, "Invalid item ID");
                    return;
                }
                
                int.TryParse(quantityStr, out int quantity);
                if (quantity <= 0) quantity = 1;
                if (quantity > 99) quantity = 99;
                
                itemReader.RequestGiveItem(itemId, quantity);
                
                string json = $"{{\"success\":true,\"itemId\":{itemId},\"quantity\":{quantity}}}";
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                context.Response.ContentType = "application/json; charset=utf-8";
                context.Response.ContentLength64 = buffer.Length;
                context.Response.StatusCode = 200;
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
                
                Debug.Log($"[SimpleHTTPServer] Give item request queued: {itemId} x{quantity}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SimpleHTTPServer] Error handling give item API: {e.Message}");
                SendErrorResponse(context, e.Message);
            }
        }

        private void SendErrorResponse(System.Net.HttpListenerContext context, string error)
        {
            try
            {
                string json = $"{{\"error\":\"{EscapeJson(error)}\"}}";
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                context.Response.ContentType = "application/json; charset=utf-8";
                context.Response.ContentLength64 = buffer.Length;
                context.Response.StatusCode = 500;
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
            }
            catch { }
        }

        private string EscapeJson(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
        }

        public void Stop()
        {
            running = false;
            
            if (listener != null)
            {
                listener.Stop();
                listener.Close();
            }
            
            if (listenerThread != null)
            {
                listenerThread.Join(1000);
            }
            
            Debug.Log("[SimpleHTTPServer] Stopped");
        }
    }
}

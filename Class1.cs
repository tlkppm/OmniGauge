using UnityEngine;
using cvbhhnClassLibrary1.Systems;

namespace cvbhhnClassLibrary1
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private SimpleWebView webViewSystem;
        private DamageTracker damageTracker;
        private TaskManager taskManager;
        private GameTaskReader gameTaskReader;
        private GameHooks gameHooks;
        private ItemReader itemReader;
        private KeyCode toggleKey = KeyCode.T;

        void Start()
        {
            Debug.Log("[ModBehaviour] Test Tool v1.0.0 starting");
            InitializeSystems();
            Debug.Log("[ModBehaviour] Test Tool started successfully");
        }

        void Update()
        {
            if (itemReader != null)
            {
                itemReader.UpdateOnMainThread();
            }

            if (Input.GetKeyDown(toggleKey))
            {
                Debug.Log($"[ModBehaviour] Toggle key pressed: {toggleKey}");
                webViewSystem.Toggle();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                gameTaskReader.ReadAllGameTasks();
                itemReader.RequestReload();
                Debug.Log("[ModBehaviour] Read all game tasks and items");
            }
        }


        void InitializeSystems()
        {
            Debug.Log("[ModBehaviour] Initializing systems");
            
            damageTracker = new DamageTracker();
            damageTracker.Initialize();

            taskManager = new TaskManager();
            taskManager.Initialize();

            gameTaskReader = new GameTaskReader();
            gameTaskReader.Initialize();

            itemReader = new ItemReader();
            itemReader.Initialize();

            webViewSystem = SimpleWebView.Instance;
            webViewSystem.Initialize(gameTaskReader, damageTracker, taskManager, itemReader);

            gameHooks = new GameHooks();
            gameHooks.Initialize(damageTracker, taskManager);
            
            Debug.Log("[ModBehaviour] All systems initialized");
        }

        void OnDestroy()
        {
            Debug.Log("[ModBehaviour] Destroying mod");
            
            if (gameHooks != null)
            {
                gameHooks.Destroy();
            }
            
            if (webViewSystem != null)
            {
                webViewSystem.Destroy();
            }
            
            Debug.Log("[ModBehaviour] Mod destroyed successfully");
        }
    }
}

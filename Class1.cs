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
        private bool wasInGame = false;
        private bool systemsInitialized = false;

        void Start()
        {
            Debug.Log("[ModBehaviour] OmniGauge v1.0.0 starting");
            InitializeBaseSystems();
            Debug.Log("[ModBehaviour] OmniGauge base systems initialized, waiting for game scene");
        }

        void Update()
        {
            bool isInGame = SceneDetector.IsInGame();
            
            if (isInGame && !wasInGame)
            {
                Debug.Log($"[ModBehaviour] Entered game scene: {SceneDetector.GetCurrentSceneName()}");
                if (!systemsInitialized)
                {
                    InitializeGameSystems();
                    systemsInitialized = true;
                }
            }
            else if (!isInGame && wasInGame)
            {
                Debug.Log($"[ModBehaviour] Left game scene, current: {SceneDetector.GetCurrentSceneName()}");
            }
            wasInGame = isInGame;

            if (!isInGame)
                return;

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
                if (SceneDetector.IsInGame())
                {
                    gameTaskReader.ReadAllGameTasks();
                    itemReader.RequestReload();
                    Debug.Log("[ModBehaviour] Read all game tasks and items");
                }
                else
                {
                    Debug.Log("[ModBehaviour] Cannot reload data in main menu");
                }
            }
        }


        void InitializeBaseSystems()
        {
            Debug.Log("[ModBehaviour] Initializing base systems");
            
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
            
            Debug.Log("[ModBehaviour] Base systems initialized");
        }

        void InitializeGameSystems()
        {
            Debug.Log("[ModBehaviour] Initializing game systems");
            
            gameHooks = new GameHooks();
            gameHooks.Initialize(damageTracker, taskManager);
            
            Debug.Log("[ModBehaviour] Game systems initialized");
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

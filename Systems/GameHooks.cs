using UnityEngine;
using System;
using System.Reflection;
using HarmonyLib;

namespace cvbhhnClassLibrary1.Systems
{
    public class GameHooks
    {
        private DamageTracker damageTracker;
        private TaskManager taskManager;
        private RealDamageTracker realDamageTracker;
        private Harmony harmony;

        public void Initialize(DamageTracker tracker, TaskManager manager)
        {
            damageTracker = tracker;
            taskManager = manager;
            
            try
            {
                harmony = new Harmony("cvbhhnClassLibrary1.GameHooks");
                
                realDamageTracker = RealDamageTracker.Instance;
                realDamageTracker.Initialize(tracker);
                
                PatchGameMethods();
                Debug.Log("[GameHooks] Initialized and patched game methods");
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameHooks] Failed to initialize: {e.Message}");
            }
        }

        private void PatchGameMethods()
        {
            try
            {
                PatchHealthHurt();
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameHooks] Error patching methods: {e.Message}");
            }
        }

        private void PatchHealthHurt()
        {
            try
            {
                Type healthType = FindType("Health");
                if (healthType == null)
                {
                    Debug.LogWarning("[GameHooks] Health type not found in any assembly");
                    return;
                }
                
                Debug.Log($"[GameHooks] Found Health type in assembly: {healthType.Assembly.GetName().Name}");

                MethodInfo hurtMethod = healthType.GetMethod("Hurt", BindingFlags.Public | BindingFlags.Instance);
                if (hurtMethod == null)
                {
                    Debug.LogWarning("[GameHooks] Health.Hurt method not found");
                    return;
                }

                var postfix = typeof(GameHooks_Patches).GetMethod("HealthHurtPostfix");
                harmony.Patch(hurtMethod, null, new HarmonyMethod(postfix));
                
                GameHooks_Patches.SetRealDamageTracker(realDamageTracker);
                
                Debug.Log("[GameHooks] Successfully patched Health.Hurt");
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameHooks] Failed to patch Health.Hurt: {e.Message}");
            }
        }
        
        private Type FindType(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.Name == typeName)
                        {
                            return type;
                        }
                    }
                }
                catch
                {
                }
            }
            return null;
        }


        public void Destroy()
        {
            if (harmony != null)
            {
                harmony.UnpatchAll("cvbhhnClassLibrary1.GameHooks");
                Debug.Log("[GameHooks] Unpatched all methods");
            }
        }
    }

    public static class GameHooks_Patches
    {
        private static DamageTracker damageTracker;
        private static TaskManager taskManager;
        private static RealDamageTracker realDamageTracker;

        public static void SetInstances(DamageTracker tracker, TaskManager manager)
        {
            damageTracker = tracker;
            taskManager = manager;
        }

        public static void SetRealDamageTracker(RealDamageTracker tracker)
        {
            realDamageTracker = tracker;
        }

        public static void HealthHurtPostfix(object __instance, object damageInfo, bool __result)
        {
            try
            {
                if (!__result) return;
                
                if (realDamageTracker != null && damageInfo != null)
                {
                    realDamageTracker.OnDamageDealt(damageInfo);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameHooks_Patches] HealthHurtPostfix error: {e.Message}");
            }
        }


        public static void DeathPrefix(object __instance, params object[] __args)
        {
            try
            {
                if (taskManager != null)
                {
                    string enemyType = __instance?.GetType().Name ?? "Enemy";
                    taskManager.CheckKillTask(enemyType, 1);
                }
            }
            catch
            {
            }
        }

        public static void ItemPickupPrefix(object __instance, params object[] __args)
        {
            try
            {
                if (taskManager != null && __args != null && __args.Length > 0)
                {
                    string itemId = "Item";
                    
                    foreach (var arg in __args)
                    {
                        if (arg != null)
                        {
                            itemId = arg.ToString();
                            break;
                        }
                    }
                    
                    taskManager.CheckPickupTask(itemId);
                }
            }
            catch
            {
            }
        }

        public static void TriggerPrefix(object __instance, params object[] __args)
        {
            try
            {
                if (taskManager != null)
                {
                    string triggerId = __instance?.GetType().Name ?? "Trigger";
                    
                    if (__args != null && __args.Length > 0)
                    {
                        var collider = __args[0] as Collider;
                        if (collider != null)
                        {
                            triggerId = collider.gameObject.name;
                        }
                    }
                    
                    taskManager.CheckTriggerTask(triggerId);
                }
            }
            catch
            {
            }
        }
    }
}

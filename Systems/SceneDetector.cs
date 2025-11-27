using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;

namespace cvbhhnClassLibrary1.Systems
{
    public static class SceneDetector
    {
        private static readonly string[] MainMenuSceneNames = new string[]
        {
            "MainMenu",
            "Main Menu",
            "Menu",
            "Title",
            "TitleScreen",
            "StartScene",
            "Start"
        };

        private static readonly string[] GameSceneNames = new string[]
        {
            "Game",
            "Gameplay",
            "Level",
            "World",
            "Base",
            "Hideout",
            "Shelter"
        };

        private static Type cachedPlayerType;
        private static Type cachedGameManagerType;
        private static Type cachedCheatingManagerType;
        private static PropertyInfo cachedPlayerInstanceProp;
        private static PropertyInfo cachedGameManagerInstanceProp;
        private static PropertyInfo cachedCheatingManagerInstanceProp;
        private static bool typesInitialized = false;
        private static string lastSceneName = "";
        private static bool lastIsInGame = false;
        private static int cacheFrameCount = 0;

        private static void InitializeTypes()
        {
            if (typesInitialized) return;
            typesInitialized = true;

            cachedPlayerType = FindTypeCached("Player");
            cachedGameManagerType = FindTypeCached("GameManager");
            cachedCheatingManagerType = FindTypeCached("CheatingManager");

            if (cachedPlayerType != null)
            {
                cachedPlayerInstanceProp = cachedPlayerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)
                    ?? cachedPlayerType.GetProperty("LocalPlayer", BindingFlags.Public | BindingFlags.Static);
            }
            if (cachedGameManagerType != null)
            {
                cachedGameManagerInstanceProp = cachedGameManagerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            }
            if (cachedCheatingManagerType != null)
            {
                cachedCheatingManagerInstanceProp = cachedCheatingManagerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            }
        }

        public static bool IsInGame()
        {
            try
            {
                string currentSceneName = SceneManager.GetActiveScene().name;
                
                if (currentSceneName == lastSceneName && cacheFrameCount < 30)
                {
                    cacheFrameCount++;
                    return lastIsInGame;
                }
                
                cacheFrameCount = 0;
                lastSceneName = currentSceneName;
                
                if (string.IsNullOrEmpty(currentSceneName))
                {
                    lastIsInGame = false;
                    return false;
                }

                foreach (var menuName in MainMenuSceneNames)
                {
                    if (currentSceneName.IndexOf(menuName, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        lastIsInGame = false;
                        return false;
                    }
                }

                InitializeTypes();

                if (CheckPlayerExistsCached())
                {
                    lastIsInGame = true;
                    return true;
                }

                foreach (var gameName in GameSceneNames)
                {
                    if (currentSceneName.IndexOf(gameName, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        lastIsInGame = true;
                        return true;
                    }
                }

                lastIsInGame = CheckGameManagerExistsCached();
                return lastIsInGame;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SceneDetector] Error checking scene: {e.Message}");
                return false;
            }
        }

        public static bool IsInMainMenu()
        {
            return !IsInGame();
        }

        public static string GetCurrentSceneName()
        {
            try
            {
                return SceneManager.GetActiveScene().name;
            }
            catch
            {
                return "Unknown";
            }
        }

        private static bool CheckPlayerExistsCached()
        {
            try
            {
                if (cachedPlayerInstanceProp != null)
                {
                    var instance = cachedPlayerInstanceProp.GetValue(null);
                    return instance != null;
                }
            }
            catch
            {
            }
            return false;
        }

        private static bool CheckGameManagerExistsCached()
        {
            try
            {
                if (cachedGameManagerInstanceProp != null)
                {
                    var instance = cachedGameManagerInstanceProp.GetValue(null);
                    return instance != null;
                }

                if (cachedCheatingManagerInstanceProp != null)
                {
                    var instance = cachedCheatingManagerInstanceProp.GetValue(null);
                    return instance != null;
                }
            }
            catch
            {
            }
            return false;
        }

        private static Type FindTypeCached(string typeName)
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
    }
}

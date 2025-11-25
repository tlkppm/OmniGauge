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

        public static bool IsInGame()
        {
            try
            {
                string currentSceneName = SceneManager.GetActiveScene().name;
                
                if (string.IsNullOrEmpty(currentSceneName))
                    return false;

                foreach (var menuName in MainMenuSceneNames)
                {
                    if (currentSceneName.IndexOf(menuName, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return false;
                    }
                }

                if (CheckPlayerExists())
                {
                    return true;
                }

                foreach (var gameName in GameSceneNames)
                {
                    if (currentSceneName.IndexOf(gameName, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }

                return CheckGameManagerExists();
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

        private static bool CheckPlayerExists()
        {
            try
            {
                var playerType = FindType("Player");
                if (playerType != null)
                {
                    var instanceProp = playerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                    if (instanceProp != null)
                    {
                        var instance = instanceProp.GetValue(null);
                        return instance != null;
                    }
                    
                    var localPlayerProp = playerType.GetProperty("LocalPlayer", BindingFlags.Public | BindingFlags.Static);
                    if (localPlayerProp != null)
                    {
                        var localPlayer = localPlayerProp.GetValue(null);
                        return localPlayer != null;
                    }
                }

                var player = GameObject.FindObjectOfType<MonoBehaviour>();
                if (player != null && player.GetType().Name.Contains("Player"))
                {
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        private static bool CheckGameManagerExists()
        {
            try
            {
                var gameManagerType = FindType("GameManager");
                if (gameManagerType != null)
                {
                    var instanceProp = gameManagerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                    if (instanceProp != null)
                    {
                        var instance = instanceProp.GetValue(null);
                        if (instance != null)
                        {
                            var isInGameProp = gameManagerType.GetProperty("IsInGame", BindingFlags.Public | BindingFlags.Instance);
                            if (isInGameProp != null)
                            {
                                return (bool)isInGameProp.GetValue(instance);
                            }
                            return true;
                        }
                    }
                }

                var cheatingManagerType = FindType("CheatingManager");
                if (cheatingManagerType != null)
                {
                    var instanceProp = cheatingManagerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                    if (instanceProp != null)
                    {
                        var instance = instanceProp.GetValue(null);
                        return instance != null;
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        private static Type FindType(string typeName)
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

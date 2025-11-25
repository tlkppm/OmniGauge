using UnityEngine;
using ItemStatsSystem;
using System.Collections.Generic;
using System.Reflection;

namespace cvbhhnClassLibrary1.Systems
{
    public class RealDamageTracker : MonoBehaviour
    {
        private static RealDamageTracker instance;
        private DamageTracker damageTracker;
        private List<Health> trackedHealthComponents = new List<Health>();
        
        public static RealDamageTracker Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("RealDamageTracker");
                    instance = go.AddComponent<RealDamageTracker>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        public void Initialize(DamageTracker tracker)
        {
            damageTracker = tracker;
            Debug.Log("[RealDamageTracker] Initialized");
        }

        void Update()
        {
            if (LevelManager.Instance == null || LevelManager.Instance.MainCharacter == null)
            {
                return;
            }

            FindAndTrackHealthComponents();
        }

        private void FindAndTrackHealthComponents()
        {
            Health[] allHealthComponents = FindObjectsOfType<Health>();
            
            foreach (var health in allHealthComponents)
            {
                if (!trackedHealthComponents.Contains(health))
                {
                    trackedHealthComponents.Add(health);
                    HookHealthComponent(health);
                }
            }
        }

        private void HookHealthComponent(Health health)
        {
            health.OnHealthChange.AddListener((h) => OnHealthChanged(h));
        }

        private void OnHealthChanged(Health health)
        {
            if (health == null) return;
            
            if (health.IsMainCharacterHealth)
            {
                return;
            }

            if (LevelManager.Instance.MainCharacter == null) return;
            
            CharacterMainControl mainChar = LevelManager.Instance.MainCharacter;
            ItemAgentHolder agentHolder = mainChar.agentHolder;
            
            if (agentHolder == null) return;

            string weaponName = GetCurrentWeaponName(agentHolder);
            
            Debug.Log($"[RealDamageTracker] Health changed, weapon: {weaponName}");
        }

        public void OnDamageDealt(object damageInfoObj)
        {
            if (damageInfoObj == null) return;

            try
            {
                System.Type damageInfoType = damageInfoObj.GetType();
                
                var finalDamageField = damageInfoType.GetField("finalDamage");
                var fromCharacterField = damageInfoType.GetField("fromCharacter");
                var fromWeaponItemIDField = damageInfoType.GetField("fromWeaponItemID");
                var isExplosionField = damageInfoType.GetField("isExplosion");
                
                if (finalDamageField == null || fromCharacterField == null)
                {
                    Debug.LogWarning("[RealDamageTracker] Missing required fields in DamageInfo");
                    return;
                }
                
                float damage = (float)finalDamageField.GetValue(damageInfoObj);
                object fromCharacter = fromCharacterField.GetValue(damageInfoObj);
                
                if (fromCharacter == null) return;
                if (LevelManager.Instance == null || LevelManager.Instance.MainCharacter == null) return;
                if (fromCharacter != LevelManager.Instance.MainCharacter) return;
                
                if (damage <= 0) return;
                
                string weaponName = GetWeaponNameFromDamageInfo(damageInfoObj, fromWeaponItemIDField, isExplosionField, fromCharacter);
                
                string damageType = "普通";
                var damageTypeField = damageInfoType.GetField("damageType");
                if (damageTypeField != null)
                {
                    object damageTypeValue = damageTypeField.GetValue(damageInfoObj);
                    damageType = damageTypeValue?.ToString() ?? "普通";
                }
                
                bool isCrit = false;
                var critField = damageInfoType.GetField("crit");
                if (critField != null)
                {
                    int critValue = (int)critField.GetValue(damageInfoObj);
                    isCrit = critValue > 0;
                }
                
                string targetName = "未知目标";
                var toDamageReceiverField = damageInfoType.GetField("toDamageReceiver");
                if (toDamageReceiverField != null)
                {
                    object toReceiver = toDamageReceiverField.GetValue(damageInfoObj);
                    if (toReceiver != null)
                    {
                        try
                        {
                            System.Type receiverType = toReceiver.GetType();
                            var characterField = receiverType.GetField("character", BindingFlags.Public | BindingFlags.Instance);
                            if (characterField != null)
                            {
                                object character = characterField.GetValue(toReceiver);
                                if (character != null)
                                {
                                    System.Type charType = character.GetType();
                                    var presetField = charType.GetField("characterPreset", BindingFlags.Public | BindingFlags.Instance);
                                    if (presetField != null)
                                    {
                                        object preset = presetField.GetValue(character);
                                        if (preset != null)
                                        {
                                            System.Type presetType = preset.GetType();
                                            var displayNameProp = presetType.GetProperty("DisplayName", BindingFlags.Public | BindingFlags.Instance);
                                            if (displayNameProp != null)
                                            {
                                                targetName = displayNameProp.GetValue(preset) as string ?? "未知目标";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                
                if (damageTracker != null)
                {
                    damageTracker.RecordDamage(damage, weaponName, damageType, targetName, isCrit);
                }
                
                string critText = isCrit ? " [暴击]" : "";
                Debug.Log($"[RealDamageTracker] Damage dealt: {damage:F1} with {weaponName} to {targetName} ({damageType}){critText}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[RealDamageTracker] Error in OnDamageDealt: {e.Message}");
            }
        }

        private string GetWeaponNameFromDamageInfo(object damageInfoObj, FieldInfo weaponItemIDField, FieldInfo isExplosionField, object fromCharacter)
        {
            try
            {
                bool isExplosion = false;
                if (isExplosionField != null)
                {
                    isExplosion = (bool)isExplosionField.GetValue(damageInfoObj);
                }
                
                string weaponName = null;
                
                if (weaponItemIDField != null)
                {
                    int weaponItemID = (int)weaponItemIDField.GetValue(damageInfoObj);
                    if (weaponItemID > 0)
                    {
                        ItemMetaData metaData = ItemAssetsCollection.GetMetaData(weaponItemID);
                        if (metaData.id > 0)
                        {
                            weaponName = metaData.DisplayName;
                        }
                    }
                }
                
                if (weaponName == null && fromCharacter != null)
                {
                    System.Type charType = fromCharacter.GetType();
                    var agentHolderField = charType.GetField("agentHolder", BindingFlags.Public | BindingFlags.Instance);
                    if (agentHolderField != null)
                    {
                        ItemAgentHolder agentHolder = agentHolderField.GetValue(fromCharacter) as ItemAgentHolder;
                        if (agentHolder != null)
                        {
                            weaponName = GetCurrentWeaponName(agentHolder);
                        }
                    }
                }
                
                if (weaponName == null)
                {
                    weaponName = isExplosion ? "爆炸物" : "未知武器";
                }
                
                if (isExplosion)
                {
                    return $"{weaponName} (爆炸)";
                }
                
                return weaponName;
            }
            catch
            {
            }
            
            return "未知武器";
        }

        private string GetCurrentWeaponName(ItemAgentHolder agentHolder)
        {
            if (agentHolder.CurrentHoldGun != null)
            {
                Item gunItem = agentHolder.CurrentHoldGun.Item;
                if (gunItem != null)
                {
                    return $"{gunItem.DisplayName} (Gun)";
                }
            }

            if (agentHolder.CurrentHoldMeleeWeapon != null)
            {
                Item meleeItem = agentHolder.CurrentHoldMeleeWeapon.Item;
                if (meleeItem != null)
                {
                    return $"{meleeItem.DisplayName} (Melee)";
                }
            }

            if (agentHolder.CurrentHoldItemAgent != null && agentHolder.CurrentHoldItemAgent.Item != null)
            {
                Item currentItem = agentHolder.CurrentHoldItemAgent.Item;
                return currentItem.DisplayName;
            }

            return "Unarmed";
        }

        public string GetPlayerCurrentWeapon()
        {
            if (LevelManager.Instance == null || LevelManager.Instance.MainCharacter == null)
            {
                return "No Character";
            }

            CharacterMainControl mainChar = LevelManager.Instance.MainCharacter;
            if (mainChar.agentHolder == null)
            {
                return "No Weapon Holder";
            }

            return GetCurrentWeaponName(mainChar.agentHolder);
        }
    }
}

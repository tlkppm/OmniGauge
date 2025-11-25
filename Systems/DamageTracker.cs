using UnityEngine;
using System.Collections.Generic;

namespace cvbhhnClassLibrary1.Systems
{
    public class DamageTracker
    {
        private List<DamageRecord> damageRecords = new List<DamageRecord>();
        
        public struct DamageRecord
        {
            public float value;
            public string source;
            public float gameTime;
            public System.DateTime realTime;
            public string damageType;
            public string targetName;
            public bool isCrit;
        }

        public void Initialize()
        {
            Debug.Log("[DamageTracker] Initialized");
        }

        public void RecordDamage(float damage, string source = "Unknown", string damageType = "普通", string targetName = "未知", bool isCrit = false)
        {
            var record = new DamageRecord
            {
                value = damage,
                source = source,
                gameTime = Time.time,
                realTime = System.DateTime.Now,
                damageType = damageType,
                targetName = targetName,
                isCrit = isCrit
            };
            
            damageRecords.Add(record);
            
            string critText = isCrit ? " [暴击]" : "";
            Debug.Log($"[DamageTracker] Recorded damage: {damage:F1} from {source} to {targetName} ({damageType}){critText}");
        }

        public float GetTotalDamage()
        {
            float total = 0;
            foreach (var record in damageRecords)
            {
                total += record.value;
            }
            return total;
        }

        public int GetHitCount()
        {
            return damageRecords.Count;
        }

        public float GetMaxDamage()
        {
            float max = 0;
            foreach (var record in damageRecords)
            {
                if (record.value > max)
                {
                    max = record.value;
                }
            }
            return max;
        }

        public void Clear()
        {
            damageRecords.Clear();
            Debug.Log("[DamageTracker] Cleared all damage records");
        }

        public List<DamageRecord> GetAllRecords()
        {
            return new List<DamageRecord>(damageRecords);
        }

        public Dictionary<string, float> GetDamageByWeapon()
        {
            Dictionary<string, float> weaponDamage = new Dictionary<string, float>();
            foreach (var record in damageRecords)
            {
                if (!weaponDamage.ContainsKey(record.source))
                {
                    weaponDamage[record.source] = 0;
                }
                weaponDamage[record.source] += record.value;
            }
            return weaponDamage;
        }
    }
}

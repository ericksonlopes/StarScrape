using UnityEngine;

namespace StarScrape.Data
{
    [CreateAssetMenu(fileName = "New Ship Stats", menuName = "StarScrape/Ship Stats")]
    public class ShipStatsSO : ScriptableObject
    {
        [Header("General")]
        public string shipName;
        public string description;

        [Header("Economy")]
        public float baseCostScrap = 10f;
        public float baseCostPlasma = 0f;
        public float costMultiplier = 1.15f; // Cost increases by 15% each purchase

        [Header("Stats")]
        public float baseValue1 = 5f; // Could be Speed, Processing Limit, or Damage
        public float baseValue2 = 1f; // Secondary stat if needed

        public float CalculateCost(int currentLevel, bool isPlasma = false)
        {
            float baseCost = isPlasma ? baseCostPlasma : baseCostScrap;
            if (baseCost <= 0) return 0;
            return baseCost * Mathf.Pow(costMultiplier, currentLevel);
        }
    }
}

using UnityEngine;
using StarScrape.Core;
using StarScrape.Ships;
using StarScrape.Data;

namespace StarScrape.Upgrades
{
    public class UpgradeManager : MonoBehaviour
    {
        public static UpgradeManager Instance { get; private set; }

        [Header("Stats Data")]
        public ShipStatsSO mothershipSpeedStats;
        public ShipStatsSO scraperStats;
        public ShipStatsSO sentinelStats;

        [Header("Current Levels")]
        public int mothershipSpeedLevel = 0;
        public int scraperCount = 0;
        public int sentinelCount = 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void UpgradeMothershipSpeed()
        {
            float cost = mothershipSpeedStats.CalculateCost(mothershipSpeedLevel);
            if (ResourceManager.Instance.TrySpendScrap(cost))
            {
                mothershipSpeedLevel++;
                Mothership.Instance.UpgradeSpeed(mothershipSpeedStats.baseValue1);
                Debug.Log($"Upgraded Speed to Level {mothershipSpeedLevel}");
            }
        }

        public void BuyScraper()
        {
            float cost = scraperStats.CalculateCost(scraperCount);
            if (ResourceManager.Instance.TrySpendScrap(cost))
            {
                scraperCount++;
                FleetManager.Instance.AddScraper();
                Debug.Log($"Bought Scraper. Total: {scraperCount}");
            }
        }

        public void BuySentinel()
        {
            float cost = sentinelStats.CalculateCost(sentinelCount);
            if (ResourceManager.Instance.TrySpendScrap(cost))
            {
                sentinelCount++;
                FleetManager.Instance.AddSentinel();
                Debug.Log($"Bought Sentinel. Total: {sentinelCount}");
            }
        }
    }
}

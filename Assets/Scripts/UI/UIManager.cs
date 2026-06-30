using UnityEngine;
using TMPro;
using StarScrape.Core;
using StarScrape.Upgrades;

namespace StarScrape.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Resource Texts")]
        [SerializeField] private TextMeshProUGUI scrapText;
        [SerializeField] private TextMeshProUGUI plasmaText;
        [SerializeField] private TextMeshProUGUI darkMatterText;

        [Header("Upgrade Texts (Optional Data Hooks)")]
        [SerializeField] private TextMeshProUGUI speedUpgradeCostText;
        [SerializeField] private TextMeshProUGUI scraperCostText;
        [SerializeField] private TextMeshProUGUI sentinelCostText;

        private void Start()
        {
            // Subscribe to resource events
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnScrapChanged += UpdateScrapText;
                ResourceManager.Instance.OnPlasmaChanged += UpdatePlasmaText;
                ResourceManager.Instance.OnDarkMatterChanged += UpdateDarkMatterText;

                // Initial UI set
                UpdateScrapText(ResourceManager.Instance.Scrap);
                UpdatePlasmaText(ResourceManager.Instance.Plasma);
                UpdateDarkMatterText(ResourceManager.Instance.DarkMatter);
            }
            
            UpdateUpgradeCostsUI();
        }

        private void OnDestroy()
        {
            // Unsubscribe to prevent memory leaks
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnScrapChanged -= UpdateScrapText;
                ResourceManager.Instance.OnPlasmaChanged -= UpdatePlasmaText;
                ResourceManager.Instance.OnDarkMatterChanged -= UpdateDarkMatterText;
            }
        }

        private void UpdateScrapText(float amount)
        {
            if (scrapText != null) scrapText.text = $"Scrap: {Mathf.FloorToInt(amount)}";
            UpdateUpgradeCostsUI(); // Costs might be affordable now
        }

        private void UpdatePlasmaText(float amount)
        {
            if (plasmaText != null) plasmaText.text = $"Plasma: {Mathf.FloorToInt(amount)}";
        }

        private void UpdateDarkMatterText(float amount)
        {
            if (darkMatterText != null) darkMatterText.text = $"Dark Matter: {Mathf.FloorToInt(amount)}";
        }

        // Methods to link to UI Buttons
        public void OnClickUpgradeSpeed()
        {
            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.UpgradeMothershipSpeed();
                UpdateUpgradeCostsUI();
            }
        }

        public void OnClickBuyScraper()
        {
            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.BuyScraper();
                UpdateUpgradeCostsUI();
            }
        }

        public void OnClickBuySentinel()
        {
            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.BuySentinel();
                UpdateUpgradeCostsUI();
            }
        }

        private void UpdateUpgradeCostsUI()
        {
            if (UpgradeManager.Instance == null) return;

            if (speedUpgradeCostText != null)
            {
                speedUpgradeCostText.text = $"Cost: {Mathf.FloorToInt(UpgradeManager.Instance.mothershipSpeedStats.CalculateCost(UpgradeManager.Instance.mothershipSpeedLevel))}";
            }
            if (scraperCostText != null)
            {
                scraperCostText.text = $"Cost: {Mathf.FloorToInt(UpgradeManager.Instance.scraperStats.CalculateCost(UpgradeManager.Instance.scraperCount))}";
            }
            if (sentinelCostText != null)
            {
                sentinelCostText.text = $"Cost: {Mathf.FloorToInt(UpgradeManager.Instance.sentinelStats.CalculateCost(UpgradeManager.Instance.sentinelCount))}";
            }
        }
    }
}

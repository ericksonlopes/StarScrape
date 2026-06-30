using System;
using UnityEngine;

namespace StarScrape.Core
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        // Current Run Resources
        public float Scrap { get; private set; }
        public float Plasma { get; private set; }
        
        // Permanent Resources
        public float DarkMatter { get; private set; }

        // Events for UI Updates
        public event Action<float> OnScrapChanged;
        public event Action<float> OnPlasmaChanged;
        public event Action<float> OnDarkMatterChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddScrap(float amount)
        {
            Scrap += amount;
            OnScrapChanged?.Invoke(Scrap);
        }

        public bool TrySpendScrap(float amount)
        {
            if (Scrap >= amount)
            {
                Scrap -= amount;
                OnScrapChanged?.Invoke(Scrap);
                return true;
            }
            return false;
        }

        public void AddPlasma(float amount)
        {
            Plasma += amount;
            OnPlasmaChanged?.Invoke(Plasma);
        }

        public bool TrySpendPlasma(float amount)
        {
            if (Plasma >= amount)
            {
                Plasma -= amount;
                OnPlasmaChanged?.Invoke(Plasma);
                return true;
            }
            return false;
        }

        public void AddDarkMatter(float amount)
        {
            DarkMatter += amount;
            OnDarkMatterChanged?.Invoke(DarkMatter);
        }
        
        public bool TrySpendDarkMatter(float amount)
        {
            if (DarkMatter >= amount)
            {
                DarkMatter -= amount;
                OnDarkMatterChanged?.Invoke(DarkMatter);
                return true;
            }
            return false;
        }

        public void ResetRunResources()
        {
            Scrap = 0;
            Plasma = 0;
            OnScrapChanged?.Invoke(Scrap);
            OnPlasmaChanged?.Invoke(Plasma);
        }
    }
}

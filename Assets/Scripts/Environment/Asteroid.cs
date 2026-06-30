using UnityEngine;
using StarScrape.Ships;
using StarScrape.Core;

namespace StarScrape.Environment
{
    public class Asteroid : SpaceObject
    {
        [Header("Asteroid Stats")]
        [SerializeField] private float maxHealth = 20f;
        [SerializeField] private float collisionDamage = 30f;
        [SerializeField] private int scrapToDrop = 3;

        private float currentHealth;

        private void OnEnable()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                DestroyAsteroid();
            }
        }

        private void DestroyAsteroid()
        {
            // Drop scrap
            if (ObjectPoolManager.Instance != null)
            {
                for (int i = 0; i < scrapToDrop; i++)
                {
                    // Espalha a sucata um pouquinho
                    Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                    ObjectPoolManager.Instance.GetObject("Scrap", transform.position + offset, Quaternion.identity);
                }
            }

            Despawn();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Se bateu na Nave Mãe
            Mothership mothership = other.GetComponent<Mothership>();
            if (mothership != null)
            {
                mothership.TakeDamage(collisionDamage);
                Despawn();
            }
        }
    }
}

using UnityEngine;
using StarScrape.Ships;
using StarScrape.Core;

namespace StarScrape.Environment
{
    public class SpawnerManager : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [Tooltip("Distância à frente da nave para nascer os objetos.")]
        [SerializeField] private float spawnDistance = 15f; 
        [Tooltip("O quão espalhado para os lados eles podem nascer.")]
        [SerializeField] private float lateralSpread = 8f;

        [Header("Asteroids")]
        [SerializeField] private string asteroidPoolName = "Asteroid";
        [SerializeField] private float baseAsteroidSpawnRate = 2f; // Segundos entre spawns na velocidade base

        [Header("Passive Scrap")]
        [SerializeField] private string scrapPoolName = "Scrap";
        [SerializeField] private float baseScrapSpawnRate = 1f;

        private float asteroidTimer;
        private float scrapTimer;

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;
            if (Mothership.Instance == null || ObjectPoolManager.Instance == null) return;

            // A velocidade da frota funciona como um multiplicador. Mais rápido = menos tempo entre spawns.
            // Assumindo que a velocidade base é 10.
            float speedMultiplier = Mothership.Instance.CurrentSpeed / 10f; 
            
            // Prevenindo divisão por zero ou velocidade negativa
            speedMultiplier = Mathf.Max(0.1f, speedMultiplier);

            float currentAsteroidRate = baseAsteroidSpawnRate / speedMultiplier;
            float currentScrapRate = baseScrapSpawnRate / speedMultiplier;

            asteroidTimer += Time.deltaTime;
            scrapTimer += Time.deltaTime;

            if (asteroidTimer >= currentAsteroidRate)
            {
                SpawnObject(asteroidPoolName);
                asteroidTimer = 0f;
            }

            if (scrapTimer >= currentScrapRate)
            {
                SpawnObject(scrapPoolName);
                scrapTimer = 0f;
            }
        }

        private void SpawnObject(string poolName)
        {
            if (Camera.main == null || Mothership.Instance == null) return;
            
            // A direção para a qual a nave está voando
            Vector3 forward = Mothership.Instance.transform.up;
            Vector3 right = Mothership.Instance.transform.right;
            
            // Posição base (uma distância à frente da CÂMERA, na direção da NAVE)
            Vector3 spawnCenter = Camera.main.transform.position + forward * spawnDistance;
            
            // Aleatoriza para os lados (usando o vetor Right da nave)
            float randomLateral = Random.Range(-lateralSpread, lateralSpread);
            Vector3 spawnPosition = spawnCenter + right * randomLateral;
            
            ObjectPoolManager.Instance.GetObject(poolName, spawnPosition, Quaternion.identity);
        }
    }
}

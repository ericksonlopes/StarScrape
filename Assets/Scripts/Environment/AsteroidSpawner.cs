using System.Collections.Generic;
using UnityEngine;
using StarScrape.Ships;

namespace StarScrape.Environment
{
    /// <summary>
    /// Gera asteroides na frente da Nave Mãe em posições aleatórias.
    /// Você só precisa arrastar as sprites dos meteoros no Inspector.
    /// O spawner cria os GameObjects automaticamente (sem prefab manual).
    /// </summary>
    public class AsteroidSpawner : MonoBehaviour
    {
        [Header("Sprites dos Asteroides")]
        [Tooltip("Arraste aqui as sprites de asteroide do spritesheet.")]
        [SerializeField] private Sprite[] asteroidSprites;

        [Header("Spawn Settings")]
        [Tooltip("Distância à frente da Nave Mãe para gerar asteroides.")]
        [SerializeField] private float spawnDistance = 9f;

        [Tooltip("Largura da zona de spawn (espalha horizontalmente).")]
        [SerializeField] private float spawnWidth = 12f;

        [Tooltip("Intervalo em segundos entre cada spawn.")]
        [SerializeField] private float spawnInterval = 1.5f;

        [Tooltip("Máximo de asteroides ativos no espaço ao mesmo tempo.")]
        [SerializeField] private int maxAsteroids = 15;

        [Header("Asteroid Properties")]
        [Tooltip("Velocidade de rotação aleatória dos asteroides.")]
        [SerializeField] private float maxRotationSpeed = 60f;

        [Tooltip("Escala mínima do asteroide.")]
        [SerializeField] private float minScale = 0.4f;

        [Tooltip("Escala máxima do asteroide.")]
        [SerializeField] private float maxScale = 1.5f;

        private float spawnTimer;
        private List<GameObject> activeAsteroids = new List<GameObject>();

        private void Update()
        {
            if (Mothership.Instance == null) return;
            if (asteroidSprites == null || asteroidSprites.Length == 0) return;

            // Limpa referências de asteroides já destruídos
            activeAsteroids.RemoveAll(a => a == null);

            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval && activeAsteroids.Count < maxAsteroids)
            {
                SpawnAsteroid();
                spawnTimer = 0f;
            }
        }

        private void SpawnAsteroid()
        {
            Transform mothership = Mothership.Instance.transform;

            // Posição: na frente da nave mãe + offset lateral aleatório
            Vector3 forwardPos = mothership.position + mothership.up * spawnDistance;
            Vector3 lateralOffset = mothership.right * Random.Range(-spawnWidth / 2f, spawnWidth / 2f);
            Vector3 spawnPos = forwardPos + lateralOffset;

            // Cria o GameObject do asteroide por código
            GameObject asteroidObj = new GameObject("Asteroid");
            asteroidObj.transform.position = spawnPos;

            // Rotação inicial aleatória
            asteroidObj.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

            // Escala aleatória
            float scale = Random.Range(minScale, maxScale);
            asteroidObj.transform.localScale = Vector3.one * scale;

            // Sprite aleatória
            SpriteRenderer sr = asteroidObj.AddComponent<SpriteRenderer>();
            sr.sprite = asteroidSprites[Random.Range(0, asteroidSprites.Length)];
            sr.sortingOrder = 5; // Na frente do fundo

            // Collider 2D para interagir com naves
            CircleCollider2D col = asteroidObj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;

            // Rigidbody2D (necessário para o trigger funcionar)
            Rigidbody2D rb = asteroidObj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.linearDamping = 0f;

            // Adiciona o script de Asteroid (comportamento: vida, dano, drop de sucata)
            Asteroid asteroid = asteroidObj.AddComponent<Asteroid>();

            // Adiciona rotação visual contínua
            AsteroidSpin spin = asteroidObj.AddComponent<AsteroidSpin>();
            spin.SetSpeed(Random.Range(-maxRotationSpeed, maxRotationSpeed));

            activeAsteroids.Add(asteroidObj);
        }
    }
}

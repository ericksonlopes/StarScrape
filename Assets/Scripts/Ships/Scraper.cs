using UnityEngine;
using StarScrape.Core;
using StarScrape.Environment;

namespace StarScrape.Ships
{
    public class Scraper : FleetShip
    {
        [Header("Scraping Stats")]
        [Tooltip("Raio de detecção de asteroides/recursos.")]
        [SerializeField] private float detectionRadius = 3f;

        [Tooltip("Distância para 'tocar' o asteroide e começar a minerar.")]
        [SerializeField] private float miningDistance = 1f;

        [Tooltip("Dano por segundo ao minerar um asteroide.")]
        [SerializeField] private float miningDamagePerSecond = 10f;

        [Tooltip("Distância máxima da Nave Mãe antes de abandonar a missão e voltar.")]
        [SerializeField] private float maxDistanceFromMothership = 4f;

        [Tooltip("Tempo de espera entre buscas por novos alvos.")]
        [SerializeField] private float scanInterval = 0.5f;

        private float scanTimer;
        private Asteroid currentTarget;
        private bool isReturningWithCargo = false;

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            scanTimer += Time.deltaTime;

            if (currentTarget != null)
            {
                // Verifica se o alvo ainda existe
                if (currentTarget == null || currentTarget.gameObject == null)
                {
                    AbandonMission();
                    return;
                }

                // Verifica se ficou longe demais da Nave Mãe (abandonar e voltar)
                if (Mothership.Instance != null)
                {
                    float distToMother = Vector3.Distance(transform.position, Mothership.Instance.transform.position);
                    if (distToMother > maxDistanceFromMothership)
                    {
                        AbandonMission();
                        return;
                    }
                }

                // Se está perto o suficiente do asteroide, minera!
                float distToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
                if (distToTarget <= miningDistance)
                {
                    currentTarget.TakeDamage(miningDamagePerSecond * Time.deltaTime);
                }
            }
            else
            {
                if (isReturningWithCargo)
                {
                    // Checa se já chegou de volta na sua posição da formação
                    if (Mothership.Instance != null)
                    {
                        Vector3 worldOffset = Mothership.Instance.transform.rotation * GetFormationOffset();
                        Vector3 idealPos = Mothership.Instance.transform.position + worldOffset;
                        
                        // Se chegou perto o suficiente da formação, "deposita" a carga e volta ao trabalho
                        if (Vector3.Distance(transform.position, idealPos) < 1.5f)
                        {
                            isReturningWithCargo = false;
                            scanTimer = 0f; // Dá um tempinho antes de escanear de novo
                        }
                    }
                }
                else
                {
                    // Sem alvo e não está retornando: procura um novo asteroide periodicamente
                    if (scanTimer >= scanInterval)
                    {
                        scanTimer = 0f;
                        SearchForAsteroid();
                    }
                }
            }
        }

        private void SearchForAsteroid()
        {
            // Procura todos os asteroides próximos
            Asteroid nearest = null;
            float nearestDist = float.MaxValue;

            // Busca todos os Asteroids ativos no cenário
            Asteroid[] allAsteroids = FindObjectsByType<Asteroid>(FindObjectsInactive.Exclude);

            foreach (var asteroid in allAsteroids)
            {
                if (asteroid == null || asteroid.gameObject == null) continue;

                // Verifica se já não tem outro Scraper perseguindo este asteroide
                bool alreadyTargeted = false;
                foreach (var ship in AllShips)
                {
                    if (ship != this && ship is Scraper otherScraper && otherScraper.currentTarget == asteroid)
                    {
                        alreadyTargeted = true;
                        break;
                    }
                }
                if (alreadyTargeted) continue;

                float dist = Vector3.Distance(transform.position, asteroid.transform.position);
                if (dist <= detectionRadius && dist < nearestDist)
                {
                    nearest = asteroid;
                    nearestDist = dist;
                }
            }

            if (nearest != null)
            {
                currentTarget = nearest;
                SetMissionTarget(nearest.transform);
            }
        }

        private void AbandonMission()
        {
            currentTarget = null;
            isReturningWithCargo = true; // Força a voltar para a base antes de pegar outro alvo
            ClearMission(); // Volta para a formação
        }

        private void OnDrawGizmosSelected()
        {
            // Mostra o raio de detecção no editor
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            // Mostra o raio de mineração
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, miningDistance);
        }
    }
}

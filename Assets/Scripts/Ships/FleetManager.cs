using System.Collections.Generic;
using UnityEngine;

namespace StarScrape.Ships
{
    /// <summary>
    /// Gerencia a frota inteira da Nave Mãe.
    /// 
    /// REGRAS DE FORMAÇÃO:
    /// 1. Sentinelas SEMPRE ficam na linha da frente (proteção).
    /// 2. Coletores ficam atrás dos Sentinelas, mas ainda na frente da Nave Mãe.
    /// 3. NENHUMA nave pode ficar atrás da Nave Mãe. O arco é limitado a 120 graus.
    /// 4. Se muitas naves são criadas, elas formam fileiras extras para a frente.
    /// </summary>
    public class FleetManager : MonoBehaviour
    {
        public static FleetManager Instance { get; private set; }

        [Header("Fleet Prefabs")]
        [SerializeField] private GameObject scraperPrefab;
        [SerializeField] private GameObject sentinelPrefab;

        [Header("Formation Settings")]
        [SerializeField] private Transform mothershipTransform;

        [Tooltip("Distância dos Coletores à Nave Mãe (ficam mais perto, atrás dos Sentinelas).")]
        [SerializeField] private float scraperDistance = 2.0f;

        [Tooltip("Distância dos Sentinelas à Nave Mãe (ficam mais longe, na linha de frente).")]
        [SerializeField] private float sentinelDistance = 4.0f;


        private List<Scraper> activeScrapers = new List<Scraper>();
        private List<Sentinel> activeSentinels = new List<Sentinel>();

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

        private void Update()
        {
            // Atalhos para testes rápidos
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                if (UnityEngine.InputSystem.Keyboard.current.digit1Key.wasPressedThisFrame) AddScraper();
                if (UnityEngine.InputSystem.Keyboard.current.digit2Key.wasPressedThisFrame) AddSentinel();
            }
        }

        public void AddScraper()
        {
            if (mothershipTransform == null) return;
            GameObject obj = Instantiate(scraperPrefab);
            obj.transform.position = mothershipTransform.position;
            Scraper scraper = obj.GetComponent<Scraper>();
            if (scraper != null)
            {
                activeScrapers.Add(scraper);
                RefreshAllFormations();
                // Teleporta direto para a posição da formação (evita empilhamento)
                TeleportToFormation(scraper);
            }
        }

        public void AddSentinel()
        {
            if (mothershipTransform == null) return;
            GameObject obj = Instantiate(sentinelPrefab);
            obj.transform.position = mothershipTransform.position;
            Sentinel sentinel = obj.GetComponent<Sentinel>();
            if (sentinel != null)
            {
                activeSentinels.Add(sentinel);
                RefreshAllFormations();
                // Teleporta direto para a posição da formação (evita empilhamento)
                TeleportToFormation(sentinel);
            }
        }

        /// <summary>
        /// Recalcula a posição de TODAS as naves da frota.
        /// Coletores: arco perto da Nave Mãe. Sentinelas: arco na frente de tudo.
        /// </summary>
        private void RefreshAllFormations()
        {
            // Coletores formam um arco perto da Nave Mãe
            PlaceArc(activeScrapers.ConvertAll(x => x.transform), scraperDistance);

            // REGRA ABSOLUTA: Sentinelas ficam NO MÍNIMO 2 metros à frente dos Coletores.
            // Não importa o que estiver no Inspector: se scraperDistance = 2, sentinelas = 4 (no mínimo).
            float forcedSentinelDistance = Mathf.Max(sentinelDistance, scraperDistance + 2.0f);
            PlaceArc(activeSentinels.ConvertAll(x => x.transform), forcedSentinelDistance);
        }

        /// <summary>
        /// Distribui TODAS as naves em um único arco contínuo na frente da Nave Mãe.
        /// O arco nunca ultrapassa 120 graus. A distância é fixa.
        /// </summary>
        private void PlaceArc(List<Transform> ships, float distance)
        {
            if (ships.Count == 0 || mothershipTransform == null) return;

            // ---- ARCO LIMITADO A 120 GRAUS ----
            // Quanto mais naves, mais o arco abre (20° por nave), mas trava em 120°.
            // Isso garante que nenhuma nave fica atrás da Nave Mãe.
            float spreadAngle;
            if (ships.Count <= 1)
            {
                spreadAngle = 0f;
            }
            else
            {
                spreadAngle = Mathf.Min((ships.Count - 1) * 20f, 120f);
            }

            float startAngle = -spreadAngle / 2f;
            float angleStep = ships.Count > 1 ? spreadAngle / (ships.Count - 1) : 0f;

            for (int i = 0; i < ships.Count; i++)
            {
                float angle = ships.Count == 1 ? 0f : startAngle + (i * angleStep);

                Vector3 direction = Quaternion.Euler(0, 0, -angle) * Vector3.up;
                Vector3 offset = direction * distance;

                FleetShip fleetShip = ships[i].GetComponent<FleetShip>();
                if (fleetShip != null)
                {
                    fleetShip.SetFormationOffset(offset);
                }
            }
        }
        /// <summary>
        /// Teleporta uma nave recém-criada diretamente para sua posição de formação.
        /// Isso evita que ela nasça empilhada na Nave Mãe e cause colisões.
        /// </summary>
        private void TeleportToFormation(FleetShip ship)
        {
            if (ship == null || mothershipTransform == null) return;
            Vector3 worldPos = mothershipTransform.position + mothershipTransform.rotation * ship.GetFormationOffset();
            ship.transform.position = worldPos;
        }
    }
}

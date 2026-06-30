using UnityEngine;

namespace StarScrape.Ships
{
    public class FleetShip : MonoBehaviour
    {
        [Header("Swarm Movement")]
        [Tooltip("Suavidade ao seguir o ponto alvo da formação.")]
        [SerializeField] private float smoothTime = 0.8f;
        [Tooltip("Velocidade do 'balanço' independente da nave.")]
        [SerializeField] private float wanderSpeed = 2f;
        [Tooltip("Tamanho do raio do balanço independente.")]
        [SerializeField] private float wanderRadius = 0.3f; // Reduzido para não invadirem o espaço uma da outra
        
        private Vector3 formationOffset;
        private Vector3 velocity = Vector3.zero;
        private Vector2 randomOffsetSeed;

        public static System.Collections.Generic.List<FleetShip> AllShips = new System.Collections.Generic.List<FleetShip>();

        protected virtual void Start()
        {
            AllShips.Add(this);
            // Dá a cada nave um "balanço" único, para não voarem sincronizadas como robôs
            randomOffsetSeed = new Vector2(Random.Range(0f, 100f), Random.Range(0f, 100f));
        }

        protected virtual void OnDestroy()
        {
            AllShips.Remove(this);
        }

        public void SetFormationOffset(Vector3 newOffset)
        {
            formationOffset = newOffset;
        }

        public Vector3 GetFormationOffset()
        {
            return formationOffset;
        }

        // --- Sistema de Missão ---
        // Se a nave tem um alvo de missão (ex: asteroide), ela voa até lá em vez de ficar na formação.
        protected Transform missionTarget;
        [SerializeField] protected float missionSpeed = 3.0f; // Quanto maior, BEM MAIS LENTA a nave fica na missão

        public bool IsOnMission => missionTarget != null;

        public void SetMissionTarget(Transform target)
        {
            missionTarget = target;
        }

        public void ClearMission()
        {
            missionTarget = null;
        }

        protected virtual void Update()
        {
            if (Mothership.Instance == null) return;

            Vector3 targetWorldPos;

            if (missionTarget != null)
            {
                // EM MISSÃO: voa até o alvo (asteroide, sucata, etc.)
                targetWorldPos = missionTarget.position;
                transform.position = Vector3.SmoothDamp(transform.position, targetWorldPos, ref velocity, missionSpeed);
                
                // Olha para o alvo enquanto está em missão (suavemente)
                Vector3 dir = (missionTarget.position - transform.position).normalized;
                if (dir.sqrMagnitude > 0.001f)
                {
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
                    Quaternion targetRot = Quaternion.Euler(0, 0, angle);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 1.5f * Time.deltaTime);
                }
            }
            else
            {
                // EM FORMAÇÃO: segue a posição da frota com balanço orgânico
                float noiseX = (Mathf.PerlinNoise(Time.time * wanderSpeed + randomOffsetSeed.x, 0) - 0.5f) * 2f;
                float noiseY = (Mathf.PerlinNoise(0, Time.time * wanderSpeed + randomOffsetSeed.y) - 0.5f) * 2f;
                Vector3 wanderOffset = new Vector3(noiseX, noiseY, 0) * wanderRadius;

                Vector3 worldOffset = Mothership.Instance.transform.rotation * formationOffset;
                targetWorldPos = Mothership.Instance.transform.position + worldOffset + wanderOffset;

                transform.position = Vector3.SmoothDamp(transform.position, targetWorldPos, ref velocity, smoothTime);

                // Volta suavemente a olhar para a mesma direção da Nave Mãe
                transform.rotation = Quaternion.Slerp(transform.rotation, Mothership.Instance.transform.rotation, 2.5f * Time.deltaTime);
            }
            
            // --- Repulsão da Nave Mãe (suave) ---
            float minSafeDistance = 1.0f;
            Vector3 diff = transform.position - Mothership.Instance.transform.position;
            if (diff.sqrMagnitude < minSafeDistance * minSafeDistance && diff.sqrMagnitude > 0.001f)
            {
                Vector3 pushDir = diff.normalized;
                transform.position += pushDir * 2f * Time.deltaTime;
            }

            // --- Repulsão entre naves (suave) ---
            float shipSafeDistance = 0.5f;
            foreach (var otherShip in AllShips)
            {
                if (otherShip != this && otherShip != null)
                {
                    Vector3 diffToOther = transform.position - otherShip.transform.position;
                    float sqrDist = diffToOther.sqrMagnitude;
                    if (sqrDist < shipSafeDistance * shipSafeDistance && sqrDist > 0.001f)
                    {
                        Vector3 pushDir = diffToOther.normalized;
                        transform.position += pushDir * 1.5f * Time.deltaTime;
                    }
                }
            }
        }
    }
}

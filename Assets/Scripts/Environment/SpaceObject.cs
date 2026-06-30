using UnityEngine;

namespace StarScrape.Environment
{
    public class SpaceObject : MonoBehaviour
    {
        [Header("Space Object Settings")]
        [Tooltip("Multiplicador da velocidade do scroll para este objeto. 1 = mesma velocidade da frota.")]
        [SerializeField] protected float scrollMultiplier = 1f;
        [SerializeField] protected float despawnDistance = 25f; // Ponto fora da tela em qualquer direção para sumir

        protected virtual void Update()
        {
            if (ScrollManager.Instance == null) return;

            // Move (o vetor já vem com a direção e velocidade corretas do ScrollManager)
            Vector3 movement = ScrollManager.Instance.GetScrollVector() * scrollMultiplier * Time.deltaTime;
            transform.position += movement;

            // Checa se ficou muito longe da câmera
            Vector3 camPos = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
            if (Vector3.Distance(transform.position, camPos) > despawnDistance)
            {
                Despawn();
                return;
            }

            // Despawn rápido: se o objeto ficou para TRÁS da Nave Mãe, pode sumir logo.
            if (Ships.Mothership.Instance != null)
            {
                Vector3 localPos = Ships.Mothership.Instance.transform.InverseTransformPoint(transform.position);
                // Se o Y local for menor que -5, ele já passou da Nave Mãe e ficou pra trás
                if (localPos.y < -5f)
                {
                    Despawn();
                }
            }
        }

        protected virtual void Despawn()
        {
            // Tenta devolver para o pool, se não existir um pool com esse nome (ou se não soubermos), apenas destroi
            // Idealmente, a classe que instanciou sabe o pool. Para simplificar, o SpawnerManager fará isso.
            Destroy(gameObject); 
        }
    }
}

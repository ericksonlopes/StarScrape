using UnityEngine;

namespace StarScrape.Environment
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ParallaxBackground : MonoBehaviour
    {
        [Header("Parallax Settings")]
        [Tooltip("Multiplicador da velocidade do scroll. Fundo mais distante deve ter valor menor (ex: 0.2).")]
        [SerializeField] private float parallaxEffectMultiplier = 0.5f;
        
        private float lengthX;
        private float lengthY;
        private Vector3 startPosition;

        private Transform cam;
        private Vector3 lastCameraPosition;

        private void Start()
        {
            startPosition = transform.position;
            lengthY = GetComponent<SpriteRenderer>().bounds.size.y;
            lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
            
            if (Camera.main != null)
            {
                cam = Camera.main.transform;
                lastCameraPosition = cam.position;
            }

            // Cria uma grade 3x3 de clones para cobrir voo em qualquer direção (360 graus)
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue; // Pula a própria imagem central
                    CreateClone(x * lengthX, y * lengthY);
                }
            }
        }

        private void CreateClone(float offsetX, float offsetY)
        {
            GameObject clone = new GameObject($"Background_Clone_{offsetX}_{offsetY}");
            clone.transform.parent = transform;
            clone.transform.localPosition = new Vector3(offsetX, offsetY, 0); // Fica exatamente grudado
            
            SpriteRenderer cloneRenderer = clone.AddComponent<SpriteRenderer>();
            SpriteRenderer myRenderer = GetComponent<SpriteRenderer>();
            cloneRenderer.sprite = myRenderer.sprite;
            cloneRenderer.color = myRenderer.color;
            cloneRenderer.sortingLayerID = myRenderer.sortingLayerID;
            cloneRenderer.sortingOrder = myRenderer.sortingOrder;
        }

        private void LateUpdate()
        {
            if (ScrollManager.Instance == null || cam == null) return;

            // 1. Calcula o quanto o jogo deveria ter rolado automaticamente (agora é um Vector3 em qualquer direção)
            Vector3 scrollDir = ScrollManager.Instance.GetScrollVector();
            
            // O auto-scroll também afeta a câmera simulada (mesmo princípio, mas 3D)
            Vector3 deltaCamera = cam.position - lastCameraPosition;
            
            // Aplica parallax na movimentação da câmera
            transform.position += deltaCamera * parallaxEffectMultiplier;
            
            // Aplica o movimento do auto-scroll constante
            transform.position += scrollDir * Time.deltaTime;

            lastCameraPosition = cam.position;

            // 2. Loop Infinito 360 Graus
            float distanceToCameraX = cam.position.x - transform.position.x;
            float distanceToCameraY = cam.position.y - transform.position.y;

            if (distanceToCameraX > lengthX)
            {
                transform.position += new Vector3(lengthX, 0, 0);
            }
            else if (distanceToCameraX < -lengthX)
            {
                transform.position -= new Vector3(lengthX, 0, 0);
            }

            if (distanceToCameraY > lengthY)
            {
                transform.position += new Vector3(0, lengthY, 0);
            }
            else if (distanceToCameraY < -lengthY)
            {
                transform.position -= new Vector3(0, lengthY, 0);
            }
        }
    }
}

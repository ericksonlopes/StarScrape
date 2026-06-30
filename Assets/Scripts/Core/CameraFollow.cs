using UnityEngine;
using StarScrape.Ships;

namespace StarScrape.Core
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Follow Settings")]
        [Tooltip("A velocidade com que a câmera persegue a nave. Menor = mais atraso (mais suave).")]
        [SerializeField] private float smoothTime = 0.3f;
        
        [Tooltip("A distância que a câmera fica 'à frente' da nave. Ajusta a visão independente de para onde você voa.")]
        [SerializeField] private float lookAheadDistance = 3f;

        private Vector3 velocity = Vector3.zero;

        private void LateUpdate()
        {
            if (Mothership.Instance != null)
            {
                // A posição alvo da câmera é a posição da nave + um avanço na direção que ela aponta
                Vector3 forwardOffset = Mothership.Instance.transform.up * lookAheadDistance;
                Vector3 targetPosition = Mothership.Instance.transform.position + forwardOffset + new Vector3(0, 0, -10f);

                // Move a câmera suavemente para a posição alvo
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            }
        }
    }
}

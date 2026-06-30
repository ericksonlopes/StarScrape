using UnityEngine;

namespace StarScrape.Environment
{
    /// <summary>
    /// Componente simples que faz um objeto girar continuamente.
    /// Usado nos asteroides para dar vida ao visual.
    /// </summary>
    public class AsteroidSpin : MonoBehaviour
    {
        private float speed;

        public void SetSpeed(float rotationSpeed)
        {
            speed = rotationSpeed;
        }

        private void Update()
        {
            transform.Rotate(0, 0, speed * Time.deltaTime);
        }
    }
}

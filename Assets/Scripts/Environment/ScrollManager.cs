using UnityEngine;
using StarScrape.Ships;

namespace StarScrape.Environment
{
    public class ScrollManager : MonoBehaviour
    {
        public static ScrollManager Instance { get; private set; }

        [Header("Scroll Settings")]
        [SerializeField] private float globalSpeedMultiplier = 1f;

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

        public float GetScrollSpeed()
        {
            if (Mothership.Instance != null)
            {
                return Mothership.Instance.CurrentSpeed * globalSpeedMultiplier;
            }
            return 0f;
        }

        public Vector3 GetScrollVector()
        {
            // O ambiente se move na direção oposta à qual a nave está apontando
            if (Mothership.Instance != null)
            {
                return -Mothership.Instance.transform.up * GetScrollSpeed();
            }
            return Vector3.zero;
        }
    }
}

using UnityEngine;
using StarScrape.Core;

namespace StarScrape.Ships
{
    public class Sentinel : FleetShip
    {
        [Header("Combat Stats")]
        [SerializeField] private float attackRange = 8f;
        [SerializeField] private float fireRate = 1f; // shots per second
        [SerializeField] private string projectilePoolName = "Laser";
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private Transform firePoint;

        private float fireTimer = 0f;

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
            fireTimer += Time.deltaTime;

            if (fireTimer >= 1f / fireRate)
            {
                Transform target = FindNearestTarget();
                if (target != null)
                {
                    FireAt(target);
                    fireTimer = 0f;
                }
            }
        }

        private Transform FindNearestTarget()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, obstacleLayer);
            
            Transform nearest = null;
            float minDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearest = hit.transform;
                }
            }

            return nearest;
        }

        private void FireAt(Transform target)
        {
            if (ObjectPoolManager.Instance != null)
            {
                Vector3 direction = (target.position - firePoint.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                
                GameObject proj = ObjectPoolManager.Instance.GetObject(projectilePoolName, firePoint.position, rotation);
                
                // Assuming the projectile has a script to move it forward
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace StarScrape.Core
{
    public class ObjectPoolManager : MonoBehaviour
    {
        public static ObjectPoolManager Instance { get; private set; }

        [System.Serializable]
        public class PoolSetup
        {
            public string poolName;
            public GameObject prefab;
            public int defaultCapacity = 20;
            public int maxSize = 100;
        }

        [SerializeField] private List<PoolSetup> initialPools = new List<PoolSetup>();
        
        private Dictionary<string, IObjectPool<GameObject>> pools = new Dictionary<string, IObjectPool<GameObject>>();
        private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializePools();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializePools()
        {
            foreach (var setup in initialPools)
            {
                CreatePool(setup.poolName, setup.prefab, setup.defaultCapacity, setup.maxSize);
            }
        }

        public void CreatePool(string poolName, GameObject prefab, int defaultCapacity = 20, int maxSize = 100)
        {
            if (pools.ContainsKey(poolName)) return;

            prefabs[poolName] = prefab;
            
            var pool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(prefabs[poolName]),
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: true,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );

            pools.Add(poolName, pool);
        }

        public GameObject GetObject(string poolName, Vector3 position, Quaternion rotation)
        {
            if (pools.TryGetValue(poolName, out var pool))
            {
                GameObject obj = pool.Get();
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                return obj;
            }
            Debug.LogWarning($"Pool {poolName} does not exist!");
            return null;
        }

        public void ReturnObject(string poolName, GameObject obj)
        {
            if (pools.TryGetValue(poolName, out var pool))
            {
                pool.Release(obj);
            }
        }
    }
}

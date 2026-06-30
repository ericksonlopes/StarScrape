using UnityEngine;
using StarScrape.Core;

namespace StarScrape.Environment
{
    // A Sucata herda de SpaceObject para se mover e sumir com o tempo
    public class Scrap : SpaceObject
    {
        [Header("Scrap Value")]
        [Tooltip("Quanto de recurso essa sucata vale ao ser coletada.")]
        [SerializeField] private float scrapValue = 1f;

        public float Collect()
        {
            // Toca um som de coleta, partícula, etc.
            Despawn();
            return scrapValue;
        }
    }
}

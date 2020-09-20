using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter
{
    public sealed class AnchorPool : ObjectPool<AnchorPool, AnchorObject, Coordinate>
    {
        private static readonly Dictionary<GameObject, AnchorPool> PoolInstances = new Dictionary<GameObject, AnchorPool>();

        private void Awake()
        {
            if (Prefab != null && !PoolInstances.ContainsKey(Prefab))
                PoolInstances.Add(Prefab, this);
        }

        private void OnDestroy()
        {
            PoolInstances.Remove(Prefab);
        }

        public static AnchorPool GetObjectPool(GameObject prefab, int initialPoolCount = 10)
        {
            AnchorPool objPool = null;
            
            if (!PoolInstances.TryGetValue(prefab, out objPool))
            {
                var obj = new GameObject(prefab.name + "_Pool");
                objPool = obj.AddComponent<AnchorPool>();
                objPool.Prefab = prefab;
                objPool.InitialPoolCount = initialPoolCount;

                PoolInstances[prefab] = objPool;
            }

            return objPool;
        }
    }

    public sealed class AnchorObject : PoolObject<AnchorPool, AnchorObject, Coordinate>
    {
        private Transform _transform;
        private Anchor _anchor;

        public Anchor Anchor => _anchor;

        protected override void SetReferences()
        {
            _transform = Instance.transform;
            _anchor = Instance.GetComponent<Anchor>();
        }

        public override void WakeUp(Coordinate coordinate)
        {
            _transform.localPosition = coordinate.ToLocalPosition();
            _anchor.Coordinate = coordinate;
            Instance.SetActive(true);
        }

        public override void Sleep()
        {
            Instance.SetActive(false);
            _anchor.Coordinate = new Coordinate(-1, -1);
        }
    }
}
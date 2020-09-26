using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter
{
    public sealed class BubblePool : ObjectPool<BubblePool, BubbleObject, Vector2>
    {
        private static readonly Dictionary<GameObject, BubblePool> PoolInstances =
            new Dictionary<GameObject, BubblePool>();

        private void Awake()
        {
            if (Prefab != null && !PoolInstances.ContainsKey(Prefab))
                PoolInstances.Add(Prefab, this);
        }

        private void OnDestroy()
        {
            PoolInstances.Remove(Prefab);
        }

        public static BubblePool GetObjectPool(GameObject prefab, int initialPoolCount = 10)
        {
            BubblePool objPool = null;

            if (!PoolInstances.TryGetValue(prefab, out objPool))
            {
                var obj = new GameObject(prefab.name + "_Pool");
                objPool = obj.AddComponent<BubblePool>();
                objPool.Prefab = prefab;
                objPool.InitialPoolCount = initialPoolCount;

                PoolInstances[prefab] = objPool;
            }

            return objPool;
        }
    }

    public sealed class BubbleObject : PoolObject<BubblePool, BubbleObject, Vector2>
    {
        private Transform _transform;
        private Bubble _bubble;

        public Bubble Bubble => _bubble;


        protected override void SetReferences()
        {
            _transform = Instance.transform;
            _bubble = Instance.GetComponent<Bubble>();
            _bubble.BubbleObject = this;
        }

        public override void WakeUp(Vector2 localPosition)
        {
            _transform.localPosition = localPosition;
            Instance.SetActive(true);
        }

        public override void Sleep()
        {
            Instance.SetActive(false);
        }
    }
}
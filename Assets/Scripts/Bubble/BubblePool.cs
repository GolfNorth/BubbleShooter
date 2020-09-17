using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter
{
    public class BubblePool : ObjectPool<BubblePool, BubbleObject, Vector2>
    {
        protected static Dictionary<GameObject, BubblePool> PoolInstances = new Dictionary<GameObject, BubblePool>();

        private void Awake()
        {
            if(prefab != null && !PoolInstances.ContainsKey(prefab))
                PoolInstances.Add(prefab, this);
        }

        private void OnDestroy()
        {
            PoolInstances.Remove(prefab);
        }
        
        public static BubblePool GetObjectPool(GameObject prefab, int initialPoolCount = 10)
        {
            BubblePool objPool = null;
            if (!PoolInstances.TryGetValue(prefab, out objPool))
            {
                GameObject obj = new GameObject(prefab.name + "_Pool");
                objPool = obj.AddComponent<BubblePool>();
                objPool.prefab = prefab;
                objPool.initialPoolCount = initialPoolCount;

                PoolInstances[prefab] = objPool;
            }

            return objPool;
        }
    }

    public class BubbleObject : PoolObject<BubblePool, BubbleObject, Vector2>
    {
        public Transform transform;
        public Rigidbody2D rigidbody2D;
        public SpriteRenderer spriteRenderer;
        public Bubble bubble;

        protected override void SetReferences()
        {
            transform = instance.transform;
            rigidbody2D = instance.GetComponent<Rigidbody2D> ();
            spriteRenderer = instance.GetComponent<SpriteRenderer> ();
            bubble = instance.GetComponent<Bubble>();
            bubble.BubblePoolObject = this;
            bubble.MainCamera = Object.FindObjectOfType<Camera> ();
        }

        public override void WakeUp(Vector2 position)
        {
            transform.position = position;
            instance.SetActive(true);
        }

        public override void Sleep()
        {
            instance.SetActive(false);
        }
    }
}
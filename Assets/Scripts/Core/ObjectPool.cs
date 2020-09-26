using System;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter
{
    public abstract class ObjectPool<TPool, TObject, TInfo> : ObjectPool<TPool, TObject>
        where TPool : ObjectPool<TPool, TObject, TInfo>
        where TObject : PoolObject<TPool, TObject, TInfo>, new()
    {
        public virtual TObject Pop(TInfo info)
        {
            if (Pool.Count < InitialPoolCount)
                for (var i = 0; i < InitialPoolCount; i++)
                {
                    var newInitialPoolObject = CreateNewPoolObject();

                    Pool.Add(newInitialPoolObject);
                }

            for (var i = 0; i < Pool.Count; i++)
                if (Pool[i].InPool)
                {
                    Pool[i].InPool = false;
                    Pool[i].WakeUp(info);

                    return Pool[i];
                }

            var newPoolObject = CreateNewPoolObject();

            Pool.Add(newPoolObject);

            newPoolObject.InPool = false;
            newPoolObject.WakeUp(info);

            return newPoolObject;
        }
    }

    public abstract class ObjectPool<TPool, TObject> : MonoBehaviour
        where TPool : ObjectPool<TPool, TObject>
        where TObject : PoolObject<TPool, TObject>, new()
    {
        [SerializeField] private int _initialPoolCount = 10;

        [SerializeField] private GameObject _prefab;

        public int InitialPoolCount
        {
            get => _initialPoolCount;
            set => _initialPoolCount = value;
        }

        public List<TObject> Pool { get; set; } = new List<TObject>();

        public GameObject Prefab
        {
            get => _prefab;
            set => _prefab = value;
        }

        protected TObject CreateNewPoolObject()
        {
            var newPoolObject = new TObject();
            newPoolObject.Instance = Instantiate(_prefab);
            newPoolObject.Instance.transform.SetParent(transform);
            newPoolObject.InPool = true;
            newPoolObject.SetReferences(this as TPool);
            newPoolObject.Sleep();

            return newPoolObject;
        }

        public virtual TObject Pop()
        {
            if (Pool.Count < _initialPoolCount)
                for (var i = 0; i < _initialPoolCount; i++)
                {
                    var newInitialPoolObject = CreateNewPoolObject();

                    Pool.Add(newInitialPoolObject);
                }

            for (var i = 0; i < Pool.Count; i++)
                if (Pool[i].InPool)
                {
                    Pool[i].InPool = false;
                    Pool[i].WakeUp();
                    return Pool[i];
                }

            var newPoolObject = CreateNewPoolObject();

            Pool.Add(newPoolObject);

            newPoolObject.InPool = false;
            newPoolObject.WakeUp();

            return newPoolObject;
        }

        public virtual void Push(TObject poolObject)
        {
            poolObject.InPool = true;
            poolObject.Sleep();
        }
    }

    [Serializable]
    public abstract class PoolObject<TPool, TObject, TInfo> : PoolObject<TPool, TObject>
        where TPool : ObjectPool<TPool, TObject, TInfo>
        where TObject : PoolObject<TPool, TObject, TInfo>, new()
    {
        public virtual void WakeUp(TInfo info)
        {
        }
    }

    [Serializable]
    public abstract class PoolObject<TPool, TObject>
        where TPool : ObjectPool<TPool, TObject>
        where TObject : PoolObject<TPool, TObject>, new()
    {
        [SerializeField] private bool _inPool;
        [SerializeField] private GameObject _instance;
        [SerializeField] private TPool _objectPool;

        public bool InPool
        {
            get => _inPool;
            set => _inPool = value;
        }

        public GameObject Instance
        {
            get => _instance;
            set => _instance = value;
        }

        public TPool Pool
        {
            get => _objectPool;
            set => _objectPool = value;
        }

        public void SetReferences(TPool pool)
        {
            _objectPool = pool;
            SetReferences();
        }

        protected virtual void SetReferences()
        {
        }

        public virtual void WakeUp()
        {
        }

        public virtual void Sleep()
        {
        }

        public virtual void ReturnToPool()
        {
            var thisObject = this as TObject;
            _objectPool.Push(thisObject);
        }
    }
}
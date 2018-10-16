using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace WaterBlast.Game.Common
{
    public class ObjectPool : MonoBehaviour
    {
        public GameObject prefab = null;
        public int initSize = 0;

        private readonly Stack<GameObject> instances = new Stack<GameObject>();

        private void Awake()
        {
            Assert.IsNotNull(prefab);
        }

        private void Start()
        {
            for (int i = 0; i < initSize; ++i)
            {
                var obj = CreateInstans();
                obj.SetActive(false);
                instances.Push(obj);
            }
        }

        public GameObject GetObject()
        {
            return (instances.Count > 0) ? instances.Pop() : CreateInstans();
        }

        public void ReturnObject(GameObject obj)
        {
            PooledObject pooledObject = obj.GetComponent<PooledObject>();
            Assert.IsNotNull(pooledObject);
            Assert.IsTrue(pooledObject.pool == this);

            obj.SetActive(false);
            instances.Push(obj);
        }

        public void Reset()
        {
            var objectsToReturn = new List<GameObject>();
            foreach (var instance in transform.GetComponentsInChildren<PooledObject>())
            {
                if (instance.gameObject.activeSelf)
                {
                    objectsToReturn.Add(instance.gameObject);
                }
            }
            foreach (var instance in objectsToReturn)
            {
                ReturnObject(instance);
            }
        }

        private GameObject CreateInstans()
        {
            var obj = Instantiate(prefab);
            var pooledObject = obj.AddComponent<PooledObject>();
            pooledObject.pool = this;
            obj.transform.SetParent(transform);
            obj.transform.Reset();
            return obj;
        }
    }

    public class PooledObject : MonoBehaviour
    {
        public ObjectPool pool;
    }
}
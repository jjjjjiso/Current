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

        /// <summary>
        /// 풀에서 새 객체를 반환합니다.
        /// </summary>
        /// <returns>풀의 새 객체.</returns>
        public GameObject GetObj()
        {
            return (instances.Count > 0) ? instances.Pop() : CreateInstans();
        }

        /// <summary>
        /// 지정된 게임 객체를 원래의 풀에 반환합니다.
        /// </summary>
        /// <param name="obj">원점 풀로 돌아가는 객체.</param>
        public void ReturnObject(GameObject obj)
        {
            var pooledObject = obj.GetComponent<PooledObject>();
            if (pooledObject == null) return;
            if (pooledObject.pool != this) return;

            obj.SetActive(false);
            instances.Push(obj);
        }

        /// <summary>
        /// 개체 풀을 초기 상태로 다시 설정합니다.
        /// </summary>
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

        /// <summary>
        /// 풀 된 객체 유형의 새 인스턴스를 만듭니다.
        /// </summary>
        /// <returns>풀링 된 객체 유형의 새 인스턴스.</returns>
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

    /// <summary>
    /// 풀 된 객체의 풀을 식별하는 유틸리티 클래스.
    /// </summary>
    public class PooledObject : MonoBehaviour
    {
        public ObjectPool pool;
    }
}
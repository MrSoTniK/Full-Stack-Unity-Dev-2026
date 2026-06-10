using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Pool
{
    public class GameObjectPool : MonoBehaviour
    {
        public Action<Vector3> OnElementReleased;

        [SerializeField]
        private GameObject _prefab;

        [SerializeField]
        private Transform _container;

        [SerializeField]
        private int _initialSize;

        private Stack<GameObject> _stack;

        private void Awake()
        {
            _stack = new();

            for (int index = 0; index < _initialSize; index++)
            {
                CreateNewElement();
            }
        }

        public void Release(GameObject element)
        {
            element.gameObject.SetActive(false);
            _stack.Push(element);
            OnElementReleased?.Invoke(element.transform.position);
        }

        public GameObject Get()
        {
            return _stack.Count > 0 ? _stack.Pop() : CreateNewElement();
        }

        private GameObject CreateNewElement()
        {
            var element = GameObject.Instantiate(_prefab, _container);
            element.gameObject.SetActive(false);
            _stack.Push(element);

            return element;
        }
    }
}
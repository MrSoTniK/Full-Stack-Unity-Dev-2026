using Modules.Utils;
using UnityEngine;

namespace Game 
{
    public class TransformsPool : MonoBehaviour
    {
        [SerializeField]
        private Transform[] _array;

        private int _index;

        private void Awake()
        {
            _array.Shuffle();
            _index = 0;
        }

        public Vector3 NextPos()
        {
            if (_index >= _array.Length)
            {
                _array.Shuffle();
                _index = 0;
            }

            int currentIndex = _index;
            _index++;
            return _array[currentIndex].position;
        }
    }
}
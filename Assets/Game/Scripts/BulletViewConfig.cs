using UnityEngine;

namespace Game
{
    // +
    // нужно поменьше геттеров, не является объектом 
    [CreateAssetMenu(
        fileName = "BulletViewConfig",
        menuName = "Game/New BulletViewConfig"
    )]
    public sealed class BulletViewConfig : ScriptableObject
    {
        [field: SerializeField]
        public GameObject BlueVFX { get; private set; }
        
        [field: SerializeField]
        public GameObject RedVFX { get; private set; }
        
        [field: SerializeField]
        public GameObject ExplosionVFX  { get; private set; }
    }
}
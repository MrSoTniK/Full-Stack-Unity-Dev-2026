using DG.Tweening;
using UnityEngine;

namespace Game.Unit 
{
    public class UnitView : MonoBehaviour
    {
        [Header("Base")]
        [SerializeField]
        private Unit _unit;

        [SerializeField]
        private Renderer _renderer;

        [SerializeField]
        private Transform _viewTransform;

        [Header("Fire VFX")]
        [SerializeField]
        private ParticleSystem _fireVFX;

        [Header("Config")]
        [SerializeField]
        private ShipControllerViewConfig _viewConfig;

        private Material _material;
        private Tweener _damageAnimation;

        private Vector3 _moveDirection;

        private void Awake()
        {
            _material = new Material(_viewConfig.MaterialPrefab);
            _renderer.material = _material;
        }

        private void OnEnable()
        {
            _unit.OnMove += SetMoveDirection;
        }

        private void OnDisable()
        {
            _unit.OnMove -= SetMoveDirection;
        }

        private void LateUpdate()
        {
            this.AnimateMovement(Time.deltaTime);
        }

        public void SetMoveDirection(Vector2 moveDirection) 
        {
            _moveDirection = moveDirection;
        }

        public void PlayFireVFX() 
        {
            _fireVFX.Play();
        }

        public void AnimateDamage()
        {
            if (_damageAnimation.IsActive())
                _damageAnimation.Kill();

            _damageAnimation = DOVirtual.Float(
                0f,
                1f,
                _viewConfig.HitDuration,
                progress => _material?.SetFloat(_viewConfig.HitPropertyName,
                    _viewConfig.HitAnimationCurve.Evaluate(progress))
            ).SetLink(_renderer.gameObject);
        }

        public void ShowDeathEffect(Vector3 position) 
        {
            ParticleSystem prefab = _viewConfig.DestroyEffectPrefab;
            Instantiate(prefab, position, prefab.transform.rotation);
        }

        private void AnimateMovement(float deltaTime)
        {
            Vector3 shipAngles = _viewTransform.localEulerAngles;
            shipAngles.x = _viewConfig.MoveRotationAngle * _moveDirection.y;
            shipAngles.y = _viewConfig.MoveRotationAngle / 2 * _moveDirection.x * -1f;

            Quaternion shipRotation = Quaternion.Euler(shipAngles);
            float t = _viewConfig.MoveSpeed * deltaTime;
            _viewTransform.localRotation = Quaternion.Lerp(_viewTransform.localRotation, shipRotation, t);
        }
    }
}
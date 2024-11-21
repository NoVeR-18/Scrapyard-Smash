using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        public bool IsMoving
        {
            get
            {
                return _inputMove.magnitude > 0.05f;
            }
        }

        [SerializeField] private PlayerMovementSettings _settings;
        [SerializeField] private Transform _modelContainer;

        [SerializeField]
        private PlayerControls _controls;
        private Rigidbody _rigidbody;
        private Vector3 _inputMove;
        private Vector3 _move;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            move();
            rotateModel();
        }
        private void move()
        {
            _inputMove = new Vector3(
                applySensitivity(_controls.Horizontal),
                0,
                applySensitivity(_controls.Vertical)
                );

            _move = transform.forward * _inputMove.x + transform.right * -_inputMove.z;
            _rigidbody.position = _rigidbody.position + _move;
        }
        private void rotateModel()
        {
            Vector3 rotationLookAtVector = Quaternion.AngleAxis(90, Vector3.up) * new Vector3(_controls.Horizontal, 0, _controls.Vertical);

            if (rotationLookAtVector == Vector3.zero)
                return;

            _modelContainer.rotation = Quaternion.Lerp(
                _modelContainer.rotation,
                Quaternion.LookRotation(rotationLookAtVector),
                _settings.ModelRotationLerp * Time.deltaTime
                );
        }
        private float applySensitivity(float value)
        {
            return _settings.SpeedSensitivityCurve.Evaluate(value);
        }
    }
}

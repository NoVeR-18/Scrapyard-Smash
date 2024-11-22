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
        public BoxCollider boxCollider;
        private Vector3 _inputMove;
        private Vector3 _move;
        public bool CanMoving = true;
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            boxCollider = GetComponent<BoxCollider>();
        }

        private void Update()
        {
            if (CanMoving)
            {
                move();
                rotateModel();
            }
        }
        private void move()
        {
            _inputMove = new Vector3(
                _controls.Horizontal,
                0,
                _controls.Vertical
                );

            _move = transform.right * _inputMove.x + transform.forward * _inputMove.z;
            _rigidbody.MovePosition(_rigidbody.position + (_move * Time.deltaTime * _settings.Speed));
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
    }
}

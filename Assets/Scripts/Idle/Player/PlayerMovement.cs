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
        public int MoveLevel = 0;
        public bool CanMoving = true;

        [Header("Wheel Settings")]
        [SerializeField] private Transform frontLeftWheel; // Передние колеса
        [SerializeField] private Transform rearLeftWheel;  // Задние колеса
        [SerializeField] private Transform rearRightWheel;
        [SerializeField] private Transform frontRightWheel;
        [SerializeField] private float wheelRotationSpeed = 360f; // Скорость вращения колес


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
                animateWheels();
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
            _rigidbody.MovePosition(_rigidbody.position + (_move * Time.deltaTime * ((_settings.Speed + MoveLevel * 0.5f))));
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
        private void animateWheels()
        {
            // Рассчитываем скорость вращения колес при движении
            float movementSpeed = _move.magnitude * _settings.Speed * wheelRotationSpeed * Time.deltaTime;

            // Вращаем все колеса вокруг оси X (анимация движения)
            if (frontLeftWheel != null)
            {
                frontLeftWheel.Rotate(Vector3.right, movementSpeed, Space.Self);
            }
            if (frontRightWheel != null)
            {
                frontRightWheel.Rotate(Vector3.right, -movementSpeed, Space.Self);
            }
            if (rearRightWheel != null)
            {
                rearRightWheel.Rotate(Vector3.right, -movementSpeed, Space.Self);
            }
            if (rearLeftWheel != null)
            {
                rearLeftWheel.Rotate(Vector3.right, movementSpeed, Space.Self);
            }

            //// Поворот передних колес (рулежка) по оси Y
            //float steeringAngle = _controls.Horizontal * _settings.SteeringAngle; // Угол поворота рулежки

            //if (frontLeftWheel != null && frontRightWheel != null)
            //{
            //    // Поворот переднего левого колеса (учитываем текущее вращение по X)
            //    Quaternion leftWheelRotation = Quaternion.Euler(
            //        frontLeftWheel.localRotation.x, // Текущее вращение по X
            //        steeringAngle,                    // Угол поворота рулежки по Y
            //        frontLeftWheel.rotation.z                                                       // Сохраняем текущий угол по Z
            //    );
            //    frontLeftWheel.localRotation = leftWheelRotation;

            //    // Поворот переднего правого колеса (учитываем текущее вращение по X)
            //    Quaternion rightWheelRotation = Quaternion.Euler(
            //        frontRightWheel.localEulerAngles.x, // Текущее вращение по X
            //        steeringAngle - 180f,                      // Угол поворота рулежки по Y
            //        0  // Сохраняем текущий угол по Z
            //    );
            //    frontRightWheel.localRotation = rightWheelRotation;
            //}
        }

    }
}

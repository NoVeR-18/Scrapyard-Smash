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
        public Transform modelContainer;

        public PlayerControls _controls;
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

        [SerializeField] private bool haveWheels = true;
        [SerializeField] private Transform crane;
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (boxCollider == null)
                boxCollider = GetComponent<BoxCollider>();
            if (crane != null)
                cranePoss = crane.localPosition;
        }
        private Vector3 cranePoss;
        private void Update()
        {
            if (CanMoving)
            {
                Move();
                rotateModel();
                if (haveWheels)
                    animateWheels();
                if (crane != null)
                    rotateCrane();
            }
        }
        private void Move()
        {
            // Получаем ввод пользователя
            _inputMove = new Vector3(
                _controls.Horizontal,
                0,
                _controls.Vertical
            );

            // Рассчитываем направление движения
            _move = transform.right * _inputMove.x + transform.forward * _inputMove.z;

            // Устанавливаем скорость Rigidbody
            float speed = _settings.Speed + MoveLevel * 0.5f;
            _rigidbody.velocity = _move * speed;
        }
        private void rotateModel()
        {
            Vector3 rotationLookAtVector = Quaternion.AngleAxis(90, Vector3.up) * new Vector3(_controls.Horizontal, 0, _controls.Vertical);

            if (rotationLookAtVector == Vector3.zero)
                return;

            modelContainer.rotation = Quaternion.Lerp(
                modelContainer.rotation,
                Quaternion.LookRotation(rotationLookAtVector),
                _settings.ModelRotationLerp * Time.deltaTime
                );
        }
        private void rotateCrane()
        {
            // Вычисляем целевой вектор для вращения
            Vector3 rotationLookAtVector = Quaternion.AngleAxis(90, Vector3.up) * new Vector3(_controls.Horizontal, 0, _controls.Vertical);

            // Возвращаем кран в изначальную позицию
            crane.localPosition = cranePoss;

            if (rotationLookAtVector == Vector3.zero)
                return;

            // Сохраняем текущего родителя
            Transform originalParent = crane.parent;

            // Временно отсоединяем кран от модели, чтобы вращать в глобальных координатах
            crane.parent = null;

            // Выполняем плавное вращение в глобальных координатах
            crane.rotation = Quaternion.Lerp(
                crane.rotation,
                Quaternion.LookRotation(rotationLookAtVector),
                _settings.ModelRotationLerp * Time.deltaTime * 0.5f
            );

            // Возвращаем кран обратно в иерархию
            crane.parent = originalParent;
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
        }

    }
}

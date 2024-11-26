using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerMovement), typeof(PlayerBackpack))]
    public class Player : MonoBehaviour
    {
        public PlayerMovement Movement { get => _movement; }
        public PlayerBackpack Backpack { get => _backpack; }
        public PlayerWallet Wallet { get => _wallet; }
        public Transform DefaultSpawnPoss;
        public Camera mainCamera;
        public VehicleZone vehicleZone;
        [SerializeField] private Vector3 _spawnPoint;
        [SerializeField] private float _dieHeight = -20;

        private PlayerWallet _wallet;
        private PlayerBackpack _backpack;
        private PlayerMovement _movement;

        private void Awake()
        {
            _backpack = GetComponent<PlayerBackpack>();
            _movement = GetComponent<PlayerMovement>();
            _wallet = GetComponent<PlayerWallet>();
        }
        virtual public void Start()
        {
            _spawnPoint = new Vector3(DefaultSpawnPoss.position.x, transform.position.y, DefaultSpawnPoss.position.z);
            _backpack.LoadBackpack();
            SpawnPlayer();

            if (vehicleZone.gameObject.activeSelf)
                DisableVechicle();
        }

        private void Update()
        {
            validateDeathHeight();
        }
        private void validateDeathHeight()
        {
            if (transform.position.y < _dieHeight)
            {
                SpawnPlayer();
            }
        }
        private void SpawnPlayer()
        {
            transform.position = _spawnPoint;
        }
        private void OnApplicationQuit()
        {
            _backpack.SaveBackpack();
        }

        public void DisableVechicle()
        {
            _movement.CanMoving = false;
            mainCamera.gameObject.SetActive(false);
            SpawnPlayer();
            vehicleZone.gameObject.SetActive(true);
            _movement.boxCollider.isTrigger = true;
        }

        public void EnableVechicle()
        {
            _movement.CanMoving = true;
            _movement.boxCollider.isTrigger = false;
            mainCamera.gameObject.SetActive(true);
            vehicleZone.gameObject.SetActive(false);
        }

    }
}
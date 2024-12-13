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
        [SerializeField] protected Vector3 _spawnPoint;
        [SerializeField] private float _dieHeight = -20;

        public CapacityBar capacityBar;
        public AudioSource audioSource;
        public ParticleSystem particle;
        protected PlayerWallet _wallet;
        protected PlayerBackpack _backpack;
        protected PlayerMovement _movement;

        private void Awake()
        {
            _backpack = GetComponent<PlayerBackpack>();
            _movement = GetComponent<PlayerMovement>();
            _wallet = GetComponent<PlayerWallet>();
            if (DefaultSpawnPoss != null)
                _spawnPoint = new Vector3(DefaultSpawnPoss.position.x, DefaultSpawnPoss.transform.position.y, DefaultSpawnPoss.position.z);
            else
                _spawnPoint = transform.position;
        }
        virtual public void Start()
        {
            SpawnPlayer();
            if (vehicleZone != null)
            {
                if (vehicleZone.gameObject.activeSelf)
                    DisableVechicle();
            }

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
        public void SpawnPlayer()
        {
            transform.position = _spawnPoint;
        }
        private void OnApplicationQuit()
        {
            _backpack.SaveBackpack();
        }

        virtual public void DisableVechicle()
        {
            _movement.CanMoving = false;
            mainCamera.gameObject.SetActive(false);
            SpawnPlayer();
            vehicleZone?.gameObject.SetActive(true);
            _movement.boxCollider.isTrigger = true;
            if (audioSource != null)
                audioSource?.Stop();
            if (particle != null)
                particle.Play();

            _backpack.ItemsContainer.editCountItems -= capacityBar.UpdateUI;
        }

        virtual public bool EnableVechicle()
        {
            _movement.CanMoving = true;
            _movement.boxCollider.isTrigger = false;
            if (audioSource != null)
                audioSource?.Play();

            if (particle != null)
                particle.Stop();
            mainCamera.gameObject.SetActive(true);
            vehicleZone?.gameObject.SetActive(false);
            if (capacityBar != null)
            {
                capacityBar.UpdateUI(this);
                _backpack.ItemsContainer.editCountItems += capacityBar.UpdateUI;
            }
            return true;
        }

        virtual public void SelectVehicle(Transform transform)
        {
            if (!_movement.CanMoving)
            {
                EnableVechicle();
                this.transform.position = transform.position;
                this.transform.rotation = transform.rotation;
            }
        }
    }
}
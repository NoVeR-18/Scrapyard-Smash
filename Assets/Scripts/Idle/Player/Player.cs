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
        private void Start()
        {
            _backpack.LoadBackpack();
            SpawnPlayer();
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

    }
}
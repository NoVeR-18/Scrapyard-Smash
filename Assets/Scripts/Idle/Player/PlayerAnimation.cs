using UnityEngine;

namespace Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private Player _player;
        private void Start()
        {
            TryGetComponent<Player>(out _player);
        }
        private void Update()
        {
            _animator.SetBool("isWalking", _player.Movement.IsMoving);
        }
    }
}

using UnityEngine;

namespace Player
{
    public class PlayerControls : MonoBehaviour
    {
        public float Horizontal { get => _joystick.Horizontal; }
        public float Vertical { get => _joystick.Vertical; }

        [SerializeField] private Joystick _joystick;

        public Transform tutorialSwipe;

        private void Start()
        {
            EnableTutorial();
        }

        private void Click()
        {
            tutorialSwipe.gameObject.SetActive(false);
            _joystick.FirstClick -= Click;
        }

        public void EnableTutorial()
        {
            tutorialSwipe.gameObject.SetActive(true);
            _joystick.FirstClick += Click;
        }
    }
}
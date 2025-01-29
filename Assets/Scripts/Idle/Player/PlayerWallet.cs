using System.Collections;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerWallet : MonoBehaviour
    {
        private const string BalanceKey = "PlayerBalance";
        private const string CrystalsKey = "PlayerCrystals";

        public GameObject crystalUIPrefab; // Префаб UI-кристалла
        public GameObject moneyUIPrefab; // Префаб UI-кристалла
        public Canvas canvas; // Ссылка на Canvas для размещения UI-кристалла

        [SerializeField] private TextMeshProUGUI balanceText;
        [SerializeField] private TextMeshProUGUI crystalsText;

        public float Balance { get; private set; } = 0;
        public int Crystals { get; private set; } = 0;

        private void Start()
        {
            LoadBalance();
            LoadCrystals();
            UpdateBalanceUI();
            UpdateCrystalsUI();
        }

        // Методы для баланса (деньги)
        public void AddMoney(float amount)
        {
            Balance += amount;
            SaveBalance();
            UpdateBalanceUI();
            Debug.Log($"Деньги добавлены: {amount}. Баланс: {Balance}");
        }

        public bool WithdrawMoney(float amount)
        {
            if (Balance >= amount)
            {
                Balance -= amount;
                SaveBalance();
                UpdateBalanceUI();
                Debug.Log($"Снято: {amount}. Оставшийся баланс: {Balance}");
                return true;
            }
            else
            {
                Debug.Log("Недостаточно средств для снятия");
                return false;
            }
        }

        public bool CanWithdrawMoney(float amount)
        {
            return Balance >= amount;
        }

        private void SaveBalance()
        {
            PlayerPrefs.SetFloat(BalanceKey, Balance);
            PlayerPrefs.Save();
        }

        private void LoadBalance()
        {
            Balance = PlayerPrefs.GetFloat(BalanceKey, 0);
        }

        private void UpdateBalanceUI()
        {
            if (balanceText != null)
            {
                balanceText.text = $"{Balance}";
            }
        }

        // Методы для кристаллов
        public void AddCrystals(int amount)
        {
            Crystals += amount;
            SaveCrystals();
            UpdateCrystalsUI();
            Debug.Log($"Кристаллы добавлены: {amount}. Всего кристаллов: {Crystals}");
        }

        public bool SpendCrystals(int amount)
        {
            if (Crystals >= amount)
            {
                Crystals -= amount;
                SaveCrystals();
                UpdateCrystalsUI();
                Debug.Log($"Кристаллы потрачены: {amount}. Оставшиеся кристаллы: {Crystals}");
                return true;
            }
            else
            {
                Debug.Log("Недостаточно кристаллов для снятия");
                return false;
            }
        }

        public bool CanSpendCrystals(int amount)
        {
            return Crystals >= amount;
        }

        private void SaveCrystals()
        {
            PlayerPrefs.SetInt(CrystalsKey, Crystals);
            PlayerPrefs.Save();
        }

        private void LoadCrystals()
        {
            Crystals = PlayerPrefs.GetInt(CrystalsKey, 0);
        }

        private void UpdateCrystalsUI()
        {
            if (crystalsText != null)
            {
                crystalsText.text = $"{Crystals}";
            }
        }

        public void CollectCrystal(Vector3 worldPosition)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

            // Конвертируем экранные координаты в локальные координаты Canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, screenPos, canvas.worldCamera, out Vector2 localPos
            );

            // Создаем UI-кристалл
            GameObject crystalUI = Instantiate(crystalUIPrefab, canvas.transform);
            RectTransform crystalRect = crystalUI.GetComponent<RectTransform>();

            crystalRect.anchoredPosition = localPos; // Устанавливаем начальную позицию

            // Получаем локальную позицию цели внутри Canvas
            RectTransform crystalTargetRect = crystalsText.GetComponent<RectTransform>();
            Vector2 targetLocalPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, crystalTargetRect.position),
                canvas.worldCamera,
                out targetLocalPos);

            // Запускаем анимацию полета
            StartCoroutine(MoveToTarget(crystalRect, targetLocalPos));
        }
        public void CollectMoney(Vector3 worldPosition)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

            // Конвертируем экранные координаты в локальные координаты Canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, screenPos, canvas.worldCamera, out Vector2 localPos
            );

            // Создаем UI-кристалл
            GameObject crystalUI = Instantiate(moneyUIPrefab, canvas.transform);
            RectTransform crystalRect = crystalUI.GetComponent<RectTransform>();

            crystalRect.anchoredPosition = localPos; // Устанавливаем начальную позицию

            // Получаем локальную позицию цели внутри Canvas
            RectTransform crystalTargetRect = balanceText.GetComponent<RectTransform>();
            Vector2 targetLocalPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, crystalTargetRect.position),
                canvas.worldCamera,
                out targetLocalPos);

            // Запускаем анимацию полета
            StartCoroutine(MoveToTarget(crystalRect, targetLocalPos));
        }

        private IEnumerator MoveToTarget(RectTransform crystalRect, Vector2 targetPos)
        {
            float duration = 0.8f; // Время полета
            float elapsed = 0f;
            Vector2 startPos = crystalRect.anchoredPosition;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                t = t * t * (3f - 2f * t); // Смягченная анимация

                crystalRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
                yield return null;
            }

            Destroy(crystalRect.gameObject);
        }



    }
}

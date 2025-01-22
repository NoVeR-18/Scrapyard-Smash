using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerWallet : MonoBehaviour
    {
        private const string BalanceKey = "PlayerBalance";
        private const string CrystalsKey = "PlayerCrystals";

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
    }
}

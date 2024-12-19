using GameAnalyticsSDK;
using TMPro;
using UnityEngine;
namespace Player
{
    public class PlayerWallet : MonoBehaviour
    {
        private const string BalanceKey = "PlayerBalance";

        [SerializeField] private TextMeshProUGUI balanceText;

        public float Balance { get; private set; } = 0;

        private void Start()
        {
            LoadBalance();
            UpdateBalanceUI();
        }

        public void AddMoney(float amount)
        {
            Balance += amount;
            SaveBalance();
            UpdateBalanceUI();
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Balance", Balance, "", "");
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
                GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Money", 2, "", "");
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
            if (Balance >= amount)
                return true;
            else
                return false;
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
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MagneteUpgradeTab : MonoBehaviour
{
    public ExperienceTable experienceTable;
    public Button closeButton;
    public Transform UpgradePanelTransform;
    public Magnete magnete;
    [Header("Speed")]
    public Button SpeedUpgradeButton;
    public Image SpeedLevelFillAmount;
    public TextMeshProUGUI SpeedUpgradeCostText;
    public TextMeshProUGUI SpeedLevelText;
    public Transform SpeedUpgradeBlocked;
    public TextMeshProUGUI SpeedBlockedCostText;
    public Transform SpeedUpgradeMax;

    [Header("Weight")]
    public Button WeightUpgradeButton;
    public Image WeightLevelFillAmount;
    public TextMeshProUGUI WeightUpgradeCostText;
    public TextMeshProUGUI WeightLevelText;
    public Transform WeightUpgradeBlocked;
    public TextMeshProUGUI WeightBlockedCostText;
    public Transform WeightUpgradeMax;

    [Header("Magnet")]
    public Button MagnetUpgradeButton;
    public Image MagnetLevelFillAmount;
    public TextMeshProUGUI MagnetUpgradeCostText;
    public TextMeshProUGUI MagnetLevelText;
    public Transform MagnetUpgradeBlocked;
    public TextMeshProUGUI MagnetBlockedCostText;
    public Transform MagnetUpgradeMax;

    private void Start()
    {
        UpdatePanel();
        closeButton.onClick.AddListener(() => { CloseTab(); });

        SpeedUpgradeButton.onClick.AddListener(() => { OnUpgradeSpeedButton(); });
        WeightUpgradeButton.onClick.AddListener(() => { OnUpgradeWeightButton(); });
        MagnetUpgradeButton.onClick.AddListener(() => { OnUpgradeMagnetButton(); });
        if (experienceTable == null)
            experienceTable = Resources.Load<ExperienceTable>("ExperienceTable");
    }


    public void OnUpgradeSpeedButton()
    {
        if (magnete != null)
        {
            var currentLevel = experienceTable.experiencePerLevel[magnete.SpeedLevel];
            if (LevelManager.Instance.wallet.WithdrawMoney(currentLevel))
            {
                magnete.UpgradeSpeed();
                UpdatePanel();
            }
            else
            {
                Debug.Log("недостаточно денег для увеличения скорости.");
            }
        }

    }

    public void OnUpgradeWeightButton()
    {
        if (magnete != null)
        {
            var currentLevel = experienceTable.experiencePerLevel[magnete.WeightLevel];
            if (LevelManager.Instance.wallet.WithdrawMoney(currentLevel))
            {
                magnete.UpgradeMagnete();
                UpdatePanel();
            }
            else
            {
                Debug.Log("Недостаточно денег для увеличения грузоподьемности.");
            }
        }
    }

    public void OnUpgradeMagnetButton()
    {
        if (magnete != null)
        {
            var currentLevel = experienceTable.experiencePerLevel[magnete.MagnetLevel];
            if (LevelManager.Instance.wallet.WithdrawMoney(currentLevel))
            {
                magnete.UpgradeWeight();
                UpdatePanel();
            }
            else
            {
                Debug.Log("Недостаточно денег для увеличения грузоподьемности.");
            }
        }
    }


    private void UpdatePanel()
    {
        if (magnete != null)
        {

            SpeedUpgradeCostText.text = experienceTable.experiencePerLevel[magnete.SpeedLevel].ToString();
            WeightUpgradeCostText.text = experienceTable.experiencePerLevel[magnete.WeightLevel].ToString();
            MagnetUpgradeCostText.text = experienceTable.experiencePerLevel[magnete.MagnetLevel].ToString();
            SpeedLevelText.text = (magnete.SpeedLevel + 1).ToString();
            WeightLevelText.text = (magnete.WeightLevel + 1).ToString();
            MagnetLevelText.text = magnete.MagnetLevel.ToString();
            SpeedLevelFillAmount.fillAmount = (float)(magnete.SpeedLevel + 1) / experienceTable.experiencePerLevel.Count;
            WeightLevelFillAmount.fillAmount = (float)(magnete.WeightLevel + 1) / experienceTable.experiencePerLevel.Count;
            MagnetLevelFillAmount.fillAmount = (float)(magnete.MagnetLevel + 1) / experienceTable.experiencePerLevel.Count;

            if (magnete.SpeedLevel + 1 == experienceTable.experiencePerLevel.Count)
            {
                SpeedUpgradeMax.gameObject.SetActive(true);
                SpeedUpgradeButton.gameObject.SetActive(false);
            }
            else
            {
                SpeedUpgradeMax.gameObject.SetActive(false);

                var currentCost = experienceTable.experiencePerLevel[magnete.SpeedLevel];
                if (!LevelManager.Instance.wallet.CanWithdrawMoney(currentCost))
                {
                    SpeedUpgradeBlocked.gameObject.SetActive(true);
                    SpeedBlockedCostText.text = currentCost.ToString();
                    SpeedUpgradeButton.gameObject.SetActive(false);
                }
                else
                {
                    SpeedUpgradeBlocked.gameObject.SetActive(false);
                    SpeedUpgradeButton.gameObject.SetActive(true);
                    SpeedUpgradeCostText.text = $"{experienceTable.experiencePerLevel[magnete.SpeedLevel]}";
                }
            }
            if (magnete.WeightLevel + 1 == experienceTable.experiencePerLevel.Count)
            {
                WeightUpgradeMax.gameObject.SetActive(true);
                WeightUpgradeButton.gameObject.SetActive(false);
            }
            else
            {
                WeightUpgradeMax.gameObject.SetActive(false);

                var currentCost = experienceTable.experiencePerLevel[magnete.WeightLevel];
                if (!LevelManager.Instance.wallet.CanWithdrawMoney(currentCost))
                {
                    WeightUpgradeBlocked.gameObject.SetActive(true);
                    WeightBlockedCostText.text = currentCost.ToString();
                    WeightUpgradeButton.gameObject.SetActive(false);
                }
                else
                {
                    WeightUpgradeBlocked.gameObject.SetActive(false);
                    WeightUpgradeButton.gameObject.SetActive(true);
                    WeightUpgradeCostText.text = $"{experienceTable.experiencePerLevel[magnete.WeightLevel]}";
                }
            }
            if (magnete.MagnetLevel == experienceTable.experiencePerLevel.Count)
            {
                MagnetUpgradeMax.gameObject.SetActive(true);
                MagnetUpgradeButton.gameObject.SetActive(false);
            }
            else
            {
                MagnetUpgradeMax.gameObject.SetActive(false);

                var currentCost = experienceTable.experiencePerLevel[magnete.MagnetLevel];
                if (!LevelManager.Instance.wallet.CanWithdrawMoney(currentCost))
                {
                    MagnetUpgradeBlocked.gameObject.SetActive(true);
                    MagnetBlockedCostText.text = currentCost.ToString();
                    MagnetUpgradeButton.gameObject.SetActive(false);
                }
                else
                {
                    MagnetUpgradeBlocked.gameObject.SetActive(false);
                    MagnetUpgradeButton.gameObject.SetActive(true);
                    MagnetUpgradeCostText.text = $"{experienceTable.experiencePerLevel[magnete.MagnetLevel]}";
                }
            }
        }
    }

    public void CloseTab()
    {
        UpgradePanelTransform.gameObject.SetActive(false);
    }
    public void AddCoins()
    {
        LevelManager.Instance.wallet.AddMoney(1000);
    }
    private void OnEnable()
    {
        UpdatePanel();
    }
}

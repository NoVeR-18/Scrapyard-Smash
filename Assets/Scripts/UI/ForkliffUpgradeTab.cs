using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForkliffUpgradeTab : MonoBehaviour
{
    public ExperienceTable experienceTable;
    public Transform UpgradePanelTransform;
    public Forkliff forkliff;
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

    private void Start()
    {
        UpdatePanel();

        SpeedUpgradeButton.onClick.AddListener(() => { OnUpgradeSpeedButton(); });
        WeightUpgradeButton.onClick.AddListener(() => { OnUpgradeWeightButton(); });
        if (experienceTable == null)
            experienceTable = Resources.Load<ExperienceTable>("ExperienceTable");
    }


    public void OnUpgradeSpeedButton()
    {
        if (forkliff != null)
        {
            var currentLevel = experienceTable.experiencePerLevel[forkliff.SpeedLevel];
            if (LevelManager.Instance.wallet.WithdrawMoney(currentLevel))
            {
                forkliff.UpgradeSpeed();
                UpdatePanel();
            }
            else
            {
                Debug.Log("Недостаточно денег для увеличения скорости.");
            }
        }

    }

    public void OnUpgradeWeightButton()
    {
        if (forkliff != null)
        {
            var currentLevel = experienceTable.experiencePerLevel[forkliff.WeightLevel];
            if (LevelManager.Instance.wallet.WithdrawMoney(currentLevel))
            {
                forkliff.UpgradeWeight();
                UpdatePanel();

                if (Tutorial.instance != null)
                    if (Tutorial.instance.TutorialIndex == 3)
                        Tutorial.instance.CompleteZone(3);
            }
            else
            {
                Debug.Log("Недостаточно денег для увелечения денег.");
            }
        }
    }


    private void UpdatePanel()
    {
        if (forkliff != null)
        {

            SpeedUpgradeCostText.text = experienceTable.experiencePerLevel[forkliff.SpeedLevel].ToString();
            WeightUpgradeCostText.text = experienceTable.experiencePerLevel[forkliff.WeightLevel].ToString();
            SpeedLevelText.text = (forkliff.SpeedLevel + 1).ToString();
            WeightLevelText.text = (forkliff.WeightLevel + 1).ToString();
            SpeedLevelFillAmount.fillAmount = (float)((forkliff.SpeedLevel % (experienceTable.experiencePerLevel.Count / 3)) + 1) / (experienceTable.experiencePerLevel.Count / 3);
            WeightLevelFillAmount.fillAmount = (float)((forkliff.WeightLevel % (experienceTable.experiencePerLevel.Count / 3)) + 1) / (experienceTable.experiencePerLevel.Count / 3);

            if (forkliff.SpeedLevel + 1 == experienceTable.experiencePerLevel.Count)
            {
                SpeedUpgradeMax.gameObject.SetActive(true);
                SpeedUpgradeButton.gameObject.SetActive(false);
            }
            else
            {
                SpeedUpgradeMax.gameObject.SetActive(false);

                var currentCost = experienceTable.experiencePerLevel[forkliff.SpeedLevel];
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
                    SpeedUpgradeCostText.text = $"{experienceTable.experiencePerLevel[forkliff.SpeedLevel]}";
                }
            }
            if (forkliff.WeightLevel + 1 == experienceTable.experiencePerLevel.Count)
            {
                WeightUpgradeMax.gameObject.SetActive(true);
                WeightUpgradeButton.gameObject.SetActive(false);
            }
            else
            {
                WeightUpgradeMax.gameObject.SetActive(false);

                var currentCost = experienceTable.experiencePerLevel[forkliff.WeightLevel];
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
                    WeightUpgradeCostText.text = $"{experienceTable.experiencePerLevel[forkliff.WeightLevel]}";
                }
            }
        }
    }
    public void AddCoins()
    {
        LevelManager.Instance.wallet.AddMoney(100000);
    }
    private void OnEnable()
    {
        UpdatePanel();
    }

}

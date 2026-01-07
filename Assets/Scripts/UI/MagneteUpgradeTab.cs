using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MagneteUpgradeTab : MonoBehaviour
{
    public ExperienceTable experienceTable;
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

    public Button SpeedAdsButton;

    [Header("Weight")]
    public Button WeightUpgradeButton;
    public Image WeightLevelFillAmount;
    public TextMeshProUGUI WeightUpgradeCostText;
    public TextMeshProUGUI WeightLevelText;
    public Transform WeightUpgradeBlocked;
    public TextMeshProUGUI WeightBlockedCostText;
    public Transform WeightUpgradeMax;

    public Button WeightAdsButton;

    [Header("Magnet")]
    public Button MagnetUpgradeButton;
    public Image MagnetLevelFillAmount;
    public TextMeshProUGUI MagnetUpgradeCostText;
    public TextMeshProUGUI MagnetLevelText;
    public Transform MagnetUpgradeBlocked;
    public TextMeshProUGUI MagnetBlockedCostText;
    public Transform MagnetUpgradeMax;

    public Button MagnetAdsButton;

    private void Start()
    {
        UpdatePanel();

        SpeedUpgradeButton.onClick.AddListener(() =>
        {
            var currentLevel = experienceTable.experiencePerLevel[magnete.SpeedLevel];
            if (LevelManager.Instance.wallet.WithdrawMoney(currentLevel))
                OnUpgradeSpeedButton();
        });
        WeightUpgradeButton.onClick.AddListener(() =>
        {

            var currentLevel = experienceTable.experiencePerLevel[magnete.WeightLevel];
            if (LevelManager.Instance.wallet.WithdrawMoney(currentLevel))
                OnUpgradeWeightButton();
        });
        MagnetUpgradeButton.onClick.AddListener(() =>
        {
            var currentLevel = experienceTable.experiencePerLevel[magnete.MagnetLevel];
            if (LevelManager.Instance.wallet.WithdrawMoney(currentLevel))
                OnUpgradeMagnetButton();
        });
        if (experienceTable == null)
            experienceTable = Resources.Load<ExperienceTable>("ExperienceTable");
    }


    public void OnUpgradeSpeedButton()
    {
        if (magnete != null)
        {
            magnete.UpgradeSpeed();
            UpdatePanel();
        }
        DailyTasksManager.Instance.PerformTask(TaskType.UpgradeMagnete);
        GameManager.Instance.Vibrate();
    }

    public void OnUpgradeWeightButton()
    {
        if (magnete != null)
        {
            magnete.UpgradeWeight();
            UpdatePanel();
        }
        DailyTasksManager.Instance.PerformTask(TaskType.UpgradeMagnete);
        GameManager.Instance.Vibrate();
    }

    public void OnUpgradeMagnetButton()
    {
        if (magnete != null)
        {
            magnete.UpgradeMagnete();
            UpdatePanel();
        }

        DailyTasksManager.Instance.PerformTask(TaskType.UpgradeMagnete);
        GameManager.Instance.Vibrate();
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
            MagnetLevelText.text = (magnete.MagnetLevel + 1).ToString();
            SpeedLevelFillAmount.fillAmount = (float)((magnete.SpeedLevel % (experienceTable.experiencePerLevel.Count / 3)) + 1) / (experienceTable.experiencePerLevel.Count / 3);
            WeightLevelFillAmount.fillAmount = (float)((magnete.WeightLevel % (experienceTable.experiencePerLevel.Count / 3)) + 1) / (experienceTable.experiencePerLevel.Count / 3);
            MagnetLevelFillAmount.fillAmount = (float)((magnete.MagnetLevel % (experienceTable.experiencePerLevel.Count / 3)) + 1) / (experienceTable.experiencePerLevel.Count / 3);

            if (magnete.SpeedLevel + 1 == experienceTable.experiencePerLevel.Count)
            {
                SpeedUpgradeMax.gameObject.SetActive(true);
                SpeedUpgradeButton.gameObject.SetActive(false);
            }
            else
            {
                SpeedUpgradeMax.gameObject.SetActive(false);

                SpeedAdsButton.gameObject.SetActive(false);
                var currentCost = experienceTable.experiencePerLevel[magnete.SpeedLevel];
                if (!LevelManager.Instance.wallet.CanWithdrawMoney(currentCost))
                {

                    if (AdManager.Instance.IsRewardedReady())
                    {

                        SpeedAdsButton.gameObject.SetActive(true);

                        SpeedAdsButton.onClick.RemoveAllListeners();
                        SpeedAdsButton.onClick.AddListener(() =>
                        {
                            AdManager.Instance.ShowRewarded(() =>
                            {
                                OnUpgradeSpeedButton();
                            });
                        });
                    }
                    else
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

                WeightAdsButton.gameObject.SetActive(false);
                var currentCost = experienceTable.experiencePerLevel[magnete.WeightLevel];
                if (!LevelManager.Instance.wallet.CanWithdrawMoney(currentCost))
                {
                    if (AdManager.Instance.IsRewardedReady())
                    {
                        WeightAdsButton.gameObject.SetActive(true);
                        WeightAdsButton.onClick.RemoveAllListeners();
                        WeightAdsButton.onClick.AddListener(() =>
                        {
                            AdManager.Instance.ShowRewarded(() =>
                            {
                                OnUpgradeWeightButton();
                            });
                        });
                    }
                    else
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
            if (magnete.MagnetLevel + 1 == experienceTable.experiencePerLevel.Count)
            {
                MagnetUpgradeMax.gameObject.SetActive(true);
                MagnetUpgradeButton.gameObject.SetActive(false);
            }
            else
            {
                MagnetUpgradeMax.gameObject.SetActive(false);

                var currentCost = experienceTable.experiencePerLevel[magnete.MagnetLevel];
                MagnetAdsButton.gameObject.SetActive(false);
                if (!LevelManager.Instance.wallet.CanWithdrawMoney(currentCost))
                {
                    if (AdManager.Instance.IsRewardedReady())
                    {
                        MagnetAdsButton.gameObject.SetActive(true);
                        MagnetAdsButton.onClick.RemoveAllListeners();
                        MagnetAdsButton.onClick.AddListener(() =>
                        {
                            AdManager.Instance.ShowRewarded(() =>
                            {
                                OnUpgradeMagnetButton();
                            });
                        });
                    }
                    else
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

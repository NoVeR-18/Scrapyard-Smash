using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelicopterUpgradeTab : MonoBehaviour
{
    public ExperienceTable experienceTable;
    public Transform UpgradePanelTransform;
    public Helicopter helicopter;

    [Header("Fuel Recovery Speed")]
    public Button RecoveryUpgradeButton;
    public Image RecoveryLevelFillAmount;
    public TextMeshProUGUI RecoveryUpgradeCostText;
    public TextMeshProUGUI RecoveryLevelText;
    public Transform RecoveryUpgradeBlocked;
    public TextMeshProUGUI RecoveryBlockedCostText;
    public Transform RecoveryUpgradeMax;

    public Button FuelRecoveryAdsButton;

    [Header("Fuel Capacity")]
    public Button FuelUpgradeButton;
    public Image FuelLevelFillAmount;
    public TextMeshProUGUI FuelUpgradeCostText;
    public TextMeshProUGUI FuelLevelText;
    public Transform FuelUpgradeBlocked;
    public TextMeshProUGUI FuelBlockedCostText;
    public Transform FuelUpgradeMax;

    public Button FuelAdsButton;

    [Header("Weight Capacity")]
    public Button WeightUpgradeButton;
    public Image WeightLevelFillAmount;
    public TextMeshProUGUI WeightUpgradeCostText;
    public TextMeshProUGUI WeightLevelText;
    public Transform WeightUpgradeBlocked;
    public TextMeshProUGUI WeightBlockedCostText;
    public Transform WeightUpgradeMax;

    public Button WeightAdsButton;

    private void Start()
    {
        UpdatePanel();

        RecoveryUpgradeButton.onClick.AddListener(() =>
        {
            var currentCost = experienceTable.experiencePerLevel[helicopter.RecoveryUpgradeLevel];
            if (LevelManager.Instance.wallet.WithdrawMoney(currentCost)) OnUpgradeRecoveryButton();
        });
        FuelUpgradeButton.onClick.AddListener(() =>
        {
            var currentCost = experienceTable.experiencePerLevel[helicopter.MaxFuelLevel];
            if (LevelManager.Instance.wallet.WithdrawMoney(currentCost)) OnUpgradeFuelButton();
        });
        WeightUpgradeButton.onClick.AddListener(() =>
        {
            var currentCost = experienceTable.experiencePerLevel[helicopter.WeightLevel];
            if (LevelManager.Instance.wallet.WithdrawMoney(currentCost)) OnUpgradeWeightButton();
        });

        if (experienceTable == null)
            experienceTable = Resources.Load<ExperienceTable>("ExperienceTable");
    }

    public void OnUpgradeRecoveryButton()
    {
        if (helicopter != null)
        {
            helicopter.UpgradeRecoverySpeed();
            UpdatePanel();
        }
        GameManager.Instance.Vibrate();
    }

    public void OnUpgradeFuelButton()
    {
        if (helicopter != null)
        {
            helicopter.UpgradeFuel();
            UpdatePanel();
        }
        GameManager.Instance.Vibrate();
    }

    public void OnUpgradeWeightButton()
    {
        if (helicopter != null)
        {
            helicopter.UpgradeWeight();
            UpdatePanel();
        }
        GameManager.Instance.Vibrate();
    }

    private void UpdatePanel()
    {
        if (helicopter != null)
        {
            // Обновление текста и заполнения для скорости восстановления
            RecoveryUpgradeCostText.text = experienceTable.experiencePerLevel[helicopter.RecoveryUpgradeLevel].ToString();
            RecoveryLevelText.text = (helicopter.RecoveryUpgradeLevel + 1).ToString();
            RecoveryLevelFillAmount.fillAmount = (float)((helicopter.RecoveryUpgradeLevel % (experienceTable.experiencePerLevel.Count / 3)) + 1) / (experienceTable.experiencePerLevel.Count / 3);

            // Блокировка кнопок или отображение максимального уровня
            if (helicopter.RecoveryUpgradeLevel + 1 == experienceTable.experiencePerLevel.Count)
            {
                RecoveryUpgradeMax.gameObject.SetActive(true);
                RecoveryUpgradeButton.gameObject.SetActive(false);
            }
            else
            {
                RecoveryUpgradeMax.gameObject.SetActive(false);

                var currentCost = experienceTable.experiencePerLevel[helicopter.RecoveryUpgradeLevel];
                FuelRecoveryAdsButton.gameObject.SetActive(false);
                if (!LevelManager.Instance.wallet.CanWithdrawMoney(currentCost))
                {
                    if (AdManager.Instance.IsRewardedReady())
                    {
                        FuelRecoveryAdsButton.gameObject.SetActive(true);

                        FuelRecoveryAdsButton.onClick.RemoveAllListeners();
                        FuelRecoveryAdsButton.onClick.AddListener(() =>
                        {
                            AdManager.Instance.ShowRewarded(() =>
                            {
                                OnUpgradeRecoveryButton();
                            });
                        });
                    }
                    else
                        RecoveryUpgradeBlocked.gameObject.SetActive(true);
                    RecoveryBlockedCostText.text = currentCost.ToString();
                    RecoveryUpgradeButton.gameObject.SetActive(false);
                }
                else
                {
                    RecoveryUpgradeBlocked.gameObject.SetActive(false);
                    RecoveryUpgradeButton.gameObject.SetActive(true);
                }
            }

            // Топливный бак
            FuelUpgradeCostText.text = experienceTable.experiencePerLevel[helicopter.MaxFuelLevel].ToString();
            FuelLevelText.text = (helicopter.MaxFuelLevel + 1).ToString();
            FuelLevelFillAmount.fillAmount = (float)((helicopter.MaxFuelLevel % (experienceTable.experiencePerLevel.Count / 3)) + 1) / (experienceTable.experiencePerLevel.Count / 3);

            if (helicopter.MaxFuelLevel + 1 == experienceTable.experiencePerLevel.Count)
            {
                FuelUpgradeMax.gameObject.SetActive(true);
                FuelUpgradeButton.gameObject.SetActive(false);
            }
            else
            {
                FuelUpgradeMax.gameObject.SetActive(false);

                var currentCost = experienceTable.experiencePerLevel[helicopter.MaxFuelLevel];
                FuelAdsButton.gameObject.SetActive(false);
                if (!LevelManager.Instance.wallet.CanWithdrawMoney(currentCost))
                {
                    if (AdManager.Instance.IsRewardedReady())
                    {
                        FuelAdsButton.gameObject.SetActive(true);

                        FuelAdsButton.onClick.RemoveAllListeners();
                        FuelAdsButton.onClick.AddListener(() =>
                        {
                            AdManager.Instance.ShowRewarded(() =>
                            {
                                OnUpgradeFuelButton();
                            });
                        });
                    }
                    else
                        FuelUpgradeBlocked.gameObject.SetActive(true);
                    FuelBlockedCostText.text = currentCost.ToString();
                    FuelUpgradeButton.gameObject.SetActive(false);
                }
                else
                {
                    FuelUpgradeBlocked.gameObject.SetActive(false);
                    FuelUpgradeButton.gameObject.SetActive(true);
                }
            }

            // Грузоподъемность
            WeightUpgradeCostText.text = experienceTable.experiencePerLevel[helicopter.WeightLevel].ToString();
            WeightLevelText.text = (helicopter.WeightLevel + 1).ToString();
            WeightLevelFillAmount.fillAmount = (float)((helicopter.WeightLevel % (experienceTable.experiencePerLevel.Count / 3)) + 1) / (experienceTable.experiencePerLevel.Count / 3);

            if (helicopter.WeightLevel + 1 == experienceTable.experiencePerLevel.Count)
            {
                WeightUpgradeMax.gameObject.SetActive(true);
                WeightUpgradeButton.gameObject.SetActive(false);
            }
            else
            {
                WeightUpgradeMax.gameObject.SetActive(false);

                var currentCost = experienceTable.experiencePerLevel[helicopter.WeightLevel];
                WeightAdsButton.gameObject.SetActive(false);
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
                }
            }
        }
    }

    public void CloseTab()
    {
        UpgradePanelTransform.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        UpdatePanel();
    }
}

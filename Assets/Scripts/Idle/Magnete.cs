using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Magnete : Player.Player
{
    public int SpeedLevel;
    public int WeightLevel;
    public int MagnetLevel;

    const string SpeedKey = "MagneteSpeedKey";
    const string WeightKey = "MagneteWeightKey";
    const string MagnetKey = "MagneteMagnetKey";

    public CapsuleCollider magneteSize;
    public UpgradeNotification upgradeNotification;


    [Header("Models")]
    public List<Mesh> hullModels;
    public List<Mesh> handModels;
    public List<Mesh> armModels;
    public List<Mesh> magneteModels;

    [SerializeField] private MeshFilter hullMainModel;
    [SerializeField] private MeshFilter handMainModel;
    [SerializeField] private MeshFilter armMainModel;
    [SerializeField] private MeshFilter magneteMainModel;

    [SerializeField] private List<MeshFilter> wheelsMainModels;
    [SerializeField] private List<Transform> wheelsLvlThird;
    public Mesh wheelsFirstLevelMesh;
    public Mesh wheelsSecondLevelMesh;

    [Header("Unlock Vehicle")]
    [SerializeField] private Transform modelLock;
    [SerializeField] private Transform modelAssemble;


    [SerializeField] private List<Transform> AssemblyDetails;

    public bool unlocked = false;
    [SerializeField] private int _detailsToUnlock = 2;
    public int colectedDetails = 0;
    const string DetailsKey = "MagneteDetailsKey";


    public Transform partNotification;
    public TextMeshProUGUI partsCount;
    public Image partsFill;


    public override void Start()
    {
        base.Start();
        LoadProgress();
        UpdateModel();
        CheckUnlocked();
    }

    public override void SelectVehicle(Transform transform)
    {
        if (unlocked)
            base.SelectVehicle(transform);
    }

    public override void SelectVehicle()
    {
        if (unlocked)
            base.SelectVehicle();
    }

    public void ColectDetails()
    {
        colectedDetails++;
        PlayerPrefs.SetInt(DetailsKey, colectedDetails);
        PlayerPrefs.Save();
        CheckUnlocked();
        if (colectedDetails <= _detailsToUnlock)
            StartCoroutine(closeTab());
        UpdateModel();
    }
    private IEnumerator closeTab()
    {

        partNotification.gameObject.SetActive(true);
        partsCount.text = $"{colectedDetails}/{_detailsToUnlock}";
        partsFill.fillAmount = (float)colectedDetails / _detailsToUnlock;
        yield return new WaitForSeconds(3.0f);
        partNotification.gameObject.SetActive(false);

    }
    private void CheckUnlocked()
    {
        if (colectedDetails >= _detailsToUnlock)
        {
            unlocked = true;
        }
        else
            unlocked = false;
    }
    private void UpdateModel()
    {
        modelLock.gameObject.SetActive(false);
        modelAssemble.gameObject.SetActive(false);
        if (colectedDetails == 0)
        {
            Movement.modelContainer.gameObject.SetActive(false);
            modelLock.gameObject.SetActive(true);
            return;
        }
        else if (colectedDetails < _detailsToUnlock)
        {
            Movement.modelContainer.gameObject.SetActive(false);
            modelAssemble.gameObject.SetActive(true);


            float step = (float)_detailsToUnlock / AssemblyDetails.Count + 1;

            int targetActiveIndex = Mathf.FloorToInt(colectedDetails / step);

            for (int i = 0; i <= targetActiveIndex && i < AssemblyDetails.Count; i++)
            {
                if (!AssemblyDetails[i].gameObject.activeSelf)
                {
                    AssemblyDetails[i].gameObject.SetActive(true);
                }
            }

            return;
        }
        Movement.modelContainer.gameObject.SetActive(true);
        int index = WeightLevel / 9;
        hullMainModel.mesh = hullModels[index];
        handMainModel.mesh = handModels[index];
        armMainModel.mesh = armModels[index];
        int speedIndex = SpeedLevel / 9;
        if (speedIndex == 2)
        {
            foreach (Transform t in wheelsLvlThird) { t.gameObject.SetActive(true); }
            foreach (MeshFilter wheel in wheelsMainModels) { wheel.gameObject.SetActive(false); }
        }
        else if (speedIndex == 1)
        {
            foreach (Transform t in wheelsLvlThird) { t.gameObject.SetActive(false); }
            foreach (MeshFilter wheel in wheelsMainModels) { wheel.mesh = wheelsSecondLevelMesh; }
        }
        else
        {
            foreach (Transform t in wheelsLvlThird) { t.gameObject.SetActive(false); }
            foreach (MeshFilter wheel in wheelsMainModels) { wheel.mesh = wheelsFirstLevelMesh; }
        }

        int magneteIndex = MagnetLevel / 9;
        magneteMainModel.mesh = magneteModels[magneteIndex];


    }
    void LoadProgress()
    {
        SpeedLevel = PlayerPrefs.GetInt(SpeedKey, 0);
        WeightLevel = PlayerPrefs.GetInt(WeightKey, 0);
        MagnetLevel = PlayerPrefs.GetInt(MagnetKey, 0);
        colectedDetails = PlayerPrefs.GetInt(DetailsKey, 0);
        Movement.MoveLevel = SpeedLevel;
        magneteSize.radius = 1 + MagnetLevel * 0.05f;
        Backpack.ItemsContainer.CapacityLevel = WeightLevel;
        Backpack.ItemsContainer.InitClosely();
    }

    public void UpgradeWeight()
    {
        WeightLevel++;
        upgradeNotification.WeightUpgrade();
        PlayerPrefs.SetInt(WeightKey, WeightLevel);

        Backpack.ItemsContainer.CapacityLevel = WeightLevel;
        Backpack.ItemsContainer.AddCloselyContainer(1);
        UpdateModel();

        capacityBar.UpdateUI();
    }

    public void UpgradeSpeed()
    {
        SpeedLevel++;
        upgradeNotification.SpeedUpgrade();
        PlayerPrefs.SetInt(SpeedKey, SpeedLevel);

        Movement.MoveLevel = SpeedLevel;
        UpdateModel();

    }
    public void UpgradeMagnete()
    {
        MagnetLevel++;
        upgradeNotification.MagneteUpgrade();
        magneteSize.radius = 1 + MagnetLevel * 0.05f;
        PlayerPrefs.SetInt(MagnetKey, MagnetLevel);
        UpdateModel();

    }
}

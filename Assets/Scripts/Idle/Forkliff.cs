using System.Collections.Generic;
using UnityEngine;

public class Forkliff : Player.Player
{

    public int SpeedLevel;
    public int WeightLevel;

    const string SpeedKey = "SpeedKey";
    const string WeightKey = "WeightKey";

    public UpgradeNotification upgradeNotification;

    public List<Mesh> hullModels;
    public List<Mesh> handModels;

    [SerializeField] private MeshFilter hullMainModel;
    [SerializeField] private MeshFilter handMainModel;

    [SerializeField] private List<MeshFilter> wheelsMainModels;
    [SerializeField] private List<Transform> wheelsLvlThird;
    public Mesh wheelsFirstLevelMesh;
    public Mesh wheelsSecondLevelMesh;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        LoadProgress();
        UpdateModel();
        if (capacityBar != null && (LevelManager.Instance.currentVehicle is Forkliff))
        {
            capacityBar.UpdateUI(this);
            _backpack.ItemsContainer.editCountItems += capacityBar.UpdateUI;
        }
    }

    private void UpdateModel()
    {
        int index = WeightLevel / 9;
        hullMainModel.mesh = hullModels[index];
        handMainModel.mesh = handModels[index];

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
    }

    void LoadProgress()
    {
        SpeedLevel = PlayerPrefs.GetInt(SpeedKey, 0);
        WeightLevel = PlayerPrefs.GetInt(WeightKey, 0);
        Movement.MoveLevel = SpeedLevel;
        Backpack.ItemsContainer.CapacityLevel = WeightLevel;
        Backpack.ItemsContainer.Init();
    }

    public void UpgradeWeight()
    {
        WeightLevel++;
        upgradeNotification.WeightUpgrade();
        PlayerPrefs.SetInt(WeightKey, WeightLevel);

        Backpack.ItemsContainer.CapacityLevel = WeightLevel;
        Backpack.ItemsContainer.AddContainer(1);
        capacityBar.UpdateUI();
        UpdateModel();
    }

    public void UpgradeSpeed()
    {
        SpeedLevel++;
        upgradeNotification.SpeedUpgrade();
        PlayerPrefs.SetInt(SpeedKey, SpeedLevel);

        Movement.MoveLevel = SpeedLevel;
        UpdateModel();
    }

}

using UnityEngine;

public class Forkliff : Player.Player
{

    public int SpeedLevel;
    public int WeightLevel;

    const string SpeedKey = "SpeedKey";
    const string WeightKey = "WeightKey";

    public UpgradeNotification upgradeNotification;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        LoadProgress();
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
    }

    public void UpgradeSpeed()
    {
        SpeedLevel++;
        upgradeNotification.SpeedUpgrade();
        PlayerPrefs.SetInt(SpeedKey, SpeedLevel);

        Movement.MoveLevel = SpeedLevel;
    }

}

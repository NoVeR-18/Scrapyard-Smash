using UnityEngine;

public class Magnete : Player.Player
{
    public int SpeedLevel;
    public int WeightLevel;
    public int MagnetLevel;

    const string SpeedKey = "MagneteSpeedKey";
    const string WeightKey = "MagneteWeightKey";
    const string MagnetKey = "MagneteMagnetKey";

    public CapsuleCollider magneteSize;

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
        MagnetLevel = PlayerPrefs.GetInt(MagnetKey, 0);
        Movement.MoveLevel = SpeedLevel;
        magneteSize.radius = 1 + MagnetLevel * 0.05f;
        Backpack.ItemsContainer.CapacityLevel = WeightLevel;
        Backpack.ItemsContainer.InitClosely();
    }

    public void UpgradeWeight()
    {
        WeightLevel++;

        PlayerPrefs.SetInt(WeightKey, WeightLevel);

        Backpack.ItemsContainer.CapacityLevel = WeightLevel;
        Backpack.ItemsContainer.AddCloselyContainer(1);
    }

    public void UpgradeSpeed()
    {
        SpeedLevel++;

        PlayerPrefs.SetInt(SpeedKey, SpeedLevel);

        Movement.MoveLevel = SpeedLevel;
    }
    public void UpgradeMagnete()
    {
        MagnetLevel++;

        magneteSize.radius = 1 + MagnetLevel * 0.05f;
        PlayerPrefs.SetInt(MagnetKey, MagnetLevel);

    }
}

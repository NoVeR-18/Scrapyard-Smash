using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Helicopter : Player.Player
{
    private int Fuel;
    public int WeightLevel;
    private int MaxFuel = 50;
    public float FuelConsumptionRate = 0.5f;
    public float FuelRecoveryTime = 500f;
    public float RecoverySpeedMultiplier = 0.01f;
    public int RecoveryUpgradeLevel = 0;
    public int MaxFuelLevel = 0;

    public TextMeshPro fuelText;

    public Player.Player defaultVehickle;

    const string FuelKey = "HelicopterFuelKey";
    const string WeightKey = "HelicopterWeightKey";
    const string LastTimeKey = "HelicopterLastTimeKey";
    const string RecoverySpeedKey = "HelicopterRecoverySpeedKey";
    const string MaxFuelLevelKey = "MaxFuelLevelKey";

    private Coroutine _fuelConsumptionCoroutine;

    public UpgradeNotification upgradeNotification;

    [SerializeField] private List<Transform> helicopters;
    [SerializeField] private Transform rotor;



    [Header("Unlock Vehicle")]

    public bool unlocked = false;
    [SerializeField] private int _detailsToUnlock = 10;
    public int colectedDetails = 0;
    const string DetailsKey = "HelicopterDetailsKey";


    public override void Start()
    {
        _spawnPoint = transform.position;
        base.Start();
        LoadProgress();
        RecoverFuel();
        UpdateModel();

        CheckUnlocked();
    }
    public void ColectDetails()
    {
        colectedDetails++;
        PlayerPrefs.SetInt(DetailsKey, colectedDetails);
        PlayerPrefs.Save();
        CheckUnlocked();
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
        int index = MaxFuelLevel / 9;
        foreach (Transform t in helicopters)
        {
            t.gameObject.SetActive(false);
        }
        helicopters[index].gameObject.SetActive(true);
        if (index != 0)
            rotor.localPosition = new Vector3(rotor.localPosition.x, 2.2f, rotor.localPosition.z);
        else
            rotor.localPosition = new Vector3(rotor.localPosition.x, 1.45f, rotor.localPosition.z);
    }
    void LoadProgress()
    {
        Fuel = PlayerPrefs.GetInt(FuelKey, MaxFuel);
        WeightLevel = PlayerPrefs.GetInt(WeightKey, 0);
        RecoveryUpgradeLevel = PlayerPrefs.GetInt(RecoverySpeedKey, 0);
        MaxFuelLevel = PlayerPrefs.GetInt(MaxFuelLevelKey, 0);
        RecoverySpeedMultiplier += RecoveryUpgradeLevel * 0.002f;
        MaxFuel += MaxFuelLevel * 10;
        Backpack.ItemsContainer.CapacityLevel = WeightLevel;
        fuelText.text = $"{Fuel}/{MaxFuel}";

        Backpack.ItemsContainer.InitClosely();
    }

    void SaveProgress()
    {
        PlayerPrefs.SetInt(FuelKey, Fuel);
        PlayerPrefs.SetInt(WeightKey, WeightLevel);
        PlayerPrefs.SetInt(RecoverySpeedKey, RecoveryUpgradeLevel);
        PlayerPrefs.SetString(LastTimeKey, DateTime.Now.ToString());
    }

    void RecoverFuel()
    {
        if (PlayerPrefs.HasKey(LastTimeKey))
        {
            DateTime lastTime = DateTime.Parse(PlayerPrefs.GetString(LastTimeKey));
            TimeSpan timeAway = DateTime.Now - lastTime;

            float recoveredFuel = (float)timeAway.TotalSeconds / FuelRecoveryTime * MaxFuel * RecoverySpeedMultiplier;
            Fuel = Mathf.Clamp(Fuel + Mathf.RoundToInt(recoveredFuel), 0, MaxFuel);

            fuelText.text = $"{Fuel}/{MaxFuel}";
        }
    }

    public override bool EnableVechicle()
    {
        if (unlocked)
        {
            if (Fuel > 0)
            {
                _movement.CanMoving = true;
                _movement.boxCollider.isTrigger = true;
                mainCamera.gameObject.SetActive(true);
                vehicleZone?.gameObject.SetActive(false);
                StartCoroutine(MoveUpSmoothly(new Vector3(_spawnPoint.x, _spawnPoint.y + 12f, _spawnPoint.z)));
                _fuelConsumptionCoroutine = StartCoroutine(ConsumeFuel());
                return true;

            }
            else
            {
                Debug.LogWarning("Не хватает топлива для использования вертолета!");
                return false;
            }
        }
        else
        { return false; }
    }

    public override void DisableVechicle()
    {
        _movement.CanMoving = false;
        mainCamera.gameObject.SetActive(false);
        vehicleZone?.gameObject.SetActive(true);
        _movement.boxCollider.isTrigger = true;
        StartCoroutine(MoveUpSmoothly(_spawnPoint));

        if (_fuelConsumptionCoroutine != null)
        {
            StopCoroutine(_fuelConsumptionCoroutine);
            _fuelConsumptionCoroutine = null;
        }
    }

    private IEnumerator ConsumeFuel()
    {
        while (Fuel > 0)
        {
            Fuel--;

            fuelText.text = $"{Fuel}/{MaxFuel}";
            PlayerPrefs.SetInt(FuelKey, Fuel);
            yield return new WaitForSeconds(FuelConsumptionRate);
        }

        Debug.Log("Топливо закончилось!");
        DisableVechicle();

        defaultVehickle?.EnableVechicle();
    }

    private void FixedUpdate()
    {
        if (!_movement.CanMoving && Fuel < MaxFuel)
        {
            float recoveryRate = Time.deltaTime / FuelRecoveryTime * MaxFuel * RecoverySpeedMultiplier;
            Fuel += Mathf.RoundToInt(recoveryRate);
            Fuel = Mathf.Clamp(Fuel, 0, MaxFuel);

            fuelText.text = $"{Fuel}/{MaxFuel}";
        }
    }

    private void OnApplicationQuit()
    {
        SaveProgress();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveProgress();
        }
    }

    public void UpgradeWeight()
    {
        WeightLevel++;
        upgradeNotification.WeightUpgrade();
        PlayerPrefs.SetInt(WeightKey, WeightLevel);
        Backpack.ItemsContainer.CapacityLevel = WeightLevel;
        Backpack.ItemsContainer.AddCloselyContainer(1);

        UpdateModel();
    }

    public void UpgradeFuel()
    {
        MaxFuelLevel++;
        Fuel += 10;
        MaxFuel += MaxFuelLevel * 10;
        upgradeNotification.FuelUpgrade();
        fuelText.text = $"{Fuel}/{MaxFuel}";
        PlayerPrefs.SetInt(FuelKey, Fuel);
        PlayerPrefs.SetInt(MaxFuelLevelKey, MaxFuelLevel);

        UpdateModel();
    }

    public void UpgradeRecoverySpeed()
    {
        RecoveryUpgradeLevel++;
        upgradeNotification.RefilUpgrade();
        RecoverySpeedMultiplier += 0.002f;
        PlayerPrefs.SetInt(RecoverySpeedKey, RecoveryUpgradeLevel);
    }

    private IEnumerator MoveUpSmoothly(Vector3 poss, float duration = 2f)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, poss, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = poss;
    }
}

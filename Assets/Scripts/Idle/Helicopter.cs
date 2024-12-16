using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Helicopter : Player.Player
{
    public int Fuel;
    public int WeightLevel;
    public int MaxFuel = 50;
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
    [SerializeField] private Transform modelLock;
    [SerializeField] private Transform modelAssemble;
    [SerializeField] private List<Transform> helicopters;
    [SerializeField] private Transform rotor;

    [SerializeField] private List<BoxCollider> phisicColiders;

    public Transform heliFuel;

    [Header("Unlock Vehicle")]

    public bool unlocked = false;
    [SerializeField] private int _detailsToUnlock = 10;
    public int colectedDetails = 0;
    const string DetailsKey = "HelicopterDetailsKey";

    public Transform partNotification;
    public TextMeshProUGUI partsCount;
    public Image partsFill;

    [SerializeField] private BoxCollider boxCollider;

    public override void Start()
    {
        _spawnPoint = transform.position;
        base.Start();
        LoadProgress();
        RecoverFuel();
        UpdateModel();

        foreach (var item in phisicColiders)
        {
            item.enabled = false;
        }
        CheckUnlocked();
    }
    public void ColectDetails()
    {
        colectedDetails++;
        PlayerPrefs.SetInt(DetailsKey, colectedDetails);
        PlayerPrefs.Save();
        CheckUnlocked();

        StartCoroutine(closeTab());
        UpdateModel();
    }
    private IEnumerator closeTab()
    {

        partNotification.gameObject.SetActive(true);
        partsCount.text = $"{colectedDetails}/{_detailsToUnlock}";
        partsFill.fillAmount = (float)colectedDetails / _detailsToUnlock;
        yield return new WaitForSeconds(1.0f);
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
        foreach (Transform t in helicopters)
        {
            t.gameObject.SetActive(false);
        }
        modelLock.gameObject.SetActive(false);
        modelAssemble.gameObject.SetActive(false);
        if (colectedDetails == 0)
        {
            modelLock.gameObject.SetActive(true);
            rotor.gameObject.SetActive(false);
            return;
        }
        else if (colectedDetails < _detailsToUnlock)
        {
            rotor.gameObject.SetActive(false);
            modelAssemble.gameObject.SetActive(true);
            return;
        }
        int index = MaxFuelLevel / 9;
        helicopters[index].gameObject.SetActive(true);
        _movement.modelContainer = helicopters[index];
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
        colectedDetails = PlayerPrefs.GetInt(DetailsKey, 0);

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
            if (Fuel > 2)
            {
                boxCollider.enabled = true;
                _movement.CanMoving = true;
                _movement.boxCollider.isTrigger = true;
                mainCamera.gameObject.SetActive(true);
                vehicleZone?.gameObject.SetActive(false);
                StartCoroutine(MoveUpSmoothly(new Vector3(_spawnPoint.x, _spawnPoint.y + 12f, _spawnPoint.z)));
                _fuelConsumptionCoroutine = StartCoroutine(ConsumeFuel());
                capacityBar.UpdateUI(this);
                capacityBar.UpdateHelicopterUI();
                foreach (var item in phisicColiders)
                {
                    item.enabled = true;
                }
                heliFuel.gameObject.SetActive(false);
                LevelManager.Instance.currentVehicle = this;
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
        boxCollider.enabled = false;
        _movement.CanMoving = false;
        foreach (var item in phisicColiders)
        {
            item.enabled = false;
        }
        mainCamera.gameObject.SetActive(false);
        vehicleZone?.gameObject.SetActive(true);
        _movement.boxCollider.isTrigger = true;
        StartCoroutine(MoveUpSmoothly(_spawnPoint));
        while (_backpack.ItemsContainer.Count > 0)
        {

            Item item;
            _backpack.ItemsContainer.TakeItem(out item);
            item.gameObject.transform.parent = null;

            item.CanTake = true;
            item.phisicCollider.enabled = true;
            item.AddComponent<Rigidbody>();
        }

        heliFuel.gameObject.SetActive(true);
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
            capacityBar.UpdateHelicopterUI();
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

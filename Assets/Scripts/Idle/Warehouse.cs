using System;
using System.Linq;
using UnityEngine;

public class Warehouse : MonoBehaviour
{
    public StoragePad OutputStoragePad
    {
        get
        {
            return _storagePads.Where(x => x.Type == StorageBuildingType.Output).First();
        }
    }

    [SerializeField] private StoragePad[] _storagePads;
    [SerializeField] private float _timeToProduce;
    private BuildingProductionTimer _productionTimer = new BuildingProductionTimer();
    public BuildingProductionTimer ProductionTimer { get => _productionTimer; }

    private void OnDestroy()
    {
        _productionTimer.Stop();
    }

    [SerializeField] private Item ItemPrefab;
    [Space]
    [SerializeField] private Transform _outputPoint;


    private void Start()
    {
        //ProductionTimer.TickTime = _timeToProduce;
        //ProductionTimer.Ticked += produce;
        //ProductionTimer.Start();
    }

    bool CanCreaft()
    {
        if (OutputStoragePad.ItemsContainer.CanAddItem() == false)
            return false;
        else return true;
    }
    public void produce()
    {
        if (ItemPrefab == null)
            return;

        if (OutputStoragePad.ItemsContainer.CanAddItem() == false)
            return;

        Item newItem = CreateItem(ItemPrefab);
        newItem.transform.position = _outputPoint.position;

        if (OutputStoragePad.ItemsContainer.AddItem(newItem) == false)
        {
            newItem.Disappear();
            return;
        }

    }

    public Item CreateItem(Item ItemPrefab)
    {
        if (ItemPrefab == null)
        {
            throw new NullReferenceException($"FrameMachineTab: Can't find item ");
        }

        return instantiateItem(ItemPrefab);
    }
    private Item instantiateItem(Item item)
    {
        Item instance = Instantiate(item);
        instance.transform.position = Vector3.zero;
        return instance;
    }
}

using Items;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SellPad : StoragePad
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private List<ItemType> itemsTypeTaken;
    public StoragePad OutputStoragePads;
    [SerializeField] private Truck _truck;
    private void Start()
    {
        _truck.ItemsCount.text = $"{ItemsContainer.Count}/{ItemsContainer.Capacity}";

    }
    public override void CloseInteract()
    {
        base.CloseInteract();
        //_truck.Animated();
        //Sell();
        text.text = "SELL";
    }
    void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Interact(collision.gameObject.GetComponent<Player.Player>());
        }
    }
    public override void Interact(Player.Player player)
    {
        if (_truck.canSell)
        {
            base.Interact(player);
            _truck.ItemsCount.text = $"{ItemsContainer.Count}/{ItemsContainer.Capacity}";
            if (ItemsContainer.IsFull)
            {
                _truck.Animated();
                Sell();
            }
        }

    }

    public override void takeItemsFromPlayer(Player.Player player)
    {
        if (!ItemsContainer.CanAddItem())
            return;

        Item takenItem = null;
        foreach (var itemType in itemsTypeTaken)
        {
            if (player.Backpack.ItemsContainer.TakeItem(out takenItem, itemType)) { break; }
        }
        if (takenItem != null)
            ItemsContainer.AddItem(takenItem);
    }
    private void Sell()
    {
        while (true)
        {
            if (!OutputStoragePads.ItemsContainer.CanAddItem())
                break;
            if (!ItemsContainer.CanTakeItem())
                break;


            if (Type == StorageBuildingType.Input)
            {
                Item takenItem;
                if (!ItemsContainer.TakeItem(out takenItem))
                    return;
                takenItem.Disappear();
                Item newItem = CreateItem();
                if (OutputStoragePads.ItemsContainer.AddItem(newItem) == false)
                {
                    newItem.Disappear();
                    return;
                }
                newItem.transform.position = OutputStoragePads.transform.position;
            }
        }
    }
}

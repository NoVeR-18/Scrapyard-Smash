using Items;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SellPad : StoragePad
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private List<ItemType> itemsTypeTaken;
    public StoragePad OutputStoragePads;
    public float timeToSell = 2f;

    public override void OnTriggerEnter(Collider other)
    {
    }
    public override void OnTriggerExit(Collider other)
    {
    }

    public IEnumerator Sell()
    {
        yield return new WaitForSeconds(timeToSell);
        if (!OutputStoragePads.ItemsContainer.CanAddItem())
            StopCoroutine(Sell());
        if (!ItemsContainer.CanTakeItem())
            StopCoroutine(Sell());



        if (Type == StorageBuildingType.Input)
        {
            Item takenItem;
            if (!ItemsContainer.TakeItem(out takenItem))
                StopCoroutine(Sell());
            takenItem.Disappear();
            Item newItem = CreateItem();
            if (OutputStoragePads.ItemsContainer.AddItem(newItem) == false)
            {
                newItem.Disappear();
                StopCoroutine(Sell());
            }
            newItem.transform.position = OutputStoragePads.transform.position;
        }
    }
}


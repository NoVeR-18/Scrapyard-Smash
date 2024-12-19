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

    public Animator animator;
    public ParticleSystem particle;

    [SerializeField] private Transform _outputPoint;

    public override void OnTriggerEnter(Collider other)
    {
    }
    public override void OnTriggerExit(Collider other)
    {
    }

    public IEnumerator Sell()
    {
        animator?.SetTrigger("Crash");
        yield return new WaitForSeconds(timeToSell / 2);
        if (!OutputStoragePads.ItemsContainer.CanAddItem())
            StopCoroutine(Sell());
        particle?.Play();
        ItemsContainer.gameObject.SetActive(false);


        if (Type == StorageBuildingType.Input)
        {
            Item takenItem;
            if (!ItemsContainer.TakeItem(out takenItem))
                StopCoroutine(Sell());
            var cost = takenItem.Cost;
            takenItem.Disappear();
            Item newItem = CreateItem();
            newItem.transform.position = _outputPoint.transform.position;
            if (OutputStoragePads.ItemsContainer.AddItem(newItem) == false)
            {
                newItem.Disappear();
                StopCoroutine(Sell());
            }
            newItem.Cost = cost;

        }
        ItemsContainer.gameObject.SetActive(true);
        if (!ItemsContainer.CanTakeItem())
            StopCoroutine(Sell());
    }
}


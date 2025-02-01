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
    private Queue<Item> _sellQueue = new Queue<Item>();
    private bool _isSelling = false;

    public override void OnTriggerEnter(Collider other)
    {
    }
    public override void OnTriggerExit(Collider other)
    {
    }

    public void AddToSellQueue(Item item)
    {
        _sellQueue.Enqueue(item);
        if (!_isSelling)
        {
            StartCoroutine(Sell());
        }
    }
    public IEnumerator Sell()
    {
        animator?.SetTrigger("Crash");
        yield return new WaitForSeconds(2f);
        if (!OutputStoragePads.ItemsContainer.CanAddItem())
            yield break;
        if (!ItemsContainer.CanTakeItem())
            yield break;


        particle?.Play();
        ItemsContainer.gameObject.SetActive(false);
        if (Type == StorageBuildingType.Input)
        {
            Item takenItem;
            if (!ItemsContainer.TakeItem(out takenItem))
                yield break;
            var cost = takenItem.Cost;
            takenItem.Disappear();
            Item newItem = CreateItem();
            newItem.Cost = cost;
            if (OutputStoragePads.ItemsContainer.AddItem(newItem) == false)
            {
                newItem.Disappear();
                yield break;
            }
            newItem.transform.position = OutputStoragePads.transform.position;
            ItemsContainer.gameObject.SetActive(true);
        }
    }
    public IEnumerator SellDebris()
    {
        animator?.SetTrigger("Crash");
        yield return new WaitForSeconds(timeToSell / 2);
        if (!OutputStoragePads.ItemsContainer.CanAddItem())
            yield break;
        particle?.Play();
        ItemsContainer.gameObject.SetActive(false);


        if (Type == StorageBuildingType.Input)
        {
            Item takenItem;
            if (!ItemsContainer.TakeItem(out takenItem))
                yield break;
            var cost = takenItem.Cost;
            takenItem.Disappear();
            Item newItem = CreateItem();
            newItem.transform.position = _outputPoint.transform.position;
            if (OutputStoragePads.ItemsContainer.AddItem(newItem) == false)
            {
                newItem.Disappear();
                yield break;
            }
            newItem.Cost = cost;

        }
        ItemsContainer.gameObject.SetActive(true);
        if (!ItemsContainer.CanTakeItem())
            yield break;
    }
    //private IEnumerator Sell()
    //{
    //    _isSelling = true;
    //    while (_sellQueue.Count > 0)
    //    {
    //        animator?.SetTrigger("Crash");
    //        yield return new WaitForSeconds(timeToSell / 2);

    //        if (!OutputStoragePads.ItemsContainer.CanAddItem())
    //        {
    //            _isSelling = false;
    //            yield break;
    //        }

    //        particle?.Play();
    //        ItemsContainer.gameObject.SetActive(false);

    //        if (Type == StorageBuildingType.Input)
    //        {
    //            Item takenItem = _sellQueue.Dequeue();

    //            ItemsContainer.TakeItem(takenItem);
    //            var cost = takenItem.Cost;
    //            takenItem.Disappear();

    //            Item newItem = CreateItem();
    //            newItem.transform.position = _outputPoint.transform.position;
    //            newItem.Cost = cost;

    //            if (!OutputStoragePads.ItemsContainer.AddItem(newItem))
    //            {
    //                newItem.Disappear();
    //            }

    //        }

    //        ItemsContainer.gameObject.SetActive(true);
    //        yield return new WaitForSeconds(timeToSell / 2);
    //    }
    //    _isSelling = false;
    //}
}

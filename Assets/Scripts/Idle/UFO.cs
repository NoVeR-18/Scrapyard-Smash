using System.Collections.Generic;
using UnityEngine;

public class UFO : Player.Player
{
    public ParticleSystem CowPartickle;
    public Transform ufoCash;
    public override void Start()
    {
        base.Start();
        LoadProgress();
    }

    void LoadProgress()
    {
        Backpack.ItemsContainer.InitClosely();
    }

    //private void FixedUpdate()
    //{
    //    if (Backpack.ItemsContainer != null)
    //    {
    //        if (Backpack.ItemsContainer.Count > 0)
    //        {
    //            Invoke("Sell", 1f);
    //        }
    //    }
    //}

    public void Sell(Item takenItem)
    {
        if (!Backpack.ItemsContainer.TakeItem(takenItem))
            return;
        LevelManager.Instance.wallet.AddMoney(takenItem.Cost);
        if (takenItem.Type == Items.ItemType.Car)
        {
            LevelManager.Instance.CarsCollected++;
            if (takenItem.name == "cow")
                Instantiate(CowPartickle, ufoCash);
            else
                Instantiate(CarPartickle, ufoCash);
        }
        if (takenItem.Type == Items.ItemType.Trash)
        {
            LevelManager.Instance.TrashCollected++;
            Instantiate(TrashPartickle, ufoCash);
        }
        LevelManager.Instance.CheckWining();
        takenItem.Disappear();

    }
    private Dictionary<Item, float> itemTimers = new Dictionary<Item, float>();
    private const float SellDelay = 1f;

    private void FixedUpdate()
    {
        // Создаём список для объектов, которые нужно продать
        List<Item> itemsToSell = new List<Item>();

        // Обходим копию ключей словаря, чтобы избежать модификации во время итерации
        foreach (var item in new List<Item>(itemTimers.Keys))
        {
            itemTimers[item] -= Time.fixedDeltaTime;

            // Если таймер истёк, добавляем объект в список для продажи
            if (itemTimers[item] <= 0)
            {
                itemsToSell.Add(item);
            }
        }

        // Продаём объекты, у которых истёк таймер
        foreach (var item in itemsToSell)
        {
            itemTimers.Remove(item); // Удаляем объект из таймеров
            Sell(item);
        }

        // Проверяем новые объекты в Backpack.ItemsContainer
        foreach (var item in Backpack.ItemsContainer.Items)
        {
            if (!itemTimers.ContainsKey(item))
            {
                // Добавляем объект в таймеры, если его ещё нет
                itemTimers[item] = SellDelay;
            }
        }
    }

}

using System.Collections;
using System.Linq;
using UnityEngine;

public class FrameMachineTab : StoragePad
{
    public SellPad sellPad;
    private void OnTriggerStay(UnityEngine.Collider other)
    {
        if (other.tag == "Player")
        {
            base.Interact(other.gameObject.GetComponent<Player.Player>());
        }
    }
    public override void takeItemsFromPlayer(Player.Player player)
    {
        if (player == null)
            return;
        if (!ItemsContainer.CanAddItem())
            return;

        Item takenItem = null;
        if (!player.Backpack.ItemsContainer.TakeItem(out takenItem, ItemType))
            return;

        DailyTasksManager.Instance.PerformTask(TaskType.CollectCar);
        GameManager.Instance.Vibrate();
        ItemsContainer.AddItem(takenItem);
        takenItem.spawnTime = Time.time;
        Destroy(takenItem.GetComponent<BoxCollider>());
        LevelManager.Instance.CarsCollected++;
        LevelManager.Instance.CheckWining();
    }

    private IEnumerator DelayAndSell(Item item)
    {
        while (ItemsContainer.Count > 0)
        {
            yield return new WaitForSeconds(0.5f); // Ждём полсекунды

            if (sellPad.ItemsContainer.CanAddItem())
            {
                //item.gameObject.transform.localScale /= 2;
                sellPad.ItemsContainer.AddItem(item);
                sellPad.ItemsContainer.TakeItem(out Item sellingItem);
                sellPad.AddToSellQueue(sellingItem);
            }
        }
    }
    private void FixedUpdate()
    {
        if (ItemsContainer.Count > 0)
        {
            Item firstItem = ItemsContainer.GetItems().First(); // Получаем предмет, но не удаляем его
            if (firstItem != null && Time.time - firstItem.spawnTime > 0.5f) // Даем 0.5 секунды на перемещение
            {
                if (sellPad.ItemsContainer.CanAddItem())
                {
                    Item item;
                    if (ItemsContainer.TakeItem(out item))
                    {
                        item.gameObject.transform.localScale /= 2;
                        sellPad.ItemsContainer.AddItem(item);
                        sellPad.AddToSellQueue(item);
                    }
                }
            }
        }
    }
}

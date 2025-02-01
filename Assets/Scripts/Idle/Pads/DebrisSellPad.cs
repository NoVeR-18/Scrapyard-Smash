using UnityEngine;

public class DebrisSellPad : StoragePad
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
        DailyTasksManager.Instance.PerformTask(TaskType.CollectTrash);
        GameManager.Instance.Vibrate();
        Destroy(takenItem.GetComponent<BoxCollider>());
        ItemsContainer.AddItem(takenItem);
        LevelManager.Instance.TrashCollected++;
        LevelManager.Instance.CheckWining();
    }
    private void FixedUpdate()
    {
        if (ItemsContainer.Count > 0 && sellPad.ItemsContainer.CanAddItem())
        {
            Item item;
            ItemsContainer.TakeItem(out item);
            sellPad.ItemsContainer.AddItem(item);
            //sellPad.AddToSellQueue(item);
            sellPad.StartCoroutine(sellPad.SellDebris());
        }
    }
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

    }
}

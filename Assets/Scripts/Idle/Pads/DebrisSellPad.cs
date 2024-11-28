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
        if (!ItemsContainer.CanAddItem())
            return;

        Item takenItem = null;
        if (!player.Backpack.ItemsContainer.TakeItem(out takenItem, ItemType))
            return;

        ItemsContainer.AddItem(takenItem);
        LevelManager.Instance.CarsOnScene--;
        LevelManager.Instance.CheckWining();
    }
    private void FixedUpdate()
    {
        if (ItemsContainer.Count > 0 && !sellPad.ItemsContainer.IsFull)
        {
            Item item;
            ItemsContainer.TakeItem(out item);
            sellPad.ItemsContainer.AddItem(item);
            sellPad.StartCoroutine(sellPad.Sell());
        }
    }
}

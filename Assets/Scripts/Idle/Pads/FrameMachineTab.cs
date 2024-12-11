
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
        GameManager.Instance.Vibrate();
        ItemsContainer.AddItem(takenItem);
        LevelManager.Instance.CarsCollected++;
        LevelManager.Instance.CheckWining();
    }
    private void FixedUpdate()
    {
        if (ItemsContainer.Count > 0 && !sellPad.ItemsContainer.IsFull)
        {
            Item item;
            ItemsContainer.TakeItem(out item);
            item.gameObject.transform.localScale /= 2;
            sellPad.ItemsContainer.AddItem(item);

            sellPad.StartCoroutine(sellPad.Sell());
        }
    }
}

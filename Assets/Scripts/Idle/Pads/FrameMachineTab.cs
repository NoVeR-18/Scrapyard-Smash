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

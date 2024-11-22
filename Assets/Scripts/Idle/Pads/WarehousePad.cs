using System.Collections;
using TMPro;
using UnityEngine;

public class WarehousePad : StoragePad
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private Warehouse warehouse;

    public override void CloseInteract()
    {
        base.CloseInteract();
        text.text = "WAREHOUSE";
        warehouse.ProductionTimer.Stop();
    }
    void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            base.Interact(collision.gameObject.GetComponent<Player.Player>());
        }
    }
    public override void Interact(Player.Player player)
    {
        base.Interact(player);
    }
    public override void giveItemsToPlayer(Player.Player player)
    {
        if (ItemsContainer.Count == 0)
            return;


        Item takenItem = null;
        if (!ItemsContainer.TakeItem(out takenItem))
            return;

    }
    IEnumerator Timer()
    {
        text.text = 3.ToString();
        yield return new WaitForSeconds(1f);
        text.text = 2.ToString();
        yield return new WaitForSeconds(1f);
        text.text = 1.ToString();
        yield return new WaitForSeconds(1f);
        text.text = "WAREHOUSE";
    }
}

using Items;
using TMPro;
using UnityEngine;

public class MoneyPad : StoragePad
{
    [SerializeField] private TextMeshPro text;

    public override void CloseInteract()
    {
        base.CloseInteract();
        text.text = "MONEY";
    }
    void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            base.Interact(collision.gameObject.GetComponent<Player.Player>());
        }
    }

    public override void giveItemsToPlayer(Player.Player player)
    {
        base.giveItemsToPlayer(player);
        Item moneyItem = null;

        if (player.Backpack.ItemsContainer.TakeItem(out moneyItem, ItemType.Money))
        {
            if (moneyItem != null)
            {
                player.Wallet.AddMoney(1);
                GameManager.Instance.Vibrate();
                moneyItem.Disappear();
                Debug.Log("MoneyAdded");
            }
        }
    }
}

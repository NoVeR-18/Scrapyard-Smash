using Items.Container;
using UnityEngine;

public class GameAward : StoragePad
{
    [Space]
    [SerializeField] private Transform _outputPoint;
    void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            base.Interact(collision.gameObject.GetComponent<Player.Player>());
        }
    }
    public override void CloseInteract()
    {
    }
    public override void giveItemsToPlayer(Player.Player player)
    {
        if (player == null)
            return;
        if (ItemsContainer.Count == 0)
            return;


        Item takenItem = null;
        if (!ItemsContainer.TakeItem(out takenItem))
            return;
        LevelManager.Instance.wallet.AddMoney(takenItem.Cost);
        Destroy(takenItem.gameObject);
    }
    bool CanCreaft()
    {
        if (ItemsContainer.CanAddItem() == false)
            return false;
        else return true;
    }
    private void produce()
    {
        if (ItemsContainer.CanAddItem() == false)
            return;

        Item newItem = CreateItem();

        if (ItemsContainer.AddItem(newItem) == false)
        {
            newItem.Disappear();
            return;
        }

        newItem.transform.position = _outputPoint.position;
    }

}

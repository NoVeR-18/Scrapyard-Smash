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
        GameManager.Instance.Vibrate();
        LevelManager.Instance.wallet.CollectMoney(takenItem.transform.position);

        Destroy(takenItem.gameObject);
    }
}

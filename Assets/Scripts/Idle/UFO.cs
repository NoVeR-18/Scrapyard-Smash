using TMPro;
using UnityEngine;

public class UFO : Player.Player
{
    const string ColectedCashTriger = "GetCash";

    [SerializeField] private TextMeshPro MoneyCount;
    [SerializeField] private Animator CashColectedAnimator;
    public override void Start()
    {
        base.Start();
        LoadProgress();
    }

    void LoadProgress()
    {
        Backpack.ItemsContainer.InitClosely();
    }

    private void FixedUpdate()
    {
        if (Backpack.ItemsContainer != null)
        {
            if (Backpack.ItemsContainer.Count > 0)
            {
                Invoke("Sell", 0.25f);
            }
        }
    }

    public void Sell()
    {
        Item takenItem;
        if (!Backpack.ItemsContainer.TakeItem(out takenItem))
            return;
        LevelManager.Instance.wallet.AddMoney(takenItem.Cost);
        PlayAnimation(takenItem.Cost);
        if (takenItem.Type == Items.ItemType.Car)
            LevelManager.Instance.CarsCollected++;
        if (takenItem.Type == Items.ItemType.Trash)
            LevelManager.Instance.TrashCollected++;
        LevelManager.Instance.CheckWining();
        takenItem.Disappear();

    }

    private void PlayAnimation(int CashCount)
    {
        CashColectedAnimator.SetTrigger(ColectedCashTriger);
        MoneyCount.text = CashCount.ToString();
    }
}

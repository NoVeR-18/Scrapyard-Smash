public class UFO : Player.Player
{

    // Start is called before the first frame update
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
                Sell();
            }
        }
    }

    public void Sell()
    {
        Item takenItem;
        if (!Backpack.ItemsContainer.TakeItem(out takenItem))
            return;
        LevelManager.Instance.wallet.AddMoney(takenItem.Cost);
        if (takenItem.Type == Items.ItemType.Car)
            LevelManager.Instance.CarsCollected++;
        if (takenItem.Type == Items.ItemType.Trash)
            LevelManager.Instance.TrashCollected++;
        LevelManager.Instance.CheckWining();
        takenItem.Disappear();

    }
}

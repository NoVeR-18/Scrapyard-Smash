using UnityEngine;

public class UFO : Player.Player
{
    public ParticleSystem CowPartickle;
    public Transform ufoCash;
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
        if (takenItem.Type == Items.ItemType.Car)
        {
            LevelManager.Instance.CarsCollected++;
            if (takenItem.name == "cow")
                Instantiate(CowPartickle, ufoCash);
            else
                Instantiate(CarPartickle, ufoCash);
        }
        if (takenItem.Type == Items.ItemType.Trash)
        {
            LevelManager.Instance.TrashCollected++;
            Instantiate(TrashPartickle, ufoCash);
        }
        LevelManager.Instance.CheckWining();
        takenItem.Disappear();

    }
}

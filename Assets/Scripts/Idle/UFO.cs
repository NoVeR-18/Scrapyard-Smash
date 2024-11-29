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

}

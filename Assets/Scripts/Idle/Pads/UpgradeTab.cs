using UnityEngine;

public class UpgradeTab : StoragePad
{
    [SerializeField] private UpgradePanel upgradePanel;
    public override void Interact(Player.Player player)
    {
        upgradePanel.gameObject.SetActive(true);
        upgradePanel.OpenTab(player);
    }

    public override void CloseInteract()
    {
        upgradePanel.gameObject.SetActive(false);
    }

}

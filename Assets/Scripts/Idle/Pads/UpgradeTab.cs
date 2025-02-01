using UnityEngine;

public class UpgradeTab : StoragePad
{
    [SerializeField] private UpgradePanel upgradePanel;
    public override void Interact(Player.Player player)
    {
        upgradePanel.gameObject.SetActive(true);
        upgradePanel.OpenTab();
    }
    public override void CloseInteract()
    {
        if (upgradePanel.TabIsOpen)
            return;
        base.CloseInteract();

        upgradePanel.CloseTab();
    }
}

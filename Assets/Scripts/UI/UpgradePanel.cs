using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    public ForkliffUpgradeTab forkliffUpgradeTab;
    public MagneteUpgradeTab magneteUpgradeTab;

    public Button forkliffOpenButton;
    public Button magneteOpenButton;

    private void Start()
    {
        forkliffOpenButton.onClick.AddListener(() => { OpenForkliffTab(); });
        magneteOpenButton.onClick.AddListener(() => { OpenMagneteTab(); });
    }

    public void OpenTab(Player.Player player)
    {
        if (player as Magnete)
        {
            OpenMagneteTab();
        }
        else if (player as Forkliff)
        {
            OpenForkliffTab();
        }
    }


    private void OpenMagneteTab()
    {
        magneteUpgradeTab.gameObject.SetActive(true);
        forkliffUpgradeTab.gameObject.SetActive(false);
    }

    private void OpenForkliffTab()
    {
        magneteUpgradeTab.gameObject.SetActive(false);
        forkliffUpgradeTab.gameObject.SetActive(true);
    }


}

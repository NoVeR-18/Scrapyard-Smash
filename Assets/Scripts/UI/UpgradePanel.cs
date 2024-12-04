using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    public ForkliffUpgradeTab forkliffUpgradeTab;
    public MagneteUpgradeTab magneteUpgradeTab;
    public HelicopterUpgradeTab helicopterUpgradeTab;

    public Button[] forkliffOpenButton;
    public Button[] magneteOpenButton;
    public Button[] helicopterOpenButton;

    public Button closeButton;
    private void Start()
    {
        foreach (var item in forkliffOpenButton)
            item.onClick.AddListener(() => { OpenForkliffTab(); });
        foreach (var item in magneteOpenButton)
            item.onClick.AddListener(() => { OpenMagneteTab(); });
        foreach (var item in helicopterOpenButton)
            item.onClick.AddListener(() => { OpenHelicopterTab(); });

        closeButton.onClick.AddListener(() => { CloseTab(); });
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
        else if (player as Helicopter)
        {
            OpenHelicopterTab();
        }
    }


    public void CloseTab()
    {
        gameObject.SetActive(false);
    }
    private void OpenMagneteTab()
    {
        forkliffUpgradeTab.gameObject.SetActive(false);
        helicopterUpgradeTab.gameObject.SetActive(false);
        magneteUpgradeTab.gameObject.SetActive(true);
    }

    private void OpenForkliffTab()
    {
        magneteUpgradeTab.gameObject.SetActive(false);
        helicopterUpgradeTab.gameObject.SetActive(false);
        forkliffUpgradeTab.gameObject.SetActive(true);
    }
    private void OpenHelicopterTab()
    {
        magneteUpgradeTab.gameObject.SetActive(false);
        forkliffUpgradeTab.gameObject.SetActive(false);
        helicopterUpgradeTab.gameObject.SetActive(true);
    }


}

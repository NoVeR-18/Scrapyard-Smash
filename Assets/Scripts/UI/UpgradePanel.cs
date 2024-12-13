using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    public ForkliffUpgradeTab forkliffUpgradeTab;
    public MagneteUpgradeTab magneteUpgradeTab;
    public HelicopterUpgradeTab helicopterUpgradeTab;

    public ChangeVehicleTab changeVehicleTab;

    public Button[] forkliffOpenButton;
    public Button[] magneteOpenButton;
    public Button[] helicopterOpenButton;

    public Button closeButton;

    public AudioSource audioSource;

    private void Start()
    {
        foreach (var item in forkliffOpenButton)
            item.onClick.AddListener(() =>
            {
                OpenForkliffTab();
                audioSource?.Play();
            });
        foreach (var item in magneteOpenButton)
            item.onClick.AddListener(() =>
            {
                OpenMagneteTab();
                audioSource?.Play();
            });
        foreach (var item in helicopterOpenButton)
            item.onClick.AddListener(() =>
            {
                OpenHelicopterTab();
                audioSource?.Play();
            });

        closeButton.onClick.AddListener(() =>
        {
            CloseTab();
            audioSource?.Play();
        });
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
        if (!magneteUpgradeTab.magnete.unlocked) { foreach (var item in magneteOpenButton) { item.gameObject.SetActive(false); } }
        else { foreach (var item in magneteOpenButton) { item.gameObject.SetActive(true); } }

        if (!helicopterUpgradeTab.helicopter.unlocked) { foreach (var item in helicopterOpenButton) { item.gameObject.SetActive(false); } }
        else { foreach (var item in helicopterOpenButton) { item.gameObject.SetActive(true); } }

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
        changeVehicleTab.SelectMagnete();
    }

    private void OpenForkliffTab()
    {
        magneteUpgradeTab.gameObject.SetActive(false);
        helicopterUpgradeTab.gameObject.SetActive(false);
        forkliffUpgradeTab.gameObject.SetActive(true);
        changeVehicleTab.SelectForkliff();
    }
    private void OpenHelicopterTab()
    {
        magneteUpgradeTab.gameObject.SetActive(false);
        forkliffUpgradeTab.gameObject.SetActive(false);
        helicopterUpgradeTab.gameObject.SetActive(true);
    }
}

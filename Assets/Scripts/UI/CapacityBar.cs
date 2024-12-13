using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CapacityBar : MonoBehaviour
{
    public TextMeshProUGUI count;
    public Image fillAmount;

    public Transform helicopterIcon;
    public Transform forkliffIcon;
    public Transform magneticIcon;
    private Player.Player player;
    public void UpdateUI()
    {
        if (player != null)
        {

            count.text = $"{player.Backpack.ItemsContainer.Count}/{player.Backpack.ItemsContainer.Capacity}";
            fillAmount.fillAmount = player.Backpack.ItemsContainer.Count / player.Backpack.ItemsContainer.Capacity;
        }

    }
    public void UpdateUI(Player.Player player)
    {
        this.player = player;

        helicopterIcon.gameObject.SetActive(false);
        magneticIcon.gameObject.SetActive(false);
        forkliffIcon.gameObject.SetActive(false);

        count.text = $"{player.Backpack.ItemsContainer.Count}/{player.Backpack.ItemsContainer.Capacity}";
        fillAmount.fillAmount = player.Backpack.ItemsContainer.Count / player.Backpack.ItemsContainer.Capacity;

        if (player as Helicopter)
        {
            helicopterIcon.gameObject.SetActive(true);
        }
        else if (player as Magnete)
        {
            magneticIcon.gameObject.SetActive(true);
        }
        else
            forkliffIcon.gameObject.SetActive(true);

    }
}

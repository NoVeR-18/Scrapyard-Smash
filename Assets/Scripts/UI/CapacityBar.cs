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

    public Transform fullIcon;


    private Player.Player player;
    public void UpdateUI()
    {
        if (player != null)
        {

            count.text = $"{player.Backpack.ItemsContainer.Count}/{player.Backpack.ItemsContainer.Capacity}";
            fillAmount.fillAmount = (float)player.Backpack.ItemsContainer.Count / player.Backpack.ItemsContainer.Capacity;

            if (player.Backpack.ItemsContainer.Count >= player.Backpack.ItemsContainer.Capacity)
                fullIcon.gameObject.SetActive(true);
            else fullIcon.gameObject.SetActive(false);

        }

    }
    public void UpdateHelicopterUI()
    {
        if (player != null)
        {
            if (player as Helicopter)
            {
                Helicopter helicopter = (Helicopter)player;
                count.text = $"{helicopter.Fuel}/{helicopter.MaxFuel}";
                fillAmount.fillAmount = (float)helicopter.Fuel / helicopter.MaxFuel;

                if (player.Backpack.ItemsContainer.Count >= player.Backpack.ItemsContainer.Capacity)
                {
                    if (!(player as Helicopter))
                        fullIcon.gameObject.SetActive(true);
                }
                else fullIcon.gameObject.SetActive(false);
            }

        }

    }
    public void UpdateUI(Player.Player player)
    {
        this.player = player;

        helicopterIcon.gameObject.SetActive(false);
        magneticIcon.gameObject.SetActive(false);
        forkliffIcon.gameObject.SetActive(false);

        UpdateUI();

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

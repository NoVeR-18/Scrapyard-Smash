using UnityEngine;
using UnityEngine.UI;

public class ChangeVehicleTab : MonoBehaviour
{
    public const string open = "Open";
    public const string close = "Close";

    public Animator animator;

    public Button OpenCloseButton;
    public Button SelectForkliffButton;
    public Button SelectMagneteButton;

    public Transform ForkliffActiveFrame;
    public Transform ForkliffInactiveFrame;

    public Transform MagneteActiveFrame;
    public Transform MagneteInactiveFrame;

    public Forkliff forkliff;
    public Magnete magnete;

    private bool TabIsOpen = false;

    public AudioSource audioSource;

    private void Start()
    {
        OpenCloseButton.onClick.AddListener(() =>
        {
            OpenCloseTab();
            audioSource?.Play();
        });
        SelectForkliffButton.onClick.AddListener(() =>
        {
            SelectForkliff();
            audioSource?.Play();
        });
        SelectMagneteButton.onClick.AddListener(() =>
        {
            SelectMagnete();
            audioSource?.Play();
        });
    }

    public void CloseTab()
    {
        TabIsOpen = false;
        animator?.SetTrigger(close);
    }
    private void OpenCloseTab()
    {
        GameManager.Instance.Vibrate();
        if (TabIsOpen)
        {
            animator?.SetTrigger(close);
            TabIsOpen = false;
        }
        else
        {
            animator?.SetTrigger(open);
            TabIsOpen = true;
            UpdateUI();
        }

    }


    public void SelectForkliff()
    {

        if (LevelManager.Instance.currentVehicle as Magnete)
        {
            forkliff.SelectVehicle(LevelManager.Instance.currentVehicle.transform);
        }
        else
        {
            LevelManager.Instance.currentVehicle.DisableVechicle();
            forkliff.SelectVehicle();
        }

        CloseTab();
    }

    public void SelectMagnete()
    {

        if (magnete.unlocked)
        {
            if (LevelManager.Instance.currentVehicle as Forkliff)
            {
                magnete.SelectVehicle(LevelManager.Instance.currentVehicle.transform);
            }
            else
            {
                LevelManager.Instance.currentVehicle.DisableVechicle();
                magnete.SelectVehicle();
            }
        }
        CloseTab();
    }
    private void UpdateUI()
    {
        if (TabIsOpen)
        {
            ForkliffActiveFrame.gameObject.SetActive(false);
            ForkliffInactiveFrame.gameObject.SetActive(true);
            MagneteActiveFrame.gameObject.SetActive(false);
            MagneteInactiveFrame.gameObject.SetActive(true);

            if (LevelManager.Instance.currentVehicle as Forkliff)
            {
                ForkliffInactiveFrame.gameObject.SetActive(false);
                ForkliffActiveFrame.gameObject.SetActive(true);
            }
            if (LevelManager.Instance.currentVehicle as Magnete)
            {
                MagneteActiveFrame.gameObject.SetActive(true);
                MagneteInactiveFrame.gameObject.SetActive(false);
            }

        }
    }

}

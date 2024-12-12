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
        }

    }


    public void SelectForkliff()
    {
        if (TabIsOpen)
        {
            ForkliffActiveFrame.gameObject.SetActive(true);
            ForkliffInactiveFrame.gameObject.SetActive(false);
            MagneteActiveFrame.gameObject.SetActive(false);
            MagneteInactiveFrame.gameObject.SetActive(true);
        }

        forkliff.SelectVehicle(magnete.transform);
        magnete.DisableVechicle();
    }

    public void SelectMagnete()
    {
        if (TabIsOpen)
        {
            ForkliffActiveFrame.gameObject.SetActive(false);
            ForkliffInactiveFrame.gameObject.SetActive(true);
            MagneteActiveFrame.gameObject.SetActive(true);
            MagneteInactiveFrame.gameObject.SetActive(false);
        }
        if (magnete.unlocked)
        {
            magnete.SelectVehicle(forkliff.transform);
            forkliff.DisableVechicle();
        }
    }
}

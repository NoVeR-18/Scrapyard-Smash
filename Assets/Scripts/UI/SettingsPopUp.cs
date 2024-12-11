using UnityEngine;
using UnityEngine.UI;

public class SettingsPopUp : MonoBehaviour
{
    public Transform VibrationSlider;
    public Transform MusicSlider;
    public Transform SoundsSlider;

    public Button vibrationSet;
    public Button musicSet;
    public Button soundSet;

    public AudioSource audioSource;

    private void Start()
    {
        soundSet.onClick.AddListener(() =>
        {
            if (GameManager.Instance.sounds)
            {
                GameManager.Instance.SetSound(false);
                SoundsSlider.gameObject.SetActive(false);
            }
            else
            {
                GameManager.Instance.SetSound(true);
                SoundsSlider.gameObject.SetActive(true);
            }
            audioSource?.Play();
        });
        musicSet.onClick.AddListener(() =>
        {
            if (GameManager.Instance.musics)
            {
                GameManager.Instance.SetMusic(false);
                MusicSlider.gameObject.SetActive(false);
            }
            else
            {
                GameManager.Instance.SetMusic(true);
                MusicSlider.gameObject.SetActive(true);
            }
            audioSource?.Play();
        });
        vibrationSet.onClick.AddListener(() =>
        {
            if (GameManager.Instance.vibrations)
            {
                GameManager.Instance.SetVibration(false);
                VibrationSlider.gameObject.SetActive(false);
            }
            else
            {
                GameManager.Instance.SetVibration(true);
                VibrationSlider.gameObject.SetActive(false);

            }
            audioSource?.Play();
        });
    }
    private void OnEnable()
    {
        if (GameManager.Instance.sounds)
            SoundsSlider.gameObject.SetActive(true);
        else
            SoundsSlider.gameObject.SetActive(false);

        if (GameManager.Instance.vibrations)
            VibrationSlider.gameObject.SetActive(true);
        else
            VibrationSlider.gameObject.SetActive(false);

        if (GameManager.Instance.musics)
            MusicSlider.gameObject.SetActive(true);
        else
            MusicSlider.gameObject.SetActive(false);
    }

    public void CloseTab()
    {
        gameObject.SetActive(false);
    }

}

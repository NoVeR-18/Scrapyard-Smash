using UnityEngine;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    private const string VibrationPrefKey = "VibrationEnabled";
    private const string SoundsPrefKey = "SoundsEnabled";
    public int SelectedLevel = 0;
    public bool vibrations;
    public AudioMixer audioMixer;
    public bool sounds;
    //Tutorial
    public int CurrentZoneIndex = 0;

    public static GameManager Instance
    {
        get; set;
    }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        LoadVibrationSettings();
        LoadSoundSettings();
    }
    public void SetVibration(bool enabled)
    {
        vibrations = enabled;
        PlayerPrefs.SetInt(VibrationPrefKey, vibrations ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadVibrationSettings()
    {

        if (PlayerPrefs.HasKey(VibrationPrefKey))
        {
            vibrations = PlayerPrefs.GetInt(VibrationPrefKey) == 1;
        }
        else
        {
            vibrations = true;
        }
    }

    public void SetSound(bool enabled)
    {
        sounds = enabled;

        if (sounds)
        {
            audioMixer.SetFloat("SoundGroup", 0f);
        }
        else
        {
            audioMixer.SetFloat("SoundGroup", -80f);
        }

        PlayerPrefs.SetInt(SoundsPrefKey, sounds ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadSoundSettings()
    {
        if (PlayerPrefs.HasKey(SoundsPrefKey))
        {
            sounds = PlayerPrefs.GetInt(SoundsPrefKey) == 1;
        }
        else
        {
            sounds = true;
        }

        SetSound(sounds);
    }
}
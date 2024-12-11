using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VictoryTab : MonoBehaviour
{
    public ExperienceTable experienceTable;

    public TextMeshProUGUI AwardCount;
    public Button takeButton;
    public List<Player.Player> players;
    LevelManager levelManager;
    public AudioSource audioSource;

    public ParticleSystem VictoryVFX;

    private void Awake()
    {
        takeButton.onClick.AddListener(() => { TakeAward(); });
        levelManager = LevelManager.Instance;
        if (experienceTable == null)
            experienceTable = Resources.Load<ExperienceTable>("ExperienceTable");

    }

    private void OnEnable()
    {
        AwardCount.text = experienceTable.AwardLevelComplete[levelManager.currentLevelIndex].ToString();
    }


    private void TakeAward()
    {
        GameManager.Instance.Vibrate();
        levelManager.wallet.AddMoney(experienceTable.AwardLevelComplete[levelManager.currentLevelIndex]);
        CloseTab();
        foreach (Player.Player player in players)
        {
            player.SpawnPlayer();
            player.DisableVechicle();
        }
        players[0]?.Movement._controls.EnableTutorial();
        players[0]?.EnableVechicle();
        levelManager.LoadNextLevel();
    }
    public void NextLevel()
    {
        CloseTab();
        foreach (Player.Player player in players)
        {
            player.SpawnPlayer();
            player.DisableVechicle();
        }
        players[0]?.Movement._controls.EnableTutorial();
        players[0]?.EnableVechicle();
        levelManager.LoadNextLevel();

    }
    public void OpenTab()
    {
        audioSource?.Play();
        VictoryVFX.Play();
        gameObject.SetActive(true);
    }
    public void CloseTab()
    {
        gameObject.SetActive(false);

    }

}

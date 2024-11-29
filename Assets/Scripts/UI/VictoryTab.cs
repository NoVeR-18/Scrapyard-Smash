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
        levelManager.wallet.AddMoney(experienceTable.AwardLevelComplete[levelManager.currentLevelIndex]);
        levelManager.LoadNextLevel();
        CloseTab();
        foreach (Player.Player player in players)
        {
            player.SpawnPlayer();
        }
        players[0]?.Movement._controls.EnableTutorial();
    }

    public void OpenTab()
    {
        gameObject.SetActive(true);
    }
    public void CloseTab()
    {
        gameObject.SetActive(false);

    }

}

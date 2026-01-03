using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VictoryTab : MonoBehaviour
{
    public ExperienceTable experienceTable;

    public TextMeshProUGUI AwardCount;
    public Button takeButton;
    public Button takeADSButton;
    public Button NoThanksButton;
    public Animator animator;
    public List<Player.Player> players;
    public AudioSource audioSource;
    public CapacityBar capacityBar;
    public ParticleSystem VictoryVFX;

    private LevelManager levelManager;

    private void Awake()
    {
        takeButton.onClick.AddListener(() => { TakeAward(); });
        NoThanksButton.onClick.AddListener(() => { TakeAward(); });
        takeADSButton.onClick.AddListener(() =>
        {
            AdManager.Instance.ShowRewarded(() =>
            {
                TakeAward(3); // x3 reward for watching ad
            });
        });

        levelManager = LevelManager.Instance;

        if (experienceTable == null)
            experienceTable = Resources.Load<ExperienceTable>("ExperienceTable");
    }

    private void OnEnable()
    {
        AwardCount.text = experienceTable.AwardLevelComplete[levelManager.currentLevelIndex].ToString();
    }

    private void TakeAward(int multiplier = 1)
    {
        GameManager.Instance.Vibrate();

        int reward = experienceTable.AwardLevelComplete[levelManager.currentLevelIndex] * multiplier;
        levelManager.wallet.AddMoney(reward);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            levelManager.wallet.canvas.transform as RectTransform,
            RectTransformUtility.WorldToScreenPoint(levelManager.wallet.canvas.worldCamera, takeButton.transform.position),
            levelManager.wallet.canvas.worldCamera,
            out Vector2 uiPosition
        );

        levelManager.wallet.CollectUIMoney(uiPosition);
        CloseTab();

        foreach (Player.Player player in players)
        {
            player.SpawnPlayer();
            player.DisableVechicle();
        }

        players[0]?.Movement._controls.EnableTutorial();
        players[0]?.EnableVechicle();
        if (AdManager.Instance.IsInterstitialReady())
        {
            if (multiplier == 1)
            {
                // Show interstitial before loading next level
                AdManager.Instance.ShowInterstitial(() =>
                {
                    levelManager.LoadNextLevel();
                });
            }
            else
                levelManager.LoadNextLevel();
        }
        else
        {
            levelManager.LoadNextLevel();
        }
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
        gameObject.SetActive(true);
        animator.SetTrigger("OpenWin");

        // Show/hide buttons depending on ad availability
        if (AdManager.Instance != null && AdManager.Instance.IsRewardedReady())
        {
            takeADSButton.gameObject.SetActive(true);
            NoThanksButton.gameObject.SetActive(true);
            takeButton.gameObject.SetActive(false);
        }
        else
        {
            takeADSButton.gameObject.SetActive(false);
            takeButton.gameObject.SetActive(true);
            NoThanksButton.gameObject.SetActive(true);
        }

        audioSource?.Play();
        VictoryVFX.Play();
        capacityBar.gameObject.SetActive(false);
    }

    public void CloseTab()
    {
        gameObject.SetActive(false);
    }
}
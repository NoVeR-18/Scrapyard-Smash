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
    LevelManager levelManager;
    public AudioSource audioSource;

    public CapacityBar capacityBar;

    public ParticleSystem VictoryVFX;

    private void Awake()
    {
        takeButton.onClick.AddListener(() => { TakeAward(); });
        NoThanksButton.onClick.AddListener(() => { TakeAward(); });
        takeADSButton.onClick.AddListener(() =>
        {
            YsoCorp.GameUtils.YCManager.instance.adsManager.ShowRewarded((bool ok) =>
            {
                if (ok) { TakeAward(3); }
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
        levelManager.wallet.AddMoney(experienceTable.AwardLevelComplete[levelManager.currentLevelIndex] * multiplier);
        Vector2 uiPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            LevelManager.Instance.wallet.canvas.transform as RectTransform,
            RectTransformUtility.WorldToScreenPoint(LevelManager.Instance.wallet.canvas.worldCamera, takeButton.transform.position),
            LevelManager.Instance.wallet.canvas.worldCamera,
            out uiPosition
        );

        LevelManager.Instance.wallet.CollectUIMoney(uiPosition);
        CloseTab();
        foreach (Player.Player player in players)
        {
            player.SpawnPlayer();
            player.DisableVechicle();
        }
        players[0]?.Movement._controls.EnableTutorial();
        players[0]?.EnableVechicle();
        if (multiplier == 1)
            YsoCorp.GameUtils.YCManager.instance.adsManager.ShowInterstitial(() =>
            {
                levelManager.LoadNextLevel();
            });
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
        if (YsoCorp.GameUtils.YCManager.instance.adsManager.IsRewardedAdReady())
        {
            takeADSButton.gameObject.SetActive(true);
            takeButton.gameObject.SetActive(false);
        }
        else
        {
            takeADSButton.gameObject.SetActive(false);
            takeButton.gameObject.SetActive(true);
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

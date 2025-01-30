using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyTasksManager : MonoBehaviour
{
    public enum RewardType
    {
        Money,
        Crystal
    }
    [System.Serializable]
    public class DailyTask
    {
        public TaskType TaskType;
        public string Description;
        public int RequiredCalls;

        public int CurrentCalls;
        public bool IsCompleted;
        public Reward reward;

        [System.Serializable]
        public class Reward
        {
            public int Count;
            public RewardType rewardType;
        }
    }
    public List<DailyTask> DailyTasks = new List<DailyTask>();
    private int currentTaskIndex;
    private DateTime lastResetDate;

    public static DailyTasksManager Instance { get; private set; }

    //UI
    public TextMeshProUGUI DescriptionText;
    public TextMeshProUGUI RewardText;
    public TextMeshProUGUI CountToCompleteText;
    public TextMeshProUGUI CurrentIndexTaskText;
    public Transform TaskPanelPopUp;
    public Transform MoneyIcon;
    public Transform CrystalIcon;
    public Animator Animator;


    public Button TakeAward;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void UpdateUI()
    {
        if (currentTaskIndex >= DailyTasks.Count)
        {
            TaskPanelPopUp.gameObject.SetActive(false);
        }
        else
        {
            TaskPanelPopUp.gameObject.SetActive(true);
            var task = DailyTasks[currentTaskIndex];
            DescriptionText.text = task.Description;
            RewardText.text = task.reward.Count.ToString();
            CountToCompleteText.text = $"{task.CurrentCalls}/{task.RequiredCalls}";
            CurrentIndexTaskText.text = $"{currentTaskIndex + 1}/{DailyTasks.Count}";
            if (task.reward.rewardType == RewardType.Money)
            {
                MoneyIcon.gameObject.SetActive(true);
                CrystalIcon.gameObject.SetActive(false);
            }
            else if (task.reward.rewardType == RewardType.Crystal)
            {
                MoneyIcon.gameObject.SetActive(false);
                CrystalIcon.gameObject.SetActive(true);
            }
            var currentTask = DailyTasks[currentTaskIndex];
            if (currentTask.CurrentCalls >= currentTask.RequiredCalls)
            {
                TakeAward.interactable = true;
                Animator.SetBool("Complete", true);
                if (currentTaskIndex == DailyTasks.Count)
                {
                    Debug.Log("Все задания на сегодня выполнены!");
                }
            }
            else
            {
                Animator.SetBool("Complete", false);
                TakeAward.interactable = false;
            }
        }
    }
    private void Start()
    {
        LoadProgress();
        CheckAndResetTasks();
        if (TakeAward != null)
        {
            TakeAward.onClick.AddListener(() => { GiveReward(); });
        }
    }
    private void CheckAndResetTasks()
    {
        if (lastResetDate.Date != DateTime.Now.Date)
        {
            ResetTasks();
            lastResetDate = DateTime.Now;
            SaveProgress();
        }
    }

    private void LoadProgress()
    {
        if (PlayerPrefs.HasKey("LastResetDate"))
        {
            lastResetDate = DateTime.Parse(PlayerPrefs.GetString("LastResetDate"));
        }
        else
        {
            lastResetDate = DateTime.MinValue;
        }

        for (int i = 0; i < DailyTasks.Count; i++)
        {
            DailyTasks[i].IsCompleted = PlayerPrefs.GetInt($"Task_{i}_Completed", 0) == 1;
        }
        currentTaskIndex = PlayerPrefs.GetInt("CurrentTaskIndex", 0);

        UpdateUI();
    }
    private void GiveReward()
    {
        var task = DailyTasks[currentTaskIndex];
        task.IsCompleted = true;
        var reward = task.reward;
        currentTaskIndex++;
        if (reward == null)
            return;
        if (reward.rewardType == RewardType.Crystal)
        {
            LevelManager.Instance.wallet.AddCrystals(reward.Count);
            LevelManager.Instance.wallet.CollectCrystal(TakeAward.transform.position);
        }
        else if (reward.rewardType == RewardType.Money)
        {
            LevelManager.Instance.wallet.AddMoney(reward.Count);
            LevelManager.Instance.wallet.CollectMoney(TakeAward.transform.position);
        }
        UpdateUI();
        SaveProgress();
    }
    public void PerformTask(TaskType taskType)
    {
        if (currentTaskIndex >= DailyTasks.Count)
        {
            Debug.Log("Все задания на сегодня выполнены!");
            return;
        }

        var currentTask = DailyTasks[currentTaskIndex];

        if (currentTask.TaskType != taskType)
        {
            return;
        }
        if (currentTask.CurrentCalls < currentTask.RequiredCalls)
            currentTask.CurrentCalls++;


        UpdateUI();
    }


    public void ResetTasks()
    {
        foreach (var task in DailyTasks)
        {
            task.CurrentCalls = 0;
            task.IsCompleted = false;
        }

        Debug.Log("Задания сброшены. Можно начинать сначала!");
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetString("LastResetDate", lastResetDate.ToString());

        for (int i = 0; i < DailyTasks.Count; i++)
        {
            PlayerPrefs.SetInt($"Task_{i}_Completed", DailyTasks[i].IsCompleted ? 1 : 0);
        }
        PlayerPrefs.SetInt("CurrentTaskIndex", currentTaskIndex);

        PlayerPrefs.Save();
    }
    private void OnApplicationQuit()
    {
        SaveProgress();
    }
}
public enum TaskType
{
    CollectCar,
    CollectTrash,
    UpgradeForkliff,
    UpgradeMagnete,
    Clean80Percent
}
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
            string savedDate = PlayerPrefs.GetString("LastResetDate");
            Debug.Log($"Загруженная дата (Ticks): {savedDate}");

            if (long.TryParse(savedDate, out long ticks))
            {
                lastResetDate = new DateTime(ticks);
                Debug.Log($"Дата успешно загружена: {lastResetDate}");
            }
            else
            {
                Debug.LogError($"Ошибка загрузки даты: {savedDate}");
                lastResetDate = DateTime.Now;
            }
        }
        else
        {
            lastResetDate = DateTime.Now; // Если нет сохранённой даты, устанавливаем текущую
            SaveProgress();
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
            Vector2 uiPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                LevelManager.Instance.wallet.canvas.transform as RectTransform,
                RectTransformUtility.WorldToScreenPoint(LevelManager.Instance.wallet.canvas.worldCamera, TakeAward.transform.position),
                LevelManager.Instance.wallet.canvas.worldCamera,
                out uiPosition
            );

            LevelManager.Instance.wallet.CollectUICrystal(uiPosition);
        }
        else if (reward.rewardType == RewardType.Money)
        {
            LevelManager.Instance.wallet.AddMoney(reward.Count);
            Vector2 uiPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                LevelManager.Instance.wallet.canvas.transform as RectTransform,
                RectTransformUtility.WorldToScreenPoint(LevelManager.Instance.wallet.canvas.worldCamera, TakeAward.transform.position),
                LevelManager.Instance.wallet.canvas.worldCamera,
                out uiPosition
            );

            LevelManager.Instance.wallet.CollectUIMoney(uiPosition);
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
        PlayerPrefs.SetString("LastResetDate", lastResetDate.Ticks.ToString());
        Debug.Log($"Сохраненная дата (Ticks): {lastResetDate.Ticks}");

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
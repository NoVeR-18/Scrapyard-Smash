using Player;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    private const string CurrentLevelKey = "currentLevelIndexKey";
    private const string TempLevelKey = "tempLevelKey";

    public static LevelManager Instance { get; private set; }
    public PlayerWallet wallet;
    public VictoryTab victoryTab;
    public ChangeVehicleTab changeVehicleTab;
    public LevelMap levelMap;

    public Transform nextLevel;
    public ExperienceTable experienceTable;

    [SerializeField] private Transform parentContainer; // Контейнер объектов уровня
    [SerializeField] private List<GameObject> prefabs; // Список префабов, связанных с типами объектов

    public List<Player.Player> vechicles;
    [SerializeField] private Player.Player ufo;
    [SerializeField]
    private List<LevelData> loadedLevels; // Список загруженных уровней

    public Player.Player currentVehicle;

    [SerializeField] private ChangeBackgroundZone changeBackgroundZone;

    public int currentLevelIndex; // Индекс текущего уровня
    public int CarsCollected = 0;
    public int TrashCollected = 0;
    [SerializeField]
    private int CarsOnScene = 0;
    [SerializeField]
    private int TrashOnScene = 0;

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

        LoadLevelsFromResources(); // Загружаем все уровни из папки Resources/Levels
    }
    private void Start()
    {
        currentLevelIndex = PlayerPrefs.GetInt(CurrentLevelKey, 0);

        LoadLevel(currentLevelIndex);
        // Проверяем наличие временных данных
        if (PlayerPrefs.HasKey(TempLevelKey))
        {
            string tempLevelJson = PlayerPrefs.GetString(TempLevelKey);
            LevelData tempLevel = ScriptableObject.CreateInstance<LevelData>();
            JsonUtility.FromJsonOverwrite(tempLevelJson, tempLevel);

            Debug.Log("Loading temporary level...");
            LoadTempLevel(tempLevel);

            // Удаляем временные данные после загрузки
            PlayerPrefs.DeleteKey(TempLevelKey);
            PlayerPrefs.Save();
        }

        nextLevel.GetComponent<Button>().onClick.AddListener(() =>
        {
            nextLevel.gameObject.SetActive(false);
            GameManager.Instance.Vibrate();
            victoryTab.OpenTab();
        });

        if (experienceTable == null)
            experienceTable = Resources.Load<ExperienceTable>("ExperienceTable");
    }
    private void LoadLevelsFromResources()
    {
        //loadedLevels = new List<LevelData>(Resources.LoadAll<LevelData>("Levels"));

        if (loadedLevels.Count == 0)
        {
            Debug.LogError("No levels found in Resources/Levels!");
        }
    }

    public void SaveLevel(LevelData levelData)
    {
        levelData.groupedObjects.Clear();

        Dictionary<string, ObjectData> objectGroups = new Dictionary<string, ObjectData>();

        // Рекурсивный метод для обхода всех объектов, включая подгруппы
        void SaveChildObjects(Transform parent)
        {
            foreach (Transform child in parent)
            {
                LevelObject levelObject = child.GetComponent<LevelObject>();
                if (levelObject != null)
                {
                    string prefabName = levelObject.Name; // Имя префаба
                    if (!prefabs.Exists(p => p.name == prefabName)) // Проверяем наличие префаба в списке
                    {
                        Debug.LogError($"Prefab {prefabName} not found in prefabs list!");
                        continue;
                    }

                    if (!objectGroups.ContainsKey(prefabName))
                    {
                        objectGroups[prefabName] = new ObjectData
                        {
                            prefabName = prefabName,
                            objectType = levelObject.objectType
                        };
                    }

                    ObjectData group = objectGroups[prefabName];
                    group.positions.Add(child.position);
                    group.rotations.Add(child.rotation);
                    group.scales.Add(child.localScale);
                }

                // Рекурсивно обходим дочерние объекты
                SaveChildObjects(child);
            }
        }

        // Запускаем обход с основного контейнера
        SaveChildObjects(parentContainer);

        foreach (var group in objectGroups.Values)
        {
            levelData.groupedObjects.Add(group);
        }

        Debug.Log("Level saved to ScriptableObject, including parent-child hierarchy!");
    }


    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= loadedLevels.Count)
        {
            Debug.LogError("Invalid level index!");
            return;
        }

        LevelData levelData = loadedLevels[levelIndex];

        if (changeVehicleTab.magnete.unlocked)
            changeVehicleTab.gameObject.SetActive(true);
        if ((levelIndex + 2) % 5 == 0 && levelIndex > 0)
        {
            foreach (var player in vechicles)
            {
                player.DisableVechicle();
            }
            ufo.EnableVechicle();
            ufo.capacityBar.gameObject.SetActive(false);
            changeVehicleTab.gameObject.SetActive(false);
            ufo.SpawnPlayer();
        }
        else
        {
            vechicles[0].EnableVechicle();
            currentVehicle.capacityBar.gameObject.SetActive(true);
        }
        LoadLevel(levelData);
        changeBackgroundZone.ChaneMaterial(levelIndex + 1);
        levelMap.UpdateLevelMap(levelIndex);
    }

    public void LoadLevel(LevelData levelData)
    {
        // Удаляем старые объекты
        if (Application.isPlaying)
        {
            foreach (Transform child in parentContainer)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            while (parentContainer.childCount > 0)
            {
                DestroyImmediate(parentContainer.GetChild(0).gameObject);
            }
        }
        CarsCollected = 0;
        CarsOnScene = 0;
        TrashCollected = 0;
        TrashOnScene = 0;

        nextLevel.gameObject.SetActive(false);
        // Словарь для хранения родительских объектов по типам
        Dictionary<ObjectType, Transform> parentGroups = new Dictionary<ObjectType, Transform>();

        // Загружаем новые объекты
        foreach (ObjectData group in levelData.groupedObjects)
        {
            // Находим префаб
            GameObject prefab = prefabs.Find(p => p.name == group.prefabName);

            if (prefab == null)
            {
                Debug.LogError($"Prefab {group.prefabName} not found in prefabs list!");
                continue;
            }

            // Получаем или создаем родительский объект для данного типа
            if (!parentGroups.ContainsKey(group.objectType))
            {
                GameObject newParent = new GameObject(group.objectType.ToString());
                newParent.transform.SetParent(parentContainer);
                parentGroups[group.objectType] = newParent.transform;
            }

            Transform parentGroup = parentGroups[group.objectType];

            // Создаем объекты
            for (int i = 0; i < group.positions.Count; i++)
            {

                if (group.objectType == ObjectType.Car)
                    CarsOnScene++;
                if (group.objectType == ObjectType.Trash)
                    TrashOnScene++;

                if (i >= group.positions.Count || i >= group.rotations.Count || i >= group.scales.Count)
                {
                    Debug.LogError("Invalid data in saved level!");
                    continue;
                }

                Vector3 position = group.positions[i];
                Quaternion rotation = group.rotations[i];
                Vector3 scale = group.scales[i];

                GameObject obj = null;

#if UNITY_EDITOR
                if (!Application.isPlaying)
                    // Используем PrefabUtility для создания экземпляра префаба
                    obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parentGroup);
                else
                    obj = Instantiate(prefab, parentGroup);
#else
            // Если не в редакторе, используем обычный Instantiate
            obj = Instantiate(prefab, parentGroup);
#endif

                if (obj == null || obj.transform == null)
                {
                    Debug.LogError($"Failed to instantiate or find Transform for prefab {prefab.name}");
                    continue;
                }

                // Применяем свойства
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.transform.localScale = scale;

                // Назначаем тип объекта
                LevelObject levelObject = obj.GetComponent<LevelObject>();
                if (levelObject != null)
                {
                    levelObject.objectType = group.objectType;
                }
            }
        }
        CheckWining();
        Debug.Log("Level loaded successfully and sorted by object types!");
    }
    public void LoadTempLevel(LevelData levelData)
    {
        // Удаляем старые объекты
        if (Application.isPlaying)
        {
            foreach (Transform child in parentContainer)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            while (parentContainer.childCount > 0)
            {
                DestroyImmediate(parentContainer.GetChild(0).gameObject);
            }
        }
        CarsCollected = 0;
        TrashCollected = 0;

        nextLevel.gameObject.SetActive(false);
        // Словарь для хранения родительских объектов по типам
        Dictionary<ObjectType, Transform> parentGroups = new Dictionary<ObjectType, Transform>();

        // Загружаем новые объекты
        foreach (ObjectData group in levelData.groupedObjects)
        {
            // Находим префаб
            GameObject prefab = prefabs.Find(p => p.name == group.prefabName);

            if (prefab == null)
            {
                Debug.LogError($"Prefab {group.prefabName} not found in prefabs list!");
                continue;
            }

            // Получаем или создаем родительский объект для данного типа
            if (!parentGroups.ContainsKey(group.objectType))
            {
                GameObject newParent = new GameObject(group.objectType.ToString());
                newParent.transform.SetParent(parentContainer);
                parentGroups[group.objectType] = newParent.transform;
            }

            Transform parentGroup = parentGroups[group.objectType];

            // Создаем объекты
            for (int i = 0; i < group.positions.Count; i++)
            {
                if (group.objectType == ObjectType.Car)
                    CarsCollected++;
                if (group.objectType == ObjectType.Trash)
                    TrashCollected++;

                if (i >= group.positions.Count || i >= group.rotations.Count || i >= group.scales.Count)
                {
                    Debug.LogError("Invalid data in saved level!");
                    continue;
                }

                Vector3 position = group.positions[i];
                Quaternion rotation = group.rotations[i];
                Vector3 scale = group.scales[i];

                GameObject obj = null;

#if UNITY_EDITOR
                if (!Application.isPlaying)
                    // Используем PrefabUtility для создания экземпляра префаба
                    obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parentGroup);
                else
                    obj = Instantiate(prefab, parentGroup);
#else
            // Если не в редакторе, используем обычный Instantiate
            obj = Instantiate(prefab, parentGroup);
#endif

                if (obj == null || obj.transform == null)
                {
                    Debug.LogError($"Failed to instantiate or find Transform for prefab {prefab.name}");
                    continue;
                }

                // Применяем свойства
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.transform.localScale = scale;

                // Назначаем тип объекта
                LevelObject levelObject = obj.GetComponent<LevelObject>();
                if (levelObject != null)
                {
                    levelObject.objectType = group.objectType;
                }
            }

        }
        CarsCollected = CarsOnScene - CarsCollected;
        TrashCollected = TrashOnScene - TrashCollected;
        CheckWining();
        Debug.Log("Level loaded successfully and sorted by object types!");
    }


    public void LoadNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex >= loadedLevels.Count)
            currentLevelIndex = 4;
        PlayerPrefs.SetInt(CurrentLevelKey, currentLevelIndex);
        LoadLevel(currentLevelIndex);

        YsoCorp.GameUtils.YCManager.instance.OnGameStarted(currentLevelIndex);

    }

    public void LoadPreviousLevel()
    {
        int prevLevelIndex = (currentLevelIndex - 1 + loadedLevels.Count) % loadedLevels.Count;
        LoadLevel(prevLevelIndex);
    }
    public void CheckWining()
    {
        if (CarsCollected < (0.8f * CarsOnScene) || TrashCollected < (0.8f * TrashOnScene))
            return;
        if (CarsCollected >= (0.8f * CarsOnScene) || TrashCollected >= (0.8f * TrashOnScene))
        {

            DailyTasksManager.Instance.PerformTask(TaskType.Clean80Percent);
            nextLevel.gameObject.SetActive(true);
            if ((CarsCollected < CarsOnScene) || (TrashCollected < TrashOnScene))
                return;
            else
            {
                nextLevel.gameObject.SetActive(false);
                victoryTab.OpenTab();
                YsoCorp.GameUtils.YCManager.instance.OnGameFinished(true);
                Debug.Log("Wining");
            }
        }
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("Приложение свернуто — выполняем сохранение");
            SaveGameData();
        }
    }

    private void SaveGameData()
    {

        // Сохраняем текущий уровень в PlayerPrefs
        PlayerPrefs.SetInt(CurrentLevelKey, currentLevelIndex);

        // Создаем временный уровень и сохраняем его
        if (loadedLevels.Count > currentLevelIndex)
        {
            LevelData tempLevel = ScriptableObject.CreateInstance<LevelData>();
            SaveLevel(tempLevel);

            // Сохраняем данные временного уровня
            string tempLevelJson = JsonUtility.ToJson(tempLevel);
            PlayerPrefs.SetString(TempLevelKey, tempLevelJson);
        }

        PlayerPrefs.Save(); // Сохраняем изменения
    }
}

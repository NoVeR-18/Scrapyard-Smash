using GameAnalyticsSDK;
using Player;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    private const string CurrentLevelKey = "currentLevelIndexKey";

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
    private int CarsOnScene = 0;
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
            // Если в игровом режиме, используем Destroy
            foreach (Transform child in parentContainer)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            // Если в редакторе, используем DestroyImmediate
            while (parentContainer.childCount > 0)
            {
                DestroyImmediate(parentContainer.GetChild(0).gameObject);
            }
        }

        CarsCollected = 0;
        CarsOnScene = 0;
        TrashCollected = 0;
        TrashOnScene = 0;
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, $"{currentLevelIndex}", "", "Level_Progress");
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

        Debug.Log("Level loaded successfully and sorted by object types!");
    }



    private GameObject FindObjectOnScene(string prefabName, Transform parentGroup)
    {
        foreach (Transform child in parentGroup)
        {
            if (child.name == prefabName)
            {
                return child.gameObject;
            }
        }
        return null;
    }
    public void LoadNextLevel()
    {
        currentLevelIndex++;//= (currentLevelIndex + 1);
        if (currentLevelIndex >= loadedLevels.Count)
            currentLevelIndex = 4;
        PlayerPrefs.SetInt(CurrentLevelKey, currentLevelIndex);
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, $"{currentLevelIndex}", "", "Level_Progress");
        LoadLevel(currentLevelIndex);
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
        if (CarsCollected > (0.8f * CarsOnScene) || TrashCollected > (0.8f * TrashOnScene))
        {
            nextLevel.gameObject.SetActive(true);
            if ((CarsCollected < CarsOnScene) || (TrashCollected < TrashOnScene))
                return;
            else
            {
                nextLevel.gameObject.SetActive(false);
                victoryTab.OpenTab();

                Debug.Log("Wining");
            }
        }
    }

    private void OnApplicationQuit()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, $"{currentLevelIndex}", "", "Level_Progress");
    }

}

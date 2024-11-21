using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private Transform parentContainer; // Контейнер объектов уровня
    [SerializeField] private List<GameObject> prefabs; // Список префабов, связанных с типами объектов

    private List<LevelData> loadedLevels; // Список загруженных уровней
    public int currentLevelIndex; // Индекс текущего уровня

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

    private void LoadLevelsFromResources()
    {
        loadedLevels = new List<LevelData>(Resources.LoadAll<LevelData>("Levels"));

        if (loadedLevels.Count == 0)
        {
            Debug.LogError("No levels found in Resources/Levels!");
        }
    }

    public void SaveLevel(LevelData levelData)
    {
        levelData.groupedObjects.Clear();

        Dictionary<string, ObjectData> objectGroups = new Dictionary<string, ObjectData>();

        foreach (Transform child in parentContainer)
        {
            LevelObject levelObject = child.GetComponent<LevelObject>();
            if (levelObject != null)
            {
                string prefabName = levelObject.Name; // Имя префаба
                if (!prefabs.Exists(p => p.name == prefabName)) // Проверка существования префаба в списке
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
        }

        foreach (var group in objectGroups.Values)
        {
            levelData.groupedObjects.Add(group);
        }

        Debug.Log("Level saved to ScriptableObject!");
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= loadedLevels.Count)
        {
            Debug.LogError("Invalid level index!");
            return;
        }

        currentLevelIndex = levelIndex;
        LevelData levelData = loadedLevels[levelIndex];
        LoadLevel(levelData);
    }

    public void LoadLevel(LevelData levelData)
    {
        // Удаляем старые объекты
        foreach (Transform child in parentContainer)
        {
            DestroyImmediate(child.gameObject);
        }

        foreach (ObjectData group in levelData.groupedObjects)
        {
            GameObject prefab = prefabs.Find(p => p.name == group.prefabName);

            if (prefab == null)
            {
                Debug.LogError($"Prefab {group.prefabName} not found in prefabs list!");
                continue;
            }

            for (int i = 0; i < group.positions.Count; i++)
            {
                Vector3 position = group.positions[i];
                Quaternion rotation = group.rotations[i];
                Vector3 scale = group.scales[i];

                GameObject obj = Instantiate(prefab, position, rotation, parentContainer);
                obj.transform.localScale = scale;

                // Присваиваем тип объекта
                LevelObject levelObject = obj.GetComponent<LevelObject>();
                if (levelObject != null)
                {
                    levelObject.objectType = group.objectType;
                }
            }
        }

        Debug.Log("Level loaded from ScriptableObject!");
    }

    public void LoadNextLevel()
    {
        int nextLevelIndex = (currentLevelIndex + 1) % loadedLevels.Count;
        LoadLevel(nextLevelIndex);
    }

    public void LoadPreviousLevel()
    {
        int prevLevelIndex = (currentLevelIndex - 1 + loadedLevels.Count) % loadedLevels.Count;
        LoadLevel(prevLevelIndex);
    }
}

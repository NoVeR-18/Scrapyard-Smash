using System.IO;
using UnityEditor;
using UnityEngine;

public class LevelEditor : EditorWindow
{
    private static LevelManager levelManager;
    private LevelData levelData;

    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditor>("Level Editor");
        levelManager = FindAnyObjectByType<LevelManager>();
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Editor", EditorStyles.boldLabel);

        levelManager = (LevelManager)EditorGUILayout.ObjectField("Level Manager", levelManager, typeof(LevelManager), true);
        levelData = (LevelData)EditorGUILayout.ObjectField("Level Data", levelData, typeof(LevelData), false);

        GUILayout.Space(10);

        if (GUILayout.Button("Create New LevelData"))
        {
            CreateNewLevelData();
        }

        GUILayout.Space(10);

        if (levelManager != null && levelData != null)
        {
            if (GUILayout.Button("Save Level"))
            {
                levelManager.SaveLevel(levelData);
                EditorUtility.SetDirty(levelData); // Помечаем объект как измененный
                AssetDatabase.SaveAssets();       // Сохраняем изменения в проекте
            }

            if (GUILayout.Button("Load Level"))
            {
                levelManager.LoadLevel(levelData);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Please assign Level Manager or Level Data.", MessageType.Warning);
        }
    }

    private void CreateNewLevelData()
    {
        // Указываем путь по умолчанию
        string defaultFolder = "Assets/Resources/Levels";

        // Проверяем и создаем папки, если их нет
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        if (!AssetDatabase.IsValidFolder(defaultFolder))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Levels");
        }

        // Базовое имя файла
        string baseName = "Level";
        string extension = "asset";
        string path;

        // Проверяем существование файла и добавляем номер, если нужно
        int counter = 0;
        do
        {
            string fileName = counter == 0 ? baseName : $"{baseName} {counter}";
            path = $"{defaultFolder}/{fileName}.{extension}";
            counter++;
        }
        while (File.Exists(path) || AssetDatabase.LoadAssetAtPath<LevelData>(path) != null);

        // Открываем диалог сохранения с предложенным именем
        string userPath = EditorUtility.SaveFilePanelInProject("Save New LevelData", Path.GetFileNameWithoutExtension(path), extension, "Choose a location to save the new LevelData.", defaultFolder);

        if (!string.IsNullOrEmpty(userPath))
        {
            // Создаем новый LevelData
            LevelData newLevelData = ScriptableObject.CreateInstance<LevelData>();

            // Сохраняем в указанный путь
            AssetDatabase.CreateAsset(newLevelData, userPath);
            AssetDatabase.SaveAssets();

            // Устанавливаем созданный LevelData для редактирования
            levelData = newLevelData;

            Debug.Log($"New LevelData created at {userPath}");
        }
    }


}

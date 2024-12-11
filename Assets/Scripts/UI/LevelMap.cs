using TMPro;
using UnityEngine;

[System.Serializable]
public class MapObject
{
    public GameObject current; // Объект для текущего уровня
    public GameObject next;    // Объект для следующего уровня
    public GameObject complete; // Объект для пройденного уровня
}
public class LevelMap : MonoBehaviour
{

    public MapObject[] mapObjects; // Массив всех уровней
    public GameObject[] seasons; // Сезоны для этого уровня (каждые 5 уровней)

    public TextMeshProUGUI levelIndex;

    public void UpdateLevelMap(int currentLevel)
    {
        var mapIndex = currentLevel;
        while (mapIndex > (mapObjects.Length - 1))
            mapIndex -= mapObjects.Length;
        Debug.Log("MapIndex :" + mapIndex);
        for (int i = 0; i < mapObjects.Length; i++)
        {
            // Скрываем все состояния сначала
            mapObjects[i].current.SetActive(false);
            if (mapObjects[i].next != null)
                mapObjects[i].next.SetActive(false);
            if (mapObjects[i].complete != null)
                mapObjects[i].complete.SetActive(false);

            // Логика переключения состояния
            if (i < mapIndex && mapObjects[i].complete != null)
            {
                mapObjects[i].complete?.SetActive(true);
            }
            else if (i == mapIndex)
            {
                mapObjects[i].current.SetActive(true);
            }
            else if (mapObjects[i].next != null)
            {
                mapObjects[i].next?.SetActive(true);
            }

        }
        // Смена сезона каждые 5 уровней
        if (seasons.Length > 0)
        {
            for (int j = 0; j < seasons.Length; j++)
            {
                seasons[j].SetActive(j == (currentLevel / 5) % seasons.Length);
            }
        }
        levelIndex.text = (currentLevel + 1).ToString();
    }
}

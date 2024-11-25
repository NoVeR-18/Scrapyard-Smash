using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectData
{
    public string prefabName; // Имя префаба
    public ObjectType objectType; // Тип объекта
    public List<Vector3> positions = new List<Vector3>(); // Позиции
    public List<Quaternion> rotations = new List<Quaternion>(); // Повороты
    public List<Vector3> scales = new List<Vector3>(); // Размеры
}

[CreateAssetMenu(fileName = "Level", menuName = "Levels/LevelData")]
public class LevelData : ScriptableObject
{
    public List<ObjectData> groupedObjects = new List<ObjectData>();
}

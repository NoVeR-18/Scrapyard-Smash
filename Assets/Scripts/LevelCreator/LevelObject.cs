using UnityEngine;

public class LevelObject : MonoBehaviour
{
    public ObjectType objectType;
    public string Name;
}
public enum ObjectType
{
    Wall,
    Car,
    Trash,
    Decoration,
    Details,
    Coins
}

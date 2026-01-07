using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExperienceTable", menuName = "Game Data/Experience Table")]
public class ExperienceTable : ScriptableObject
{
    public List<int> experiencePerLevel;
    public List<int> AwardLevelComplete;
    public int CarCost = 30;
    public int TrashCost = 20;
    public int CowCost = 100;

}

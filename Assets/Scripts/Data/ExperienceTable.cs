using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExperienceTable", menuName = "Game Data/Experience Table")]
public class ExperienceTable : ScriptableObject
{
    public List<int> experiencePerLevel;
    public List<int> AwardLevelComplete;
}

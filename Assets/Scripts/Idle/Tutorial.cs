using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    private Transform Arrow;

    public List<Transform> focusedItem;

    public Transform forkliffArrow;
    public Transform magneteArrow;


    [HideInInspector]
    public bool _tutorialCompleted;

    public static Tutorial instance;

    public int TutorialIndex = 0;

    private void Start()
    {
        _tutorialCompleted = PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;

        foreach (var item in focusedItem)
        {
            item.gameObject.SetActive(false);
        }
        if (!_tutorialCompleted)
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }


            TutorialIndex = PlayerPrefs.GetInt("TutorialIndex", 0);
            focusedItem[TutorialIndex].gameObject.SetActive(true);

            if (TutorialIndex < 3)
            {
                Arrow = forkliffArrow;
                Arrow.gameObject.SetActive(true);
            }
            else if (TutorialIndex >= 6)
            {
                Arrow = magneteArrow;
                Arrow.gameObject.SetActive(true);
            }
            else
                Arrow = null;

            if (TutorialIndex == 4)
            {
                if (LevelManager.Instance.changeVehicleTab.magnete.unlocked)
                    focusedItem[TutorialIndex].gameObject.SetActive(true);
                else
                    focusedItem[TutorialIndex].gameObject.SetActive(false);
            }

        }
        else
        {
            Destroy(gameObject);

        }
    }
    private void Update()
    {
        if (_tutorialCompleted) return;
        if (focusedItem != null && Arrow != null)
            RotateArrowTowardsZone();
    }



    private void RotateArrowTowardsZone()
    {
        Vector3 directionToZone = (focusedItem[TutorialIndex].transform.position - Arrow.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToZone.x, 0, directionToZone.z));

        lookRotation *= Quaternion.Euler(0, 90, 0);
        Arrow.localRotation = Quaternion.Slerp(Arrow.localRotation, lookRotation, Time.deltaTime * 5f);
    }

    public void CompleteZone(int INDEX)
    {
        if (INDEX == TutorialIndex && !_tutorialCompleted)
        {
            focusedItem[TutorialIndex].gameObject.SetActive(false);

            TutorialIndex++;
            PlayerPrefs.SetInt("TutorialIndex", TutorialIndex);
            forkliffArrow.gameObject.SetActive(false);
            magneteArrow.gameObject.SetActive(false);
            if (TutorialIndex < 3)
            {
                Arrow = forkliffArrow;
                Arrow.gameObject.SetActive(true);
            }
            else if (TutorialIndex >= 6)
            {
                Arrow = magneteArrow;
                Arrow.gameObject.SetActive(true);
            }
            else
                Arrow = null;

            if (TutorialIndex >= focusedItem.Count) { CompleteTutorial(); return; }
            if (TutorialIndex == 4)
            {
                if (LevelManager.Instance.changeVehicleTab.magnete.unlocked)
                    focusedItem[TutorialIndex].gameObject.SetActive(true);
                else
                    focusedItem[TutorialIndex].gameObject.SetActive(false);
            }
            focusedItem[TutorialIndex].gameObject.SetActive(true);
        }
    }

    private void CompleteTutorial()
    {
        magneteArrow.gameObject.SetActive(false);
        forkliffArrow.gameObject.SetActive(false);
        _tutorialCompleted = true;
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
    }
}

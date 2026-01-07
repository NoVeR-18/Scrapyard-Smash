using UnityEngine;

public class TutorialZone : MonoBehaviour
{
    public int indexZone;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (Tutorial.instance != null)
                Tutorial.instance.CompleteZone(indexZone);
        }
    }
}

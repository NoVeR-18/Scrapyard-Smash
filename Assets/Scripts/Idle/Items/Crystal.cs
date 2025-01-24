using UnityEngine;

public class Crystal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            LevelManager.Instance.wallet.AddCrystals(1);
            Destroy(gameObject);
        }
    }
}

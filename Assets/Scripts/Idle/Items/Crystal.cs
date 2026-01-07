using UnityEngine;

public class Crystal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            LevelManager.Instance.wallet.AddCrystals(1);
            LevelManager.Instance.wallet.CollectCrystal(transform.position);
            GameManager.Instance.Vibrate();
            Destroy(gameObject);
        }
    }
}

using UnityEngine;

public class Money : MonoBehaviour
{
    public int Cost = 50;
    public Item item;

    private void Start()
    {
        Cost = item.Cost;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {

            LevelManager.Instance.wallet.AddMoney(Cost);
            other.GetComponent<Player.Player>().PlayEffect(Cost);


            Destroy(gameObject);
        }

    }
}

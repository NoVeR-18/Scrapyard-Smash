using TMPro;
using UnityEngine;

public class UpgradeTab : StoragePad
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float _costUpgrade = 50f;
    private float _spendetCoin = 0;
    private void Start()
    {
        UpdateUI();
    }
    public override void CloseInteract()
    {
        base.CloseInteract();
        text.text = "Update";
    }
    //void OnTriggerStay(Collider collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        Interact(collision.gameObject.GetComponent<Player.Player>());
    //    }
    //}
    void UpdateUI()
    {
        text.text = $"Upgrade \n{_spendetCoin}/{_costUpgrade}";
    }
}

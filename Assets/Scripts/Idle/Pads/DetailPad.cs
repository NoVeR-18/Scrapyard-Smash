using System.Collections;
using TMPro;
using UnityEngine;

public class DetailPad : StoragePad
{
    [SerializeField] private GameAward _gameAward;
    [SerializeField] private TextMeshPro text;
    [SerializeField] private Transform SpawnPoint;

    private void Start()
    {
        GameManager.Instance.SelectedLevel = PlayerPrefs.GetInt("DetailCompletedLevel", 0);
    }
    public override void CloseInteract()
    {
        base.CloseInteract();
        text.text = "DETAIL MACHINE";
    }
    public override void Interact(Player.Player player)
    {
        base.Interact(player);
        StartCoroutine(Timer(player));
    }
    IEnumerator Timer(Player.Player player)
    {
        text.text = 3.ToString();
        yield return new WaitForSeconds(1f);
        text.text = 2.ToString();
        yield return new WaitForSeconds(1f);
        text.text = 1.ToString();
        yield return new WaitForSeconds(1f);
    }
}

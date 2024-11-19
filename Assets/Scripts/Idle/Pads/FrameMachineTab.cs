using System.Collections;
using TMPro;
using UnityEngine;

public class FrameMachineTab : StoragePad
{
    [SerializeField] private GameAward _gameAward;
    [SerializeField] private TextMeshPro text;
    [SerializeField] private int _sceneNumber;
    [SerializeField] private Transform SpawnPoint;
    public override void CloseInteract()
    {
        base.CloseInteract();
        text.text = "FRAIM MACHINE" + (_sceneNumber - 1);
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
        GameManager.Instance.SelectedLevel = PlayerPrefs.GetInt($"Frame{_sceneNumber}CompletedLevel", 0);
    }
}

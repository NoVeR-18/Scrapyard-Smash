using System.Collections;
using UnityEngine;

public class BrokenMachine : MonoBehaviour
{
    public StoragePad DetailPad;
    public StoragePad RepairKitZone;
    public Transform RepairLine;
    [SerializeField]
    private float _secondsToBrokeMachine = 120f;
    private void Start()
    {
        _secondsToBrokeMachine = PlayerPrefs.GetInt("SecondsToBrokeMachine", 60);

        EnableRepair(true);
        if (RepairKitZone is RepairZone repairZone)
        {
            repairZone.TimeIsUp += EnableRepair;
        }
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(_secondsToBrokeMachine);
        EnableRepair(false);
        StopCoroutine(Timer());
        PlayerPrefs.SetInt("SecondsToBrokeMachine", 240);
        _secondsToBrokeMachine = 240;
        PlayerPrefs.Save();
    }

    private void EnableRepair(bool status)
    {
        DetailPad.gameObject.SetActive(status);
        RepairKitZone.gameObject.SetActive(!status);
        RepairLine.gameObject.SetActive(!status);
        if (status)
            StartCoroutine(Timer());
    }
}

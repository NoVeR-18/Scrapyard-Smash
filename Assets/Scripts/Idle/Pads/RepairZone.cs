using System;
using UnityEngine;
using UnityEngine.UI;

public class RepairZone : StoragePad
{
    [Space]
    public Image timerImage;
    [SerializeField]
    private float _timeToRepair = 3;
    private float currentTime;
    private bool isTimerRunning = false;

    public Action<bool> TimeIsUp;

    public override void Interact(Player.Player player)
    {
        base.Interact(player);
        if (ItemsContainer.CountItemTypeInSlots(ItemType) > 0)
        {
            if (!isTimerRunning)
                StartTimer();
        }
    }
    public override void CloseInteract()
    {
        base.CloseInteract();
        StopTimer();
        UpdateTimerUI(_timeToRepair);

    }
    void OnTriggerStay(Collider collision)
    {
        base.Interact(collision.gameObject.GetComponent<Player.Player>());
        if (collision.gameObject.tag == "Player")
        {
            if (isTimerRunning)
            {
                if (currentTime > 0)
                {
                    currentTime -= Time.deltaTime;
                    UpdateTimerUI(currentTime);
                }
                else
                {
                    currentTime = 0;
                    StopTimer();
                    OnTimeOut();
                }
            }
        }
    }
    private void OnTimeOut()
    {
        Debug.Log("Time's up!");
        SpendRepairItem();
        TimeIsUp?.Invoke(true);
    }
    private void StartTimer()
    {
        currentTime = _timeToRepair;
        isTimerRunning = true;
    }

    private void StopTimer()
    {
        isTimerRunning = false;
    }
    private void UpdateTimerUI(float time)
    {
        timerImage.fillAmount = time / _timeToRepair;
    }
    public override void takeItemsFromPlayer(Player.Player player)
    {
        if (!ItemsContainer.CanAddItem())
            return;

        Item takenItem = null;

        ItemsContainer.AddItem(takenItem);
    }
    private void SpendRepairItem()
    {
        while (true)
        {
            if (!ItemsContainer.CanTakeItem())
                break;

            if (Type == StorageBuildingType.Input)
            {
                Item inputItem;

                if (!ItemsContainer.TakeItem(out inputItem))
                    return;

                inputItem.Disappear();
            }

        }
    }
}
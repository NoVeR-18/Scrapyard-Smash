using UnityEngine;

public class InteractivePad : StoragePad
{
    [SerializeField]
    private float interactionTime = 2f;
    private float timer = 0f;
    private bool isInteracting = false;

    public override void Interact(Player.Player player)
    {
        isInteracting = true;
        Debug.Log("InteractivePad start");
    }

    public override void CloseInteract()
    {
        isInteracting = false;
        timer = 0f;
    }

    private void FixedUpdate()
    {
        if (isInteracting)
        {
            timer += Time.fixedDeltaTime;

            if (timer >= interactionTime)
            {
                TriggerEvent();
                timer = 0f;
            }
        }
    }

    private void TriggerEvent()
    {
        CloseInteract();
        // Здесь можно прописать любое событие, которое должно произойти через минуту
        Debug.Log("InteractivePad complete");
    }
}

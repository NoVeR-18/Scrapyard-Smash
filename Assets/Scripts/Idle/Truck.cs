using System.Collections;
using TMPro;
using UnityEngine;


public class Truck : MonoBehaviour
{
    public Transform objectToMove;
    public float moveDistance = 30f;
    public float moveDuration = 1f;
    Vector3 startPosition;

    public TextMeshProUGUI ItemsCount;
    public bool canSell = true;
    private void Start()
    {
        startPosition = objectToMove.position;
        canSell = true;
    }
    public void Animated()
    {
        canSell = false;
        StartCoroutine(MoveObject());
    }

    private IEnumerator MoveObject()
    {
        Vector3 targetPosition = startPosition + new Vector3(moveDistance, 0, 0);

        yield return StartCoroutine(MoveToPosition(startPosition, targetPosition, moveDuration));

        yield return StartCoroutine(MoveToPosition(targetPosition, startPosition, moveDuration));
        canSell = true;

        ItemsCount.text = $"{0}/{10}";
    }

    private IEnumerator MoveToPosition(Vector3 fromPosition, Vector3 toPosition, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            objectToMove.position = Vector3.Lerp(fromPosition, toPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        objectToMove.position = toPosition;
    }
}


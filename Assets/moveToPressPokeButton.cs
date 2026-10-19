using System.Collections;
using UnityEngine;

public class MoveToPressPokeButton : MonoBehaviour
{
    public float moveDistance = 0.02f; // Distanz in Z-Richtung
    public float moveDuration = 0.5f; // Dauer der Bewegung hin und zurück

    private Vector3 originalPosition;
    private bool previousMoveWithStart;

    [SerializeField]
    private bool moveWithStart = false;
    public bool MoveWithStart
    {
        get { return moveWithStart; }
        set
        {
            if (moveWithStart != value)
            {
                moveWithStart = value;
                if (moveWithStart)
                {
                    MoveObject();
                }
            }
        }
    }

    void Start()
    {
        originalPosition = transform.localPosition;
        previousMoveWithStart = moveWithStart;

        if (moveWithStart)
        {
            MoveObject();
        }
    }

    void Update()
    {
        if (previousMoveWithStart != moveWithStart)
        {
            if (moveWithStart)
            {
                MoveObject();
            }
            previousMoveWithStart = moveWithStart;
        }
    }

    public void MoveObject()
    {
        StartCoroutine(MoveObjectRoutine());
    }

    private IEnumerator MoveObjectRoutine()
    {
        Vector3 targetPosition = originalPosition + new Vector3(0, 0, moveDistance);
        yield return StartCoroutine(MoveToPosition(targetPosition, moveDuration / 2));
        yield return StartCoroutine(MoveToPosition(originalPosition, moveDuration / 2));
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.localPosition;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPosition;
    }
}

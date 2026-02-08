using UnityEngine;
using System.Collections;

public class PlanetHover : MonoBehaviour
{
    private Vector3 originalScale;
    private Vector3 targetScale;
    private Coroutine scaleRoutine;

    void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    public void SetHover(bool hover, float scaleMultiplier, float scaleSpeed)
    {
        targetScale = hover ? originalScale * scaleMultiplier : originalScale;

        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(ScaleToTarget(scaleSpeed));
    }

    private IEnumerator ScaleToTarget(float speed)
    {
        while (Vector3.Distance(transform.localScale, targetScale) > 0.001f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
            yield return null;
        }

        transform.localScale = targetScale;
        scaleRoutine = null;
    }
}

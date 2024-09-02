using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class TargetLineUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform horizontalLine, angleLine;

    [SerializeField]
    private float maxYposition = 400f;

    public void UpdatePosition(Vector3 endPosition, float lineSpeed)
    {
        StopAllCoroutines();

        Vector3 startPos = Camera.main.WorldToScreenPoint(transform.position);
        endPosition = Camera.main.WorldToScreenPoint(endPosition);
        Vector2 yOffset = Vector2.zero;

        if (endPosition.y + 100f > startPos.y)
        {
            yOffset = Vector2.up * (Mathf.Abs(endPosition.y - startPos.y) + 100f);
            if (yOffset.y > maxYposition)
                yOffset.y = maxYposition;

            
            startPos = Camera.main.WorldToScreenPoint(transform.position) + Vector3.up * yOffset.y;
        }

        float delta = Mathf.Abs(endPosition.y - startPos.y);

        float hWidth = endPosition.x - startPos.x - delta;

        float aWidth = delta * Mathf.Sqrt(2f);

        //horizontalLine.sizeDelta = new Vector2(hWidth, 8);

        //angleLine.sizeDelta = new Vector2(aWidth, 8);

        StartCoroutine(MoveTargetLine(hWidth, aWidth, yOffset, lineSpeed));

    }

    private IEnumerator MoveTargetLine(float hWidth, float aWidth, Vector2 yOffset, float lineSpeed)
    {
        Vector2 horizontalSize = new Vector2(hWidth, 8);
        Vector2 angleSize = new Vector2(aWidth, 8);

        while (Vector2.Distance(horizontalLine.sizeDelta, horizontalSize) > 0.01f)
        {

            horizontalLine.sizeDelta = Vector2.Lerp(horizontalLine.sizeDelta, horizontalSize, lineSpeed * Time.deltaTime);

            angleLine.sizeDelta = Vector2.Lerp(angleLine.sizeDelta, angleSize, lineSpeed * Time.deltaTime);

            horizontalLine.anchoredPosition = Vector2.Lerp(horizontalLine.anchoredPosition, yOffset, lineSpeed * Time.deltaTime);

            // ∆дать до следующего кадра
            yield return null;
        }

        horizontalLine.sizeDelta = horizontalSize;

        angleLine.sizeDelta = angleSize;

        horizontalLine.anchoredPosition = yOffset;
    }
}

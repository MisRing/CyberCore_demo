using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class HandUIManger : MonoBehaviour
{
    public float minSpacing = 10f; // Минимальное значение отступов
    public float maxSpacing = 30f; // Максимальное значение отступов
    public float smoothDuration = 0.5f; // Длительность плавного изменения отступов
    public float minWidth = 200f; // Минимальная ширина родительской панели
    public float maxWidth = 800f; // Максимальная ширина родительской панели
    public float destroyY = -100f;
    public float delayInSeconds = 0.1f;

    [SerializeField]
    private float animationSpeed = 1f;

    private RectTransform parentRectTransform;
    [SerializeField]
    private RectTransform[] childRects;

    private float targetWidth;
    private float targetSpacing;
    private float targetCenterOffset;

    public static HandUIManger instance;

    private void Awake()
    {
        instance = this;
        parentRectTransform = GetComponent<RectTransform>();
    }


    public IEnumerator UpdateLayout(RectTransform[] _childRects)
    {
        childRects = _childRects;

        float totalChildSize = 0;
        int childCount = 0;

        foreach (var child in childRects)
        {
            totalChildSize += child.rect.width * child.localScale.x; // Учет масштаба
            childCount++;
        }

        if (childCount == 0)
            yield return null;

        float calculatedSpacing = (childCount > 1) ? (parentRectTransform.rect.width - totalChildSize) / (childCount - 1) : minSpacing;
        targetSpacing = Mathf.Clamp(calculatedSpacing, minSpacing, maxSpacing);

        float requiredWidth = totalChildSize + (childCount > 1 ? (childCount - 1) * targetSpacing : 0);
        targetWidth = Mathf.Clamp(requiredWidth, minWidth, maxWidth);

        targetCenterOffset = -(totalChildSize + (childCount - 1) * targetSpacing) / 2;

        SetParentWidth(targetWidth);

        ApplySpacing(targetSpacing, targetCenterOffset);

        yield return AnimateShow();
    }

    private IEnumerator AnimateShow()
    {
        float startY = Mathf.Abs(childRects[0].anchoredPosition.y);

        float skip = (childRects.Length - 1) * delayInSeconds;
        while (startY > 1f)
        {
            for(int i = 0; i < childRects.Length; i++)
            {
                if (i * delayInSeconds <= skip)
                    continue;

                Vector3 lastPosition = childRects[i].anchoredPosition;
                lastPosition.y = 0;
                childRects[i].anchoredPosition = Vector3.Lerp(childRects[i].anchoredPosition, lastPosition, animationSpeed * Time.deltaTime);
            }

            skip -= Time.deltaTime;
            // Плавное перемещение объекта к целевой позиции
            startY = Mathf.Abs(childRects[0].anchoredPosition.y);

            // Ждать до следующего кадра
            yield return null;
        }

        foreach (var rect in childRects)
        {
            Vector3 lastPosition = rect.anchoredPosition;
            lastPosition.y = 0;
            rect.anchoredPosition = lastPosition;
        }
    }

    public IEnumerator AnimateDestroy()
    {
        float startY = Mathf.Abs(Mathf.Abs(childRects[childRects.Length - 1].anchoredPosition.y) + destroyY);

        float skip = (childRects.Length - 1) * delayInSeconds;
        while (startY > 1f)
        {
            for (int i = 0; i < childRects.Length; i++)
            {
                if (i * delayInSeconds <= skip)
                    continue;

                Vector3 lastPosition = childRects[i].anchoredPosition;
                lastPosition.y = destroyY;
                childRects[i].anchoredPosition = Vector3.Lerp(childRects[i].anchoredPosition, lastPosition, animationSpeed * Time.deltaTime);
            }

            skip -= Time.deltaTime;
            // Плавное перемещение объекта к целевой позиции
            startY = Mathf.Abs(Mathf.Abs(childRects[childRects.Length - 1].anchoredPosition.y) + destroyY);

            // Ждать до следующего кадра
            yield return null;
        }
    }

    private void ApplySpacing(float spacing, float centerOffset)
    {
        float totalWidth = 0;

        foreach (var child in childRects)
        {
            RectTransform childRect = child;
            float xPosition = centerOffset + totalWidth + (childRect.rect.width * childRect.localScale.x) / 2;
            childRect.anchoredPosition = new Vector2(xPosition, childRect.anchoredPosition.y);
            totalWidth += (childRect.rect.width * childRect.localScale.x) + spacing;
        }
    }

    private void SetParentWidth(float width)
    {
        width = Mathf.Clamp(width, minWidth, maxWidth);
        RectTransform rt = parentRectTransform;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
}

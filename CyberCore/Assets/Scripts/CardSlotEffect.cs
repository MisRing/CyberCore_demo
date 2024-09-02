using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSlotEffect : MonoBehaviour
{
    public float scaleMultiplier = 1.2f;  // На сколько увеличивать карту и слот
    public float liftAmount = 20f;        // На сколько поднимать карту
    public float animationSpeed = 0.2f;   // Скорость анимации

    private Vector3 originalScale;
    private Vector2 originalSizeDelta;
    private RectTransform cardRectTransform;
    private LayoutElement layoutElement;

    private HandUIManger handLayout;

    //private void Start()
    //{
    //    cardRectTransform = GetComponentInChildren<RectTransform>();
    //    layoutElement = GetComponentInChildren<LayoutElement>();
    //    handLayout = GetComponentInParent<CustomHorizontalLayout>();

    //    originalScale = cardRectTransform.localScale;
    //    originalSizeDelta = cardRectTransform.sizeDelta;
    //}

    //public void OnPointerCardEnter(PointerEventData eventData)
    //{
    //    StopAllCoroutines();
    //    StartCoroutine(AnimateCard(true));
    //}

    //public void OnPointerCardExit(PointerEventData eventData)
    //{
    //    StopAllCoroutines();
    //    StartCoroutine(AnimateCard(false));
    //}

    //private IEnumerator AnimateCard(bool isHovered)
    //{
    //    Vector3 targetScale = isHovered ? originalScale * scaleMultiplier : originalScale;
    //    Vector3 targetPosition = isHovered ? new Vector3(cardRectTransform.anchoredPosition.x, liftAmount, 0) : new Vector3(cardRectTransform.anchoredPosition.x, 0, 0);
    //    Vector2 targetSizeDelta = isHovered ? originalSizeDelta * scaleMultiplier : originalSizeDelta;

    //    float elapsedTime = 0f;

    //    while (elapsedTime < animationSpeed)
    //    {
    //        cardRectTransform.localScale = Vector3.Lerp(cardRectTransform.localScale, targetScale, elapsedTime / animationSpeed);
    //        cardRectTransform.anchoredPosition = Vector3.Lerp(cardRectTransform.anchoredPosition, targetPosition, elapsedTime / animationSpeed);

    //        if (layoutElement != null)
    //        {
    //            layoutElement.preferredWidth = Mathf.Lerp(layoutElement.preferredWidth, targetSizeDelta.x, elapsedTime / animationSpeed);
    //            layoutElement.preferredHeight = Mathf.Lerp(layoutElement.preferredHeight, targetSizeDelta.y, elapsedTime / animationSpeed);
    //        }

    //        elapsedTime += Time.deltaTime;
    //        handLayout.UpdateLayout();
    //        yield return null;
    //    }

    //    cardRectTransform.localScale = targetScale;
    //    cardRectTransform.anchoredPosition = targetPosition;

    //    if (layoutElement != null)
    //    {
    //        layoutElement.preferredWidth = targetSizeDelta.x;
    //        layoutElement.preferredHeight = targetSizeDelta.y;
    //    }
    //}
}

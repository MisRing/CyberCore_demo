using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public uint id;
    public string cardName;
    public string description;
    public uint cost;
    public CardType cardType;

    public CardData cardData;

    private protected Vector3 startPosition;
    private Vector2 mouseOffset;
    private protected RectTransform cardRectTransform;
    private protected CanvasGroup canvasGroup;

    [SerializeField]
    private protected CardUI cardUI;

    private void Awake()
    {
        cardRectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetStats()
    {
        id = cardData.cardId;
        cardName = cardData.cardName;
        description = cardData.cardDescription;
        cost = cardData.cardCost;
        cardType = cardData.cardType;

        if (cardType == CardType.Hack)
            GetComponent<Image>().color = Color.green;
        if (cardType == CardType.Action)
            GetComponent<Image>().color = Color.blue;

        cardUI.SetStats(cardName, cost.ToString(), description, cardType.ToString());
    }

    // Начало перетаскивания
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        cardUI.updateStatus = false;
        startPosition = cardRectTransform.anchoredPosition;

        Vector2 localPointerPosition;
        RectTransform parentRectTransform = cardRectTransform.parent.GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, Input.mousePosition, eventData.pressEventCamera, out localPointerPosition))
        {
            mouseOffset = new Vector2(startPosition.x, startPosition.y) - localPointerPosition;
        }

        //canvasGroup.blocksRaycasts = false; // Отключаем блокировку лучей
    }

    // Перетаскивание
    public virtual void OnDrag(PointerEventData eventData)
    {
        Vector2 localPointerPosition;
        RectTransform parentRectTransform = cardRectTransform.parent.GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, Input.mousePosition, eventData.pressEventCamera, out localPointerPosition))
        {
            cardRectTransform.anchoredPosition = localPointerPosition + mouseOffset;
        }
    }

    // Окончание перетаскивания
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        cardUI.updateStatus = true;
        cardUI.CloseCard(eventData);

        //canvasGroup.blocksRaycasts = true;

        // Проверяем, находится ли карта в пределах допустимой зоны
        if (IsInPlayableArea())
        {
            // Использовать карту
            PlayCard(RoundController.instance.player);
        }
        else
        {
            // Возвращаем карту в исходное положение
            cardRectTransform.anchoredPosition = startPosition;
        }
    }

    private protected bool IsInPlayableArea()
    {
        // Преобразуем позицию карты в экранные координаты
        Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, cardRectTransform.position);

        // Проверяем, находится ли карта в указанной зоне по Y
        return screenPosition.y > DeckManager.instance.minYThreshold && screenPosition.y < DeckManager.instance.maxYThreshold;
    }

    public virtual void PlayCard(UnitController player)
    {
        //Debug.Log("Playing card: " + cardName);

        // Удаление карты из руки и добавление в сброс
        if (DeckManager.instance != null)
        {
            DeckManager.instance.DiscardCard(this.gameObject);
        }
    }


    private protected IEnumerator MoveToCastSlot()
    {

        // Пока объект не достигнет целевой позиции
        while (Vector3.Distance(transform.position, CardCastUI.instance.cardCastSlot.transform.position) > 0.01f)
        {
            // Плавное перемещение объекта к целевой позиции
            transform.position = Vector3.Lerp(transform.position, CardCastUI.instance.cardCastSlot.transform.position, 15f * Time.deltaTime);

            // Ждать до следующего кадра
            yield return null;
        }

        // Убедиться, что объект точно на целевой позиции
        transform.position = CardCastUI.instance.cardCastSlot.transform.position;
    }
}




public enum CardType
{
    Hack = 0,
    Attack = 1,
    Action = 2
}

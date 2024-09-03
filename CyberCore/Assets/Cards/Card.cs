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

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        cardUI.updateStatus = false;
        if (RoundController.instance.enemyTurn)
            return;
        startPosition = cardRectTransform.anchoredPosition;

        Vector2 localPointerPosition;
        RectTransform parentRectTransform = cardRectTransform.parent.GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, Input.mousePosition, eventData.pressEventCamera, out localPointerPosition))
        {
            mouseOffset = new Vector2(startPosition.x, startPosition.y) - localPointerPosition;
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (RoundController.instance.enemyTurn)
            return;

        Vector2 localPointerPosition;
        RectTransform parentRectTransform = cardRectTransform.parent.GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, Input.mousePosition, eventData.pressEventCamera, out localPointerPosition))
        {
            cardRectTransform.anchoredPosition = localPointerPosition + mouseOffset;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        cardUI.updateStatus = true;
        cardUI.CloseCard(eventData);

        if (RoundController.instance.enemyTurn)
            return;

        if (IsInPlayableArea())
        {
            PlayCard(RoundController.instance.player);
        }
        else
        {
            cardRectTransform.anchoredPosition = startPosition;
        }
    }

    private protected bool IsInPlayableArea()
    {
        Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, cardRectTransform.position);

        return screenPosition.y > DeckManager.instance.minYThreshold && screenPosition.y < DeckManager.instance.maxYThreshold;
    }

    public virtual void PlayCard(UnitController player)
    {
        if (DeckManager.instance != null)
        {
            DeckManager.instance.DiscardCard(this.gameObject);
        }
    }


    private protected IEnumerator MoveToCastSlot()
    {
        while (Vector3.Distance(transform.position, CardCastUI.instance.cardCastSlot.transform.position) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, CardCastUI.instance.cardCastSlot.transform.position, 15f * Time.deltaTime);

            yield return null;
        }

        transform.position = CardCastUI.instance.cardCastSlot.transform.position;
    }
}




public enum CardType
{
    Hack = 0,
    Attack = 1,
    Action = 2
}

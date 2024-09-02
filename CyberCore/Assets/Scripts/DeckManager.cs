using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    public static DeckManager instance;

    // Колоды по типам карт
    public List<CardData> actionDeck = new List<CardData>(); // Колода для действий
    public List<CardData> attackDeck = new List<CardData>(); // Колода для атаки
    public List<CardData> hackDeck = new List<CardData>();   // Колода для взлома

    // Руки по типам карт
    public List<GameObject> actionHand = new List<GameObject>(); // Рука для действий
    public List<GameObject> attackHand = new List<GameObject>(); // Рука для атаки
    public List<GameObject> hackHand = new List<GameObject>();   // Рука для взлома

    // Слоты по типам карт
    public List<RectTransform> actionSlots = new List<RectTransform>(); // Слоты для действий
    public List<RectTransform> attackSlots = new List<RectTransform>(); // Слоты для атаки
    public List<RectTransform> hackSlots = new List<RectTransform>();   // Слоты для взлома


    public Transform handPanel;

    public BattleStyle currentBattleStyle;
    public BattleStyle nextBattleStyle;
    public BattleStyle defaultBattleStyle;
    [SerializeField]
    private GameObject cardSlot;


    public float cardDrawSpeed = 0.5f; // Время для анимации карты
    public RectTransform deckTransform; // Трансформ колоды для анимации

    [SerializeField]
    public List<CardData> discardPile = new List<CardData>(); // Сброс карт

    public UnityEvent onDeckChanged;
    public UnityEvent onDiscardChanged;

    public float minYThreshold = 200f; // Минимальная Y-координата для активации карты
    public float maxYThreshold = 600f; // Максимальная Y-координата для активации карты

 

    [SerializeField]
    private Button endTurnButton;

    // Вызовите это событие при изменении колод
    public void NotifyDeckChanged()
    {
        onDeckChanged.Invoke();
    }

    public void NotifyDiscardChanged()
    {
        onDiscardChanged.Invoke();
    }

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator FirstStart()
    {
        List<CardData> allCards = new List<CardData>();
        allCards.AddRange(actionDeck);
        allCards.AddRange(attackDeck);
        allCards.AddRange(hackDeck);

        ShuffleDeck(ref allCards);
        UpdateDecks(allCards);

        yield return new WaitForSeconds(0.3f);
        yield return NewTurnCoroutine();
    }

    public IEnumerator SetBattleStyle(BattleStyle battleStyle)
    {
        if (handPanel.childCount != 0)
        {
            yield return HandUIManger.instance.AnimateDestroy();
            yield return new WaitForSeconds(0.2f);
        }
        
        currentBattleStyle = battleStyle;

        for (int i = handPanel.childCount - 1; i >= 0; i--)
        {
            Destroy(handPanel.GetChild(i).gameObject);
        }

        List<RectTransform> slotRects = new List<RectTransform>();

        hackSlots.Clear();
        for (int i = 0; i < currentBattleStyle.hackCards; i++)
        {
            GameObject slot = Instantiate(cardSlot, handPanel);
            slot.GetComponent<RectTransform>().anchoredPosition = Vector2.down * 100f;
            slot.GetComponent<Image>().color = Color.green;
            slotRects.Add(slot.GetComponent<RectTransform>());

            hackSlots.Add(slot.GetComponent<RectTransform>());
        }

        attackSlots.Clear();
        for (int i = 0; i < currentBattleStyle.attackCards; i++)
        {
            GameObject slot = Instantiate(cardSlot, handPanel);
            slot.GetComponent<RectTransform>().anchoredPosition = Vector2.down * 100f;
            slot.GetComponent<Image>().color = Color.red;
            slotRects.Add(slot.GetComponent<RectTransform>());

            attackSlots.Add(slot.GetComponent<RectTransform>());
        }

        actionSlots.Clear();
        for (int i = 0; i < currentBattleStyle.actionCards; i++)
        {
            GameObject slot = Instantiate(cardSlot, handPanel);
            slot.GetComponent<RectTransform>().anchoredPosition = Vector2.down * 100f;
            slot.GetComponent<Image>().color = Color.blue;
            slotRects.Add(slot.GetComponent<RectTransform>());

            actionSlots.Add(slot.GetComponent<RectTransform>());
        }

        yield return HandUIManger.instance.UpdateLayout(slotRects.ToArray());
    }

    [ContextMenu("Start New Turn")]
    public void StartNewTurn()
    {
        endTurnButton.interactable = false;
        StopAllCoroutines();

        RoundController.instance.StartEnemyTurn();
        //StartCoroutine(NewTurnCoroutine());
    }

    public IEnumerator NewTurnCoroutine()
    {
        DiscardHand();

        if (nextBattleStyle != null)
        {
            yield return SetBattleStyle(nextBattleStyle);
            nextBattleStyle = null;
        }
        else if (currentBattleStyle == null)
            yield return SetBattleStyle(defaultBattleStyle);

        //DrawCards();
        yield return DrawCardsCoroutine(currentBattleStyle.actionCards, currentBattleStyle.attackCards, currentBattleStyle.hackCards);
        NotifyDiscardChanged();

        endTurnButton.interactable = true;
        yield return null;
    }

    // Удаление карты из руки и добавление в сброс
    public void DiscardCard(GameObject card)
    {
        if (card == null) return;

        Card cardComponent = card.GetComponent<Card>();
        if (cardComponent != null)
        {
            switch (cardComponent.cardType)
            {
                case CardType.Action:
                    actionHand.Remove(card);
                    card.GetComponent<CardUI>().CloseCard(null);
                    break;
                case CardType.Attack:
                    attackHand.Remove(card);
                    card.GetComponent<CardUI>().CloseCard(null);
                    break;
                case CardType.Hack:
                    hackHand.Remove(card);
                    card.GetComponent<CardUI>().CloseCard(null);
                    break;
            }
            discardPile.Add(cardComponent.cardData); // Добавляем карту в сброс
            Destroy(card); // Удаляем визуальную карту из UI
        }

        NotifyDiscardChanged();
    }

    // Удаление всех карт из руки и добавление их в сброс
    public void DiscardHand()
    {
        List<GameObject> cardsToRemove = new List<GameObject>(actionHand);
        cardsToRemove.AddRange(attackHand);
        cardsToRemove.AddRange(hackHand);

        foreach (GameObject card in cardsToRemove)
        {
            Card cardComponent = card.GetComponent<Card>();
            if (cardComponent != null)
            {
                discardPile.Add(cardComponent.cardData); // Перемещаем данные карты в сброс
            }
            Destroy(card); // Удаляем визуальную карту из UI
        }

        actionHand.Clear();
        attackHand.Clear();
        hackHand.Clear();

        NotifyDiscardChanged();
    }

    // Добор карт в соответствии с текущими параметрами руки
    //public void DrawCards()
    //{
    //    StartCoroutine(DrawCardsCoroutine(currentBattleStyle.actionCards, currentBattleStyle.attackCards, currentBattleStyle.hackCards));
    //}

    private IEnumerator DrawCardsCoroutine(int actionCount, int attackCount, int hackCount)
    {
        yield return StartCoroutine(DrawCardType(hackDeck, hackHand, hackSlots, hackCount));
        yield return StartCoroutine(DrawCardType(attackDeck, attackHand, attackSlots, attackCount));
        yield return StartCoroutine(DrawCardType(actionDeck, actionHand, actionSlots, actionCount));
    }

    private IEnumerator DrawCardType(List<CardData> deck, List<GameObject> hand, List<RectTransform> slots, int count)
    {
        int drawn = 0;
        while (drawn < count)
        {
            if (deck.Count > 0)
            {
                CardData cardData = deck[0];
                deck.RemoveAt(0);

                GameObject cardGO = Instantiate(cardData.cardPref, slots[drawn]);

                // Получаем компонент EventTrigger
                EventTrigger eventTrigger = cardGO.GetComponent<EventTrigger>();

                int slotID = drawn;
                // Создаем новый EventTrigger.Entry для PointerEnter
                EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerEnter
                };
                pointerEnterEntry.callback.AddListener((data) => { cardGO.GetComponent<CardUI>().OpenCard((PointerEventData)data); });

                // Создаем новый EventTrigger.Entry для PointerExit
                EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerExit
                };
                pointerExitEntry.callback.AddListener((data) => { cardGO.GetComponent<CardUI>().CloseCard((PointerEventData)data); });

                // Добавляем события в список
                eventTrigger.triggers.Add(pointerEnterEntry);
                eventTrigger.triggers.Add(pointerExitEntry);

                cardGO.GetComponent<RectTransform>().anchoredPosition = deckTransform.GetComponent<RectTransform>().anchoredPosition;

                NotifyDeckChanged();

                Card cardComponent = cardGO.GetComponent<Card>();
                if (cardComponent != null)
                {
                    cardComponent.cardData = cardData;
                    cardComponent.SetStats();
                    hand.Add(cardGO);
                    yield return StartCoroutine(AnimateCardDraw(cardGO));
                    //Debug.Log($"Drew card: {cardComponent.cardName}");
                    drawn++;
                }
            }
            else
            {
                // Перетасовываем сброс в колоду
                ReshuffleDiscardIntoDeck();
                // Обновляем ссылку на текущую колоду
                deck = GetDeckForType(hand); // Перезагружаем ссылку на текущую колоду
                yield return null; // Даем Unity время обновить состояние
            }
        }
    }

    // Метод для получения текущей колоды в зависимости от типа карт в руке
    private List<CardData> GetDeckForType(List<GameObject> hand)
    {
        if (hand == actionHand)
            return actionDeck;
        if (hand == attackHand)
            return attackDeck;
        if (hand == hackHand)
            return hackDeck;
        return new List<CardData>();
    }


    private void ReshuffleDiscardIntoDeck()
    {
        List<CardData> allCards = new List<CardData>();
        allCards.AddRange(actionDeck);
        allCards.AddRange(attackDeck);
        allCards.AddRange(hackDeck);

        allCards.AddRange(discardPile);
        discardPile.Clear();
        ShuffleDeck(ref allCards);
        UpdateDecks(allCards);
    }

    private void UpdateDecks(List<CardData> allCards)
    {
        actionDeck = allCards.FindAll(card => card.cardType == CardType.Action);
        attackDeck = allCards.FindAll(card => card.cardType == CardType.Attack);
        hackDeck = allCards.FindAll(card => card.cardType == CardType.Hack);
    }

    private void ShuffleDeck(ref List<CardData> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            CardData temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    // Анимация карты при её доборе
    private IEnumerator AnimateCardDraw(GameObject cardGO)
    {
        RectTransform cardRectTransform = cardGO.GetComponent<RectTransform>();
        cardRectTransform.position = deckTransform.position;

        Vector3 startPosition = cardRectTransform.anchoredPosition;
        Vector3 endPosition = Vector3.zero;

        Vector2 canvasCenter = new Vector2(Screen.width, Screen.height) / 2 + Vector2.up * -150;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cardGO.transform.parent.GetComponent<RectTransform>(), canvasCenter, Camera.main, out localPoint);
        Vector3 cardShowPosition = localPoint;

        float elapsedTime = 0f;

        bool opened = false;
        while (elapsedTime < cardDrawSpeed / 2)
        {
            cardRectTransform.anchoredPosition = Vector3.Lerp(startPosition, cardShowPosition, elapsedTime / cardDrawSpeed * 2);
            elapsedTime += Time.deltaTime;
            if(elapsedTime > cardDrawSpeed / 4 && !opened)
            {
                cardGO.GetComponent<CardUI>().OpenCard(null, true);
                opened = true;
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.25f);

        elapsedTime = 0f;
        while (elapsedTime < cardDrawSpeed / 2)
        {
            cardRectTransform.anchoredPosition = Vector3.Lerp(cardShowPosition, endPosition, elapsedTime / cardDrawSpeed * 2);
            elapsedTime += Time.deltaTime;
            if (opened)
            {
                cardGO.GetComponent<CardUI>().CloseCard(null, true);
                opened = false;
            }
            yield return null;
        }

        cardRectTransform.anchoredPosition = endPosition;
        cardGO.GetComponent<CardUI>().updateStatus = true;
    }
}

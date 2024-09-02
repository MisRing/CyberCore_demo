using UnityEngine;
using UnityEngine.UI;

public class CardPileDisplay : MonoBehaviour
{
    public DeckManager deckManager;
    public Text uiText;
    public bool discard = false;
    // Или для TextMeshPro: public TextMeshProUGUI uiTextPro;

    void Start()
    {
        if(discard)
            deckManager.onDiscardChanged.AddListener(UpdateDiscardCount);
        else
            deckManager.onDeckChanged.AddListener(UpdateDeckCount);
    }

    void UpdateDiscardCount()
    {
        int discardCount = deckManager.discardPile.Count;

        uiText.text = $"Dis: {discardCount}";
        // Или для TextMeshPro: uiTextPro.text = $"Action Deck: {actionDeckCount}\nAttack Deck: {attackDeckCount}\nHack Deck: {hackDeckCount}";
    }

    void UpdateDeckCount()
    {
        int actionDeckCount = deckManager.actionDeck.Count;
        int attackDeckCount = deckManager.attackDeck.Count;
        int hackDeckCount = deckManager.hackDeck.Count;

        uiText.text = $"Ac: {actionDeckCount}\nAt: {attackDeckCount}\nH: {hackDeckCount}";
        // Или для TextMeshPro: uiTextPro.text = $"Action Deck: {actionDeckCount}\nAttack Deck: {attackDeckCount}\nHack Deck: {hackDeckCount}";
    }
}

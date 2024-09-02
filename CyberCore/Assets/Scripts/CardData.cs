using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "Project Data/Cards/CardData")]
public class CardData : ScriptableObject
{
    public uint cardId;
    public string cardName;
    public uint cardCost;
    public string cardDescription;

    public CardType cardType;

    public GameObject cardPref;
}

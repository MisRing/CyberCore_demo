using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HitAndRun : Card
{
    [SerializeField]
    private BattleStyle newBattleStyle;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
    }

    public override void PlayCard(UnitController player)
    {
        DeckManager.instance.nextBattleStyle = newBattleStyle;

        base.PlayCard(player);
    }
}

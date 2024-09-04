using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StepCard : Card
{
    private Vector3Int castCellPos;
    private bool availableCell = false;

    [SerializeField]
    private int stepRange = 4;
    [SerializeField]
    private bool canJump = false;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if (RoundController.instance.enemyTurn)
            return;
        StartCoroutine(MoveToCastSlot());
        HighlightAvailableCells();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (RoundController.instance.enemyTurn)
            return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = BattleGridManager.instance.tilemap.WorldToCell(worldPos);
        tilePos.z = 0;

        if (BattleGridManager.instance.tilemap.HasTile(tilePos))
        {
            if (castCellPos != tilePos)
                GridUIManager.instance.ChangeTarget(tilePos);

            availableCell = true;
            castCellPos = tilePos;
        }
        else
        {
            castCellPos = Vector3Int.zero;
            availableCell = false;

            GridUIManager.instance.StopTarget();
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        StopAllCoroutines();
        cardUI.updateStatus = true;
        cardUI.CloseCard(eventData);

        if (RoundController.instance.enemyTurn)
            return;

        GridUIManager.instance.DeliteHightLights();

        TryCast();

        GridUIManager.instance.StopTarget();
    }

    private void TryCast()
    {
        GetComponent<RectTransform>().localPosition = Vector3.zero;


        if (availableCell)
        {
            List<Vector3Int> allCells = new List<Vector3Int>();
            List<Vector3Int> lastCells = new List<Vector3Int>();

            PathFind.GetRadiusWay(BattleGridManager.instance.tilemap,
                                    RoundController.instance.player.currentGridPosition,
                                    stepRange,
                                    ref allCells,
                                    ref lastCells,
                                    canJump);

            if(allCells.Contains(castCellPos))
                PlayCard(RoundController.instance.player);
            else
                cardRectTransform.anchoredPosition = startPosition;
        }
        else
        {
            cardRectTransform.anchoredPosition = startPosition;
        }
    }

    private void HighlightAvailableCells()
    {
        List<Vector3Int> allCells = new List<Vector3Int>();
        List<Vector3Int> lastCells = new List<Vector3Int>();

        PathFind.GetRadiusWay(BattleGridManager.instance.tilemap,
                                RoundController.instance.player.currentGridPosition,
                                stepRange,
                                ref allCells,
                                ref lastCells,
                                canJump);


        GridUIManager.instance.GenerateHightLights(allCells, TargetState.AvThrough);
        GridUIManager.instance.GenerateHightLights(lastCells, TargetState.Available);
    }

    public override void PlayCard(UnitController player)
    {
        player.MoveUnit(new Vector2Int(castCellPos.x, castCellPos.y), canJump);

        base.PlayCard(player);
    }
}

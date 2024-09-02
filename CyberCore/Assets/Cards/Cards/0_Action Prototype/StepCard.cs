using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class StepCard : Card
{
    private Vector3Int castCellPos;
    private bool availableCell = false;

    [SerializeField]
    private int stepRange = 4;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        StartCoroutine(MoveToCastSlot());
        HighlightAvailableCells();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        //base.OnDrag(eventData);

        //UpdateArrow(eventData);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = BattleGridManager.instance.tilemap.WorldToCell(worldPos);
        tilePos.z = 0;

        if (BattleGridManager.instance.tilemap.HasTile(tilePos))
        {
            if (castCellPos != tilePos)
                GridUIManager.instance.ChangeTarget(tilePos);

            availableCell = true;
            castCellPos = tilePos;

            //List<Vector3Int> poseList = new List<Vector3Int>()
            //{
            //    castCellPos
            //};

            //GridUIManager.instance.GenerateHightLights(poseList, GridState.Enemy);
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

        GridUIManager.instance.DeliteHightLights();

        TryCast();

        GridUIManager.instance.StopTarget();

        //DestroyArrow();
    }

    private void TryCast()
    {
        GetComponent<RectTransform>().localPosition = Vector3.zero;

        ////canvasGroup.blocksRaycasts = true;
        ////base.OnEndDrag(eventData);

        if (availableCell)
        {
            List<Vector3Int> allCells = new List<Vector3Int>();
            List<Vector3Int> lastCells = new List<Vector3Int>();

            PathFind.GetRadiusWay(BattleGridManager.instance.tilemap,
                                    RoundController.instance.player.currentGridPosition,
                                    stepRange,
                                    ref allCells,
                                    ref lastCells);

            if(allCells.Contains(castCellPos))
                PlayCard(RoundController.instance.player);
            else
                cardRectTransform.anchoredPosition = startPosition;
        }
        else
        {
            cardRectTransform.anchoredPosition = startPosition; // Возвращаем карту в руку
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
                                ref lastCells);


        GridUIManager.instance.GenerateHightLights(allCells, TargetState.AvThrough);
        GridUIManager.instance.GenerateHightLights(lastCells, TargetState.Available);
    }

    public override void PlayCard(UnitController player)
    {
        //// Логика перемещения игрока на выбранную клетку
        player.MoveUnit(new Vector2Int(castCellPos.x, castCellPos.y));

        base.PlayCard(player);
    }
}

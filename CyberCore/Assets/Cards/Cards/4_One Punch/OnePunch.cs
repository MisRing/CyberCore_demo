using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnePunch : Card
{
    private Vector3Int castCellPos;
    private bool availableCell = false;

    [SerializeField]
    private int attackRange = 1;
    [SerializeField]
    private uint attackCount = 2;
    [SerializeField]
    private int damage = 10;
    [SerializeField]
    private float attackDelay = 0.1f;


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
            List<Vector3Int> visibleCells = new List<Vector3Int>();
            List<Vector3Int> visibleEdge = new List<Vector3Int>();

            PathFind.GetVisibleArea(BattleGridManager.instance.tilemap,
                                    RoundController.instance.player.currentGridPosition,
                                    attackRange,
                                    ref visibleCells,
                                    ref visibleEdge,
                                    "Enemy");

            if (visibleCells.Contains(castCellPos))
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
        List<Vector3Int> visibleTargetCells = new List<Vector3Int>();
        List<Vector3Int> VisibleCells = new List<Vector3Int>();

        PathFind.GetVisibleArea(BattleGridManager.instance.tilemap,
                                RoundController.instance.player.currentGridPosition,
                                attackRange,
                                ref VisibleCells,
                                ref visibleTargetCells);

        if (visibleTargetCells.Count > 0)
            GridUIManager.instance.GenerateHightLights(VisibleCells, TargetState.TargetRange);

        visibleTargetCells = new List<Vector3Int>();
        VisibleCells = new List<Vector3Int>();

        PathFind.GetVisibleArea(BattleGridManager.instance.tilemap,
                                RoundController.instance.player.currentGridPosition,
                                attackRange,
                                ref visibleTargetCells,
                                ref VisibleCells,
                                "Enemy");

        if (visibleTargetCells.Count > 0)
            GridUIManager.instance.GenerateHightLights(visibleTargetCells, TargetState.Target);
    }

    public override void PlayCard(UnitController player)
    {
        for (int i = 1; i <= attackCount; i++)
        {
            RoundController.instance.player.StartCoroutine(
                ShootAnimation(attackDelay * i,
                               BattleGridManager.instance.GetCellEntity(new Vector2Int(castCellPos.x, castCellPos.y)).GetComponent<EnemyController>(),
                               damage)
                );
        }

        base.PlayCard(player);
    }

    private static IEnumerator ShootAnimation(float delay, EnemyController enemy, int damage)
    {
        yield return new WaitForSeconds(delay);

        if(enemy != null)
            if(enemy.TakeDamage(damage))
                RoundController.instance.EnemyDefeated(enemy);
    }
}
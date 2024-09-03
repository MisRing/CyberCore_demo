using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleFire : Card
{
    private Vector3Int castCellPos;
    private bool availableCell = false;

    [SerializeField]
    private int shootRange = 5;
    [SerializeField]
    private uint bullets = 2;
    [SerializeField]
    private int damage = 3;
    [SerializeField]
    private float shootDelay = 0.1f;
    [SerializeField]
    private float bulletSpeed = 10f;

    [SerializeField]
    private GameObject bulletPref;


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
                                    shootRange,
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
                                shootRange,
                                ref VisibleCells,
                                ref visibleTargetCells);

        if (visibleTargetCells.Count > 0)
            GridUIManager.instance.GenerateHightLights(VisibleCells, TargetState.TargetRange);

        visibleTargetCells = new List<Vector3Int>();
        VisibleCells = new List<Vector3Int>();

        PathFind.GetVisibleArea(BattleGridManager.instance.tilemap,
                                RoundController.instance.player.currentGridPosition,
                                shootRange,
                                ref visibleTargetCells,
                                ref VisibleCells,
                                "Enemy");

        if (visibleTargetCells.Count > 0)
            GridUIManager.instance.GenerateHightLights(visibleTargetCells, TargetState.Target);
    }

    public override void PlayCard(UnitController player)
    {
        for (int i = 1; i <= bullets; i++)
        {
            RoundController.instance.player.StartCoroutine(
                ShootAnimation(shootDelay * i,
                               RoundController.instance.player.transform.position,
                               BattleGridManager.instance.GetCellEntity(new Vector2Int(castCellPos.x, castCellPos.y)).GetComponent<EnemyController>(),
                               bulletPref,
                               bulletSpeed,
                               damage)
                );
        }

        base.PlayCard(player);
    }

    private static IEnumerator ShootAnimation(float delay, Vector3 startPosition, EnemyController enemy, GameObject bulletPref, float bulletSpeed, int damage)
    {
        yield return new WaitForSeconds(delay);

        startPosition.z = -3;
        startPosition.y += 0.5f;
        GameObject bullet = Instantiate(bulletPref, startPosition, Quaternion.identity);

        Vector3 target = enemy.transform.position;
        target.z = -3;
        target.y += 0.5f;
        target += new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0);

        Vector3 direction = (target - bullet.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        while (Vector3.Distance(bullet.transform.position, target) > 0.01f)
        {
            bullet.transform.position = Vector3.MoveTowards(bullet.transform.position, target, bulletSpeed * Time.deltaTime);

            yield return null;
        }

        Destroy(bullet);

        if(enemy != null)
            if(enemy.TakeDamage(damage))
                RoundController.instance.EnemyDefeated(enemy);
    }
}
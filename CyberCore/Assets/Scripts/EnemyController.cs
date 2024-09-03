using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : UnitController
{
    [SerializeField]
    private int shootRange = 3;
    [SerializeField]
    private int moveDistance = 5;

    [SerializeField]
    private uint bullets = 3;
    [SerializeField]
    private int damage = 2;
    [SerializeField]
    private float shootDelay = 0.1f;
    [SerializeField]
    private float bulletSpeed = 1f;

    [SerializeField]
    private GameObject bulletPref;


    public void StartAction()
    {
        MoveAction();
    }

    public override void EndMove()
    {
        List<Vector3Int> visibleCells = new List<Vector3Int>();
        List<Vector3Int> visibleEdge = new List<Vector3Int>();

        PathFind.GetVisibleArea(BattleGridManager.instance.tilemap,
                                currentGridPosition,
                                shootRange,
                                ref visibleCells,
                                ref visibleEdge);

        Vector3Int playerPos = new Vector3Int(RoundController.instance.player.currentGridPosition.x,
                                              RoundController.instance.player.currentGridPosition.y,
                                              0);

        //GridUIManager.instance.DeliteHightLights();
        //GridUIManager.instance.GenerateHightLights(visibleEdge, TargetState.Enemy);

        if (visibleCells.Contains(playerPos))
            Shoot();
        else
            EndActions();
    }

    public void Shoot()
    {
        bool end = false;
        for (int i = 1; i <= bullets; i++)
        {
            if(i == bullets)
                end = true;
            StartCoroutine(ShootAnimation(shootDelay * i, end));
        }
    }

    public IEnumerator ShootAnimation(float delay, bool endShooting)
    {
        yield return new WaitForSeconds(delay);

        Vector3 startPosition = transform.position;
        startPosition.z = -3;
        startPosition.y += 0.5f;
        GameObject bullet = Instantiate(bulletPref, startPosition, Quaternion.identity);

        Vector3 target = RoundController.instance.player.transform.position;
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

        if(!RoundController.instance.player.TakeDamage(damage))
        {
            if(endShooting)
            EndActions();
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
            RoundController.instance.PlayerLost();
        }
    }

    private void MoveAction()
    {
        List<Vector3Int> visibleEdge = new List<Vector3Int>();

        PathFind.GetVisibleAreaEdge(BattleGridManager.instance.tilemap,
                                    RoundController.instance.player.currentGridPosition,
                                    shootRange,
                                    ref visibleEdge);

        List<Vector3Int> shortestPath = new List<Vector3Int>();

        foreach (var cell in visibleEdge)
        {
            List<Vector3Int> path = PathFind.GetShortestPath(BattleGridManager.instance.tilemap,
                                                                currentGridPosition,
                                                                new Vector2Int(cell.x, cell.y));

            if (path.Count == 0)
                continue;

            if (shortestPath.Count == 0 || shortestPath.Count > path.Count)
                shortestPath = path;
        }

        if (shortestPath.Count == 0)
        {
            Debug.Log("No path.");
            EndActions();
            return;
        }

        if (shortestPath.Count > moveDistance)
        {
            shortestPath.RemoveRange(moveDistance, shortestPath.Count - moveDistance);
        }

        MoveUnit(shortestPath);
    }

    public override void EndActions()
    {
        base.EndActions();

        StartCoroutine(RoundController.instance.NextEnemyAction());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : UnitController
{
    [SerializeField]
    private int shootRange = 3;
    [SerializeField]
    private int moveDistance = 5;


    public void StartAction()
    {
        List<Vector3Int> lastCells = new List<Vector3Int>();

        PathFind.GetLastRadiusWay(BattleGridManager.instance.tilemap,
                                  RoundController.instance.player.currentGridPosition,
                                  shootRange,
                                  ref lastCells);

        List<Vector3Int> shortestPath = new List<Vector3Int>();

        foreach (var cell in lastCells)
        {
            List<Vector3Int> path = PathFind.GetShortestPath(BattleGridManager.instance.tilemap,
                                                                currentGridPosition,
                                                                new Vector2Int(cell.x, cell.y));

            if(shortestPath.Count == 0 || shortestPath.Count > path.Count)
                shortestPath = path;
        }
        if (shortestPath.Count > moveDistance)
            shortestPath.RemoveRange(moveDistance, shortestPath.Count - moveDistance);

        MoveUnit(shortestPath);
    }

    public override void EndActions()
    {
        base.EndActions();

        StartCoroutine(RoundController.instance.NextEnemyAction());
    }
}

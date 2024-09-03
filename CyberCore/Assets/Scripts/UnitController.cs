using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField, Range(0, 100)]
    private int currentHealth = 10;
    [SerializeField, Range(0, 100)]
    private int maxHealth = 10;

    public Vector2Int currentGridPosition;

    public float moveSpeed = 5f;

    private Queue<List<Vector3Int>> moveQueue = new Queue<List<Vector3Int>>();
    private bool inPath = false;

    public void MoveUnit(Vector2Int newPosition)
    {
        List<Vector3Int> path = PathFind.GetShortestPath(BattleGridManager.instance.tilemap,
                                                                currentGridPosition,
                                                                newPosition);

        BattleGridManager.instance.MoveEntity(currentGridPosition, newPosition, gameObject);

        currentGridPosition = newPosition;

        if(inPath)
            moveQueue.Enqueue(path);
        else
            StartCoroutine(MoveToPosition(path));
    }

    public void MoveUnit(List<Vector3Int> path)
    {
        BattleGridManager.instance.MoveEntity(currentGridPosition,
                                              new Vector2Int(path[path.Count - 1].x, path[path.Count - 1].y),
                                              gameObject);

        currentGridPosition = new Vector2Int(path[path.Count - 1].x, path[path.Count - 1].y);
        
        if (inPath)
            moveQueue.Enqueue(path);
        else
            StartCoroutine(MoveToPosition(path));
    }

    private IEnumerator MoveToPosition(List<Vector3Int> path)
    {
        inPath = true;

        foreach (var nextCell in path)
        {
            Vector3 nextDot = BattleGridManager.instance.tilemap.CellToWorld(nextCell) + Vector3.up * 0.5f;

            while (Vector3.Distance(transform.position, nextDot) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextDot, moveSpeed * Time.deltaTime);

                yield return null;
            }
        }

        transform.position = BattleGridManager.instance.tilemap.CellToWorld(path[path.Count - 1]) + Vector3.up * 0.5f;


        if (moveQueue.Count > 0)
        {
            yield return new WaitForSeconds(0.2f);
            yield return MoveToPosition(moveQueue.Dequeue());
        }
        
        inPath = false;

        EndMove();
    }

    public virtual void EndMove()
    {
        EndActions();
    }

    public virtual void EndActions()
    {

    }



    public void SetUnitPosition(int x, int y)
    {
        currentGridPosition = new Vector2Int(x, y);
        BattleGridManager.instance.AddEntity(currentGridPosition, gameObject);
        transform.position = BattleGridManager.instance.tilemap.CellToWorld(new Vector3Int(x, y)) + Vector3.back + Vector3.up * 0.5f;
    }
}

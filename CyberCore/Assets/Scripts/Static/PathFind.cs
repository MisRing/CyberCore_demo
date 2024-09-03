using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Priority_Queue;

public static class PathFind
{
    public static void GetRadiusWay(Tilemap tileMap,
                                    Vector2Int start,
                                    int maxRadius,
                                    ref List<Vector3Int> allCells,
                                    ref List<Vector3Int> lastCells)
    {
        var queue = new Queue<Vector2Int>();
        var distances = new Dictionary<Vector2Int, int>();

        queue.Enqueue(start);
        distances[start] = 0;
        allCells.Add(new Vector3Int(start.x, start.y));

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            int currentDistance = distances[current];

            if (currentDistance == maxRadius)
            {
                lastCells.Add(new Vector3Int(current.x, current.y));
                continue;
            }

            foreach (var direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int nextPos = current + direction;

                Vector3Int nextTile = new Vector3Int(nextPos.x, nextPos.y);
                
                if (tileMap.HasTile(nextTile)
                    && tileMap.GetTile<CustomTile>(nextTile).tileState == TileState.Walkable
                    && !BattleGridManager.instance.CheckCellForEntity(nextPos))
                {
                    if (!distances.ContainsKey(nextPos) || currentDistance + 1 < distances[nextPos])
                    {
                        distances[nextPos] = currentDistance + 1;
                        queue.Enqueue(nextPos);
                        allCells.Add(new Vector3Int(nextPos.x, nextPos.y));
                    }
                }
            }
        }
    }

    public static void GetRadiusWayEdge(Tilemap tileMap,
                                    Vector2Int start,
                                    int maxRadius,
                                    ref List<Vector3Int> lastCells)
    {
        var queue = new Queue<Vector2Int>();
        var distances = new Dictionary<Vector2Int, int>();

        queue.Enqueue(start);
        distances[start] = 0;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            int currentDistance = distances[current];

            if (currentDistance == maxRadius)
            {
                lastCells.Add(new Vector3Int(current.x, current.y));
                continue;
            }

            foreach (var direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int nextPos = current + direction;

                Vector3Int nextTile = new Vector3Int(nextPos.x, nextPos.y);

                if (tileMap.HasTile(nextTile)
                    && tileMap.GetTile<CustomTile>(nextTile).tileState == TileState.Walkable
                    && !BattleGridManager.instance.CheckCellForEntity(nextPos))
                {
                    if (!distances.ContainsKey(nextPos) || currentDistance + 1 < distances[nextPos])
                    {
                        distances[nextPos] = currentDistance + 1;
                        queue.Enqueue(nextPos);
                    }
                }
            }
        }
    }

    public static List<Vector3Int> GetShortestPath(Tilemap tileMap,
                                               Vector2Int start,
                                               Vector2Int end)
    {
        var openSet = new SimplePriorityQueue<Vector2Int, int>();
        var gScore = new Dictionary<Vector2Int, int>();
        var fScore = new Dictionary<Vector2Int, int>();
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        openSet.Enqueue(start, 0);
        gScore[start] = 0;
        fScore[start] = Heuristic(start, end);

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (current == end)
            {
                return ReconstructPath(cameFrom, current);
            }

            foreach (var direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int neighbor = current + direction;
                Vector3Int neighborTile = new Vector3Int(neighbor.x, neighbor.y, 0);

                if (tileMap.HasTile(neighborTile)
                    && tileMap.GetTile<CustomTile>(neighborTile).tileState == TileState.Walkable
                    && !BattleGridManager.instance.CheckCellForEntity(neighbor))
                {
                    int tentativeGScore = gScore[current] + 1;

                    if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = tentativeGScore + Heuristic(neighbor, end);

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Enqueue(neighbor, fScore[neighbor]);
                        }
                    }
                }
            }
        }

        return new List<Vector3Int>();
    }

    private static int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static List<Vector3Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        var path = new List<Vector3Int> { new Vector3Int(current.x, current.y, 0) };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, new Vector3Int(current.x, current.y, 0));
        }

        return path;
    }

    public static void GetVisibleArea(Tilemap tileMap,
                                  Vector2Int start,
                                  int maxRadius,
                                  ref List<Vector3Int> allVisibleCells,
                                  ref List<Vector3Int> edgeCells)
    {
        var queue = new Queue<Vector2Int>();
        var distances = new Dictionary<Vector2Int, int>();

        queue.Enqueue(start);
        distances[start] = 0;
        allVisibleCells.Add(new Vector3Int(start.x, start.y, 0));

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            int currentDistance = distances[current];

            if (currentDistance == maxRadius)
            {
                edgeCells.Add(new Vector3Int(current.x, current.y, 0));
                continue;
            }

            foreach (var direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int nextPos = current + direction;
                Vector3Int nextTile = new Vector3Int(nextPos.x, nextPos.y, 0);

                // Проверка проходимости клетки и видимости
                if (tileMap.HasTile(nextTile)
                    && tileMap.GetTile<CustomTile>(nextTile).tileState == TileState.Walkable
                    && IsVisible(start, nextPos, tileMap))
                {
                    if (!distances.ContainsKey(nextPos) || currentDistance + 1 < distances[nextPos])
                    {
                        distances[nextPos] = currentDistance + 1;
                        queue.Enqueue(nextPos);
                        allVisibleCells.Add(new Vector3Int(nextPos.x, nextPos.y, 0));
                    }
                }
            }
        }
    }

    public static void GetVisibleAreaEdge(Tilemap tileMap,
                                  Vector2Int start,
                                  int maxRadius,
                                  ref List<Vector3Int> edgeCells)
    {
        var queue = new Queue<Vector2Int>();
        var distances = new Dictionary<Vector2Int, int>();

        queue.Enqueue(start);
        distances[start] = 0;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            int currentDistance = distances[current];

            if (currentDistance == maxRadius)
            {
                edgeCells.Add(new Vector3Int(current.x, current.y, 0));
                continue;
            }

            foreach (var direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int nextPos = current + direction;
                Vector3Int nextTile = new Vector3Int(nextPos.x, nextPos.y, 0);

                // Проверка проходимости клетки и видимости
                if (tileMap.HasTile(nextTile)
                    && tileMap.GetTile<CustomTile>(nextTile).tileState == TileState.Walkable
                    && IsVisible(start, nextPos, tileMap))
                {
                    if (!distances.ContainsKey(nextPos) || currentDistance + 1 < distances[nextPos])
                    {
                        distances[nextPos] = currentDistance + 1;
                        queue.Enqueue(nextPos);
                    }
                }
            }
        }
    }

    // Метод для проверки видимости между двумя клетками
    private static bool IsVisible(Vector2Int start, Vector2Int end, Tilemap tileMap)
    {
        // Алгоритм Брезенхэма для проверки видимости по прямой линии
        Vector2Int diff = end - start;
        int dx = Mathf.Abs(diff.x);
        int dy = Mathf.Abs(diff.y);
        int sx = diff.x > 0 ? 1 : -1;
        int sy = diff.y > 0 ? 1 : -1;
        int err = dx - dy;

        Vector2Int currentPos = start;

        while (currentPos != end)
        {
            Vector3Int currentTile = new Vector3Int(currentPos.x, currentPos.y, 0);

            // Если клетка на пути не проходима, то видимости нет
            if (tileMap.HasTile(currentTile) && tileMap.GetTile<CustomTile>(currentTile).tileState == TileState.Obstacle)
            {
                return false;
            }

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                currentPos.x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                currentPos.y += sy;
            }
        }

        return true;
    }


}

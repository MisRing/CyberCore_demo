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
        // Очередь с приоритетом для обработки клеток
        var queue = new Queue<Vector2Int>();
        // Словарь для хранения минимальных расстояний до клеток
        var distances = new Dictionary<Vector2Int, int>();

        // Инициализация
        queue.Enqueue(start);
        distances[start] = 0;
        allCells.Add(new Vector3Int(start.x, start.y));

        // Пока очередь не пуста
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            int currentDistance = distances[current];

            // Если достигнут максимальный радиус, добавляем клетку в lastCells
            if (currentDistance == maxRadius)
            {
                lastCells.Add(new Vector3Int(current.x, current.y));
                continue;
            }

            // Проверка всех соседей (вверх, вниз, влево, вправо)
            foreach (var direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int nextPos = current + direction;

                Vector3Int nextTile = new Vector3Int(nextPos.x, nextPos.y);
                // Проверяем границы сетки и проходимость клетки
                if (tileMap.HasTile(nextTile)
                    && tileMap.GetTile<CustomTile>(nextTile).tileState == TileState.Walkable
                    && !BattleGridManager.instance.CheckCellForEntity(nextPos))
                {
                    // Если клетка еще не посещена или найден более короткий путь
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

    public static void GetLastRadiusWay(Tilemap tileMap,
                                    Vector2Int start,
                                    int maxRadius,
                                    ref List<Vector3Int> lastCells)
    {
        // Очередь с приоритетом для обработки клеток
        var queue = new Queue<Vector2Int>();
        // Словарь для хранения минимальных расстояний до клеток
        var distances = new Dictionary<Vector2Int, int>();

        // Инициализация
        queue.Enqueue(start);
        distances[start] = 0;

        // Пока очередь не пуста
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            int currentDistance = distances[current];

            // Если достигнут максимальный радиус, добавляем клетку в lastCells
            if (currentDistance == maxRadius)
            {
                lastCells.Add(new Vector3Int(current.x, current.y));
                continue;
            }

            // Проверка всех соседей (вверх, вниз, влево, вправо)
            foreach (var direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int nextPos = current + direction;

                Vector3Int nextTile = new Vector3Int(nextPos.x, nextPos.y);
                // Проверяем границы сетки и проходимость клетки
                if (tileMap.HasTile(nextTile)
                    && tileMap.GetTile<CustomTile>(nextTile).tileState == TileState.Walkable
                    && !BattleGridManager.instance.CheckCellForEntity(nextPos))
                {
                    // Если клетка еще не посещена или найден более короткий путь
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
        // Открытый список (open set) для клеток, которые нужно исследовать
        var openSet = new SimplePriorityQueue<Vector2Int, int>();
        // Словарь, хранящий пришедшую из клетки стоимость движения
        var gScore = new Dictionary<Vector2Int, int>();
        // Словарь, хранящий общую стоимость (gScore + эвристика)
        var fScore = new Dictionary<Vector2Int, int>();
        // Словарь для отслеживания пути
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        // Инициализация начальной точки
        openSet.Enqueue(start, 0);
        gScore[start] = 0;
        fScore[start] = Heuristic(start, end);

        while (openSet.Count > 0)
        {
            // Получение клетки с минимальным fScore
            var current = openSet.Dequeue();

            // Если достигли цели, восстанавливаем путь
            if (current == end)
            {
                return ReconstructPath(cameFrom, current);
            }

            // Проверка всех соседей (вверх, вниз, влево, вправо)
            foreach (var direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int neighbor = current + direction;
                Vector3Int neighborTile = new Vector3Int(neighbor.x, neighbor.y, 0);

                // Проверяем, есть ли тайл на данной позиции и является ли он проходимым
                if (tileMap.HasTile(neighborTile)
                    && tileMap.GetTile<CustomTile>(neighborTile).tileState == TileState.Walkable
                    && !BattleGridManager.instance.CheckCellForEntity(neighbor))
                {
                    int tentativeGScore = gScore[current] + 1;

                    if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                    {
                        // Обновляем путь к соседней клетке
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

        // Если путь не найден, возвращаем пустой список
        return new List<Vector3Int>();
    }

    // Эвристическая функция для A* (манхэттенское расстояние)
    private static int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // Восстановление пути от конечной точки к начальной
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

}

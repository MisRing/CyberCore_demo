using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BattleGridManager : MonoBehaviour
{
    public static BattleGridManager instance;

    public Tilemap tilemap;

    [SerializeField]
    private Dictionary<Vector2Int, GameObject> entities = new Dictionary<Vector2Int, GameObject>();

    private void Awake()
    {
        instance = this;
    }

    public void AddEntity(Vector2Int cellPosition, GameObject entity)
    {
        entities.Add(cellPosition, entity);
    }

    public void MoveEntity(Vector2Int oldPosition, Vector2Int newPosition, GameObject entity)
    {
        entities.Remove(oldPosition);
        entities.Add(newPosition, entity);
    }

    public void DeletEntity(Vector2Int cellPosition)
    {
        entities.Remove(cellPosition);
    }

    public bool CheckCellForEntity(Vector2Int cellPosition)
    {
        return entities.ContainsKey(cellPosition);
    }

    public GameObject GetCellEntity(Vector2Int cellPosition)
    {
        if (entities.ContainsKey(cellPosition))
            return entities[cellPosition];

        return null;
    }

    public TileBase GetTile(Vector2Int position)
    {
        return tilemap.GetTile(new Vector3Int(position.x, position.y, 0));
    }

    public List<TileBase> GetAllCellsAsList()
    {
        List<TileBase> cells = new List<TileBase>();
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        cells.AddRange(allTiles);
        return cells;
    }
}

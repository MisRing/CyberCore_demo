using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "New Custom Tile", menuName = "Tiles/CustomTile")]
public class CustomTile : Tile
{
    public TileState tileState = 0;
}

public enum TileState
{
    Walkable = 0,
    Obstacle = 1,
    Fall = 2,
}



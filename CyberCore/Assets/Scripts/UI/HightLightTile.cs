using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New HightLight", menuName = "Tiles/HightLight")]
public class HightLightTile : Tile
{
    [SerializeField]
    public TargetState state = TargetState.None;
}

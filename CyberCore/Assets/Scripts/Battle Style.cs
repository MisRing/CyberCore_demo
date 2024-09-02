using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewBattleStyle", menuName = "Project Data/BattleStyle")]
public class BattleStyle : ScriptableObject
{
    public string styleName = "default";

    public ushort hackCards = 1;
    public ushort attackCards = 1;
    public ushort actionCards = 1;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StateColors
{
    private static Color availableCol = new Color(0.0f, 1f, 0.05f, 0.7803922f);
    private static Color avThroughCol = new Color(0.0f, 0.56f, 0.06f, 0.7803922f);
    private static Color enemyCol = new Color(1f, 0f, 0f, 0.7803922f);
    private static Color obstacleCol = new Color(0.47f, 0.16f, 0.51f, 0.7803922f);
    private static Color targetRangeCol = new Color(0.55f, 0.1f, 0.1f, 0.7803922f);

    public static Color GetStateColor(TargetState state)
    {
        switch (state)
        {
            case TargetState.None:
                return Color.white;
            case TargetState.Available:
                return availableCol;
            case TargetState.AvThrough:
                return avThroughCol;
            case TargetState.Target:
                return enemyCol;
            case TargetState.Obstacle:
                return obstacleCol;
            case TargetState.TargetRange:
                return targetRangeCol;

            default:
                return Color.white;
        }
    }
}

[System.Serializable]
public enum TargetState
{
    None = 0,
    Available = 1,
    AvThrough = 2,
    Target = 3,
    Obstacle = 4,
    TargetRange = 5,
}
using UnityEditor;

[CustomEditor(typeof(HightLightTile))]
public class HightLightTileInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        HightLightTile tile = (HightLightTile)target;
        tile.color = StateColors.GetStateColor(tile.state);

        serializedObject.ApplyModifiedProperties();
    }
}

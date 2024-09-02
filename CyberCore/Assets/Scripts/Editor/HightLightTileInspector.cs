using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HightLightTile))]
public class HightLightTileInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // Обновляем состояние

        // Отображаем стандартное представление
        DrawDefaultInspector();

        // Если необходимо, можно добавить дополнительное отображение
        // Например, вывод текущего цвета
        HightLightTile tile = (HightLightTile)target;
        tile.color = StateColors.GetStateColor(tile.state);

        serializedObject.ApplyModifiedProperties();
    }
}

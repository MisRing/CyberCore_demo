using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridUIManager : MonoBehaviour
{
    public static GridUIManager instance;

    [SerializeField]
    private Tilemap uiTileMap;

    [SerializeField]
    private List<HightLightTile> hightLightTiles;

    [SerializeField]
    private GameObject targetPref;

    private Transform targetObject;

    [SerializeField]
    private GameObject targetLinePref;

    private TargetLineUI targetLine;

    [SerializeField]
    private float targetSpeed = 5f;

    private void Awake()
    {
        instance = this;
    }

    public void GenerateHightLights(List<Vector3Int> positions, TargetState gridState)
    {
        for(int i = 0; i < positions.Count; i++)
        {
            uiTileMap.SetTile(positions[i], null);
            uiTileMap.SetTile(positions[i], hightLightTiles[(int)gridState]);
        }
    }

    public void DeliteHightLights()
    {
        uiTileMap.ClearAllTiles();
    }

    public void ChangeTarget(Vector3Int position)
    {
        Vector3 targetNewPosition = BattleGridManager.instance.tilemap.CellToWorld(position);

        if (targetNewPosition.x < CardCastUI.instance.cardCastSlot.TransformPoint(Vector3.right * CardCastUI.instance.cardCastSlot.rect.width).x)
        {
            return;
        }

        if (targetLine == null)
            targetLine = Instantiate(targetLinePref, CardCastUI.instance.cardCastSlot).GetComponent<TargetLineUI>();

        StopAllCoroutines();
        targetLine.StopAllCoroutines();

        targetNewPosition += Vector3.up * 0.5f;
        targetNewPosition.z = -1f;

        if (targetObject == null)
            targetObject = Instantiate(targetPref, targetNewPosition, Quaternion.identity).transform;

        targetLine.UpdatePosition(targetNewPosition + new Vector3(-0.5f, 0.25f), targetSpeed);

        StartCoroutine(MoveTarget(targetNewPosition));
    }

    public IEnumerator MoveTarget(Vector3 position)
    {

        // Пока объект не достигнет целевой позиции
        while (Vector3.Distance(targetObject.position, position) > 0.01f)
        {
            // Плавное перемещение объекта к целевой позиции
            targetObject.position = Vector3.Lerp(targetObject.position, position, targetSpeed * Time.deltaTime);

            // Ждать до следующего кадра
            yield return null;
        }

        // Убедиться, что объект точно на целевой позиции
        targetObject.position = position;
    }

    public void StopTarget()
    {
        StopAllCoroutines();
        if (targetObject != null)
            Destroy(targetObject.gameObject);

        if (targetLine != null)
        {
            targetLine.StopAllCoroutines();
            Destroy(targetLine.gameObject);
        }
    }
}

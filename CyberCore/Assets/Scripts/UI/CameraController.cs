using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1.0f;

    void Update()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        direction = Vector3.Normalize(direction);
        direction = direction * moveSpeed * Time.deltaTime;
        //direction.z = transform.position.z;

        transform.position += direction;
    }
}

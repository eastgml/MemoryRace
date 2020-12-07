using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;
    Quaternion rotation;
    // Start is called before the first frame update
    void Start()
    {
        rotation = transform.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 camPosition = playerTransform.position;
        camPosition.y = 0f;
        transform.position = camPosition + new Vector3(0f, 4f, -10f);
        transform.rotation = rotation;
    }
}

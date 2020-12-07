using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;
    Quaternion rotation;
    void Awake() {
        rotation = transform.rotation;
    }

/*    void FixedUpdate() {
        if (playerTransform != null) {
            gameObject.transform.position = playerTransform.position + new Vector3(0f, 10f, -20f);
        }
    }*/

    void LateUpdate() {
        if (playerTransform != null)
        {
            Vector3 camPos = playerTransform.position;
            camPos.y = 0;
            gameObject.transform.position = camPos + new Vector3(0f, 4f, -6f);
        }

        transform.rotation = rotation;

    }
    
    public void setTarget(Transform target) {
        playerTransform = target;
    }
}

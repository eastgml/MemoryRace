using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileChecker : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Collider collider = collision.collider;
        if (collider.CompareTag("Tile"))
        {
            Tile tile = collider.gameObject.GetComponent<Tile>();
            if (!tile.isMelting)
            {
                tile.onStepped();
            }
        }
    }
}

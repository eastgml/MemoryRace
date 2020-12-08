using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    PlayerController playerController;
    public GameObject glassItem;
    public GameObject clockItem;

    void Awake() {
        playerController = GetComponentInParent<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other == playerController.gameObject)
        {
            return;
        }

        if (other.gameObject.CompareTag("ClockItem") || other.gameObject.CompareTag("DistortionResetter"))
        {
            return;
        }

        if (other.gameObject.CompareTag("Tile"))
        {
            Tile tile = other.gameObject.GetComponent<Tile>();
            if (tile.isInstaDeath && tile.isBad && !tile.timeExtended)
            {
                playerController.SetGroundedState(false);
                return;
            }
        }

        playerController.SetGroundedState(true);

    }

    void OnTriggerExit(Collider other)
    {
        if (other == playerController.gameObject)
        {
            return;
        }

        if (other.gameObject.CompareTag("ClockItem") || other.gameObject.CompareTag("DistortionResetter"))
        {
            return;
        }

        playerController.SetGroundedState(false);
    }

    void OnTriggerStay(Collider other)
    {
        if (other == playerController.gameObject)
        {
            return;
        }

        if (other.gameObject.CompareTag("ClockItem") || other.gameObject.CompareTag("DistortionResetter")) {
            return;
        }

        if (other.gameObject.CompareTag("Tile"))
        {
            Tile tile = other.gameObject.GetComponent<Tile>();
            if (tile.isInstaDeath && tile.isBad && !tile.timeExtended)
            {
                playerController.SetGroundedState(false);
                return;
            }
        }

        playerController.SetGroundedState(true);
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject == playerController.gameObject)
        {
            return;
        }

        Collider collider = collision.collider;

        if (collider.CompareTag("DistortionResetter") || collider.CompareTag("ClockItem")) {
            return;
        }


        if (collider.CompareTag("Tile"))
        {
            Tile tile = collider.gameObject.GetComponent<Tile>();
            if (tile.isInstaDeath && tile.isBad && !tile.timeExtended)
            {
                playerController.SetGroundedState(false);
                return;
            }
        }

        playerController.SetGroundedState(true);
    }

    void OnCollisionExit(Collision collision) {
        if (collision.gameObject == playerController.gameObject)
        {
            return;
        }

        Collider collider = collision.collider;
        
        if (collider.CompareTag("DistortionResetter") || collider.CompareTag("ClockItem"))
        {
            return;
        }

        playerController.SetGroundedState(false);
    }

    void OnCollisionStay(Collision collision) {
        if (collision.gameObject == playerController.gameObject)
        {
            return;
        }

        Collider collider = collision.collider;

        if (collider.CompareTag("DistortionResetter") || collider.CompareTag("ClockItem"))
        {
            return;
        }

        if (collider.CompareTag("Tile"))
        {
            Tile tile = collider.gameObject.GetComponent<Tile>();
            if (tile.isInstaDeath && tile.isBad && !tile.timeExtended)
            {
                playerController.SetGroundedState(false);
                return;
            }
        }

        playerController.SetGroundedState(true);
    }
}

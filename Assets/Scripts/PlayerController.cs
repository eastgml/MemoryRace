using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    Rigidbody rb;
    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;
    PhotonView PV;
    int numClockItems = 0;
    public Text clockItemText;
    int numPublicMarkers = 5;
    public Text publicMarkerText;
    public Color publicMarkerColor;
    int numPrivateMarkers = 5;
    public Text privateMarkerText;
    public Color privateMarkerColor;
    public GameObject privateMarkerPrefab;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        } else {
        }

    }

    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

        Look();
        Move();
        Jump();
        UseClockItem();
        PlacePublicMarker();
        PickUpPublicMarker();
        PlacePrivateMarker();
        PickUpPrivateMarker();
        
        // respawn if player falls off
        if (transform.position.y < -10)
        {
            if (PV.Owner.IsMasterClient)
            {
                transform.position = new Vector3(5, 0, 0);
            }
            else
            {
                transform.position = new Vector3(25, 0, 0);

            }
        }
    }


    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    void UseClockItem()
    {
        if (Input.GetKeyDown(KeyCode.L) &&  numClockItems > 0) 
        {
            numClockItems -= 1;
            GameObject go = FindClosestTile();
            Tile tile = go.GetComponent<Tile>();
            tile.meltTimer += 5f;
            //Debug.Log(go.transform.position);
        }
    }

    void PlacePublicMarker()
    {
        if (Input.GetKeyDown(KeyCode.K) && numPublicMarkers > 0)
        {
            numPublicMarkers -= 1;

            GameObject go = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PublicMarkerPrefab"), transform.position - new Vector3(0, 0.8f, 0), Quaternion.identity, 0);
            go.GetComponent<MeshRenderer>().material.SetColor("_Color", publicMarkerColor);

        }
    }

    void PlacePrivateMarker()
    {
        if (Input.GetKeyDown(KeyCode.I) && numPrivateMarkers > 0)
        {
            numPrivateMarkers -= 1;
            // GameObject go = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PublicMarkerPrefab"), transform.position - new Vector3(0, 0.8f, 0), Quaternion.identity, 0);
            // go.GetComponent<MeshRenderer>().material.SetColor("_Color",publicMarkerColor);   
            GameObject go = Instantiate(privateMarkerPrefab, transform.position - new Vector3(0, 0.7f, 0), Quaternion.identity);
            go.GetComponent<MeshRenderer>().material.SetColor("_Color", privateMarkerColor);
        }
    }

    void PickUpPublicMarker()   
    {
        if (Input.GetKeyDown(KeyCode.J) && numPublicMarkers < 5)
        {
            GameObject go = FindClosestPublicMarker();
            if (go != null)
            {
                PhotonNetwork.Destroy(go);
                numPublicMarkers += 1;
            }
        }
    }

    void PickUpPrivateMarker()
    {
        if (Input.GetKeyDown(KeyCode.U) && numPrivateMarkers < 5)
        {
            GameObject go = FindclosestPrivateMarker();
            if (go != null)
            {
                Destroy(go);
                numPrivateMarkers += 1;
            }
        }
    }

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
        } else if(collider.CompareTag("ClockItem"))
        {
            numClockItems += 1;
            Destroy(collider.gameObject);
        }
    }


    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);
        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }
        if(rb != null) 
        {
            rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
        }
        clockItemText.text = "Clock Items: " + numClockItems;
        publicMarkerText.text = "Public Markers: " + numPublicMarkers;
        privateMarkerText.text = "Private Markers: " + numPrivateMarkers;
    }

    public GameObject FindClosestTile()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Tile");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }

        return closest;
    }

    public GameObject FindClosestPublicMarker()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("PublicMarker");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            PublicMarker pm = go.GetComponent<PublicMarker>();
            if(pm.PV.IsMine)
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
            
        }

        if (distance > 5f) {
            return null;
        }

        return closest;
    }

    public GameObject FindclosestPrivateMarker()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("PrivateMarker");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            PublicMarker pm = go.GetComponent<PublicMarker>();
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
            
        }

        if (distance > 5f) {
            return null;
        }

        return closest;
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(numClockItems);
            stream.SendNext(numPublicMarkers);
        }
        else if (stream.IsReading)
        {
            numClockItems = (int) stream.ReceiveNext();
            numPublicMarkers = (int) stream.ReceiveNext();
        }
    }
}

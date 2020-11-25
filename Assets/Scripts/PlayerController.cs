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
    int numPublicMakers = 5;
    public Text publicMarkertText;
    public Color color;
    

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
            // color = new Color(0, 0, 1f);
        } else {
            // color = new Color(1f, 0, 0);
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
        if (Input.GetKeyDown(KeyCode.K) && numPublicMakers > 0)
        {
            numPublicMakers -= 1;

            GameObject go = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PublicMarkerPrefab"), transform.position - new Vector3(0, 0.8f, 0), Quaternion.identity, 0);
            go.GetComponent<MeshRenderer>().material.SetColor("_Color",color);

        }
    }

    void PickUpPublicMarker()   
    {
        if (Input.GetKeyDown(KeyCode.J) && numPublicMakers < 5)
        {
            GameObject go = FindClosestPublicMarker();
            if (go != null)
            {
                PhotonNetwork.Destroy(go);
                numPublicMakers += 1;
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
        publicMarkertText.text = "Public Markers: " + numPublicMakers;
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

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(numClockItems);
            stream.SendNext(numPublicMakers);
        }
        else if (stream.IsReading)
        {
            numClockItems = (int) stream.ReceiveNext();
            numPublicMakers = (int) stream.ReceiveNext();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    Rigidbody rb;
    PhotonView PV;
    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;
    int numClockItems = 0;
    [SerializeField] TMP_Text clockItemText;
    int numPublicMarkers = 3;
    [SerializeField] TMP_Text publicMarkerText;
    public Color publicMarkerColor;
    int numPrivateMarkers = 5;
    public Color privateMarkerColor;
    public GameObject privateMarkerPrefab;
    bool tileCheckerShot;
    private int numTileCheckers = 5;
    public bool isFrozen = false;
    [SerializeField] TMP_Text tileCheckerText;
    public Material badTileMat;

    public Material markerMat;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }

        GameObject.FindGameObjectWithTag("Music").GetComponent<BGM>().PlayMusic();
        tileCheckerShot = false;
        shaderCoolTime = 15f;
        cam.GetComponent<Shader>().enabled = false;
    }

    float coolTime;
    float shaderCoolTime;

    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

        if (coolTime > 0)
        {
            coolTime -= Time.deltaTime;
            tileCheckerShot = true;
        }
        else
        {
            tileCheckerShot = false;
        }

        if (shaderCoolTime > 0)
        {
            cam.GetComponent<Shader>().enabled = false;
            shaderCoolTime -= Time.deltaTime;
        }
        else {
            cam.GetComponent<Shader>().enabled = true;
        }

        if (!isFrozen)
        {
            // Look();
            Move();
            Jump();
            Shoot();
            UseClockItem();
            HandleMarker();
            // PlacePublicMarker();
            // PickUpPublicMarker();
            // PlacePrivateMarker();
            // PickUpPrivateMarker();
        }
        
        
        // respawn if player falls off
        if (transform.position.y < -10)
        {
            shaderCoolTime = 15f;
            if (PV.Owner.IsMasterClient)
            {
                transform.position = new Vector3(15, 0, 0);
            }
            else
            {
                transform.position = new Vector3(15, 0, 0);
            }
        }
    }

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public Transform cam;

    void Move()
    {
        // float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // + cam.eulerAngles.y
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            transform.position += direction * walkSpeed * Time.deltaTime;
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    public GameObject tileCheckerPrefab;

    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (!tileCheckerShot && numTileCheckers > 0) {
                coolTime = 1f;
                numTileCheckers--;
                Vector3 projectileForce = transform.forward * 350f + new Vector3(0f, 200f, 0f);
                GameObject tileChecker = Instantiate(tileCheckerPrefab, transform.position + transform.forward * 0.5f, Quaternion.identity);
                tileChecker.GetComponent<Rigidbody>().AddForce(projectileForce);
            }
        }
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    void PlacePublicMarker()
    {
        if (Input.GetKeyDown(KeyCode.K) && numPublicMarkers > 0)
        {
            numPublicMarkers -= 1;

            GameObject go = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PublicMarkerPrefab"), transform.position - new Vector3(0, 0.1f, 0), Quaternion.identity, 0);
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
            GameObject go = Instantiate(privateMarkerPrefab, transform.position - new Vector3(0, 0.1f, 0), Quaternion.identity);
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

    public Camera camera;

    // left clicking a tile uses the clock item
    void UseClockItem()
    {
        if (Input.GetMouseButtonDown(0))
        { 
            // if left button pressed...
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
            // the object identified by hit.transform was clicked
            // do whatever you want
                if (hit.collider.CompareTag("Tile") && numClockItems > 0) 
                {
                    hit.collider.GetComponent<Tile>().OnClockItemUsed();
                    hit.collider.GetComponent<Tile>().meltTimer += 5f;
                    hit.collider.GetComponent<Tile>().timeExtended = true;
                    numClockItems -= 1;
                }
            }
        }
    }

    // right clicking a tile spawns a public marker
    // right clicking a marker picks it up
    // [PunRPC]
    void HandleMarker()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.collider.CompareTag("Tile"))
                {
                    //place the marker
                    if (!hit.collider.GetComponent<Tile>().marked && numPublicMarkers > 0)
                    {
                        numPublicMarkers -= 1;
                        hit.collider.GetComponent<Tile>().marked = true;
                        PhotonView photonView = hit.collider.GetComponent<PhotonView>();
                        photonView.RPC("changeMat", RpcTarget.All);
                        //hit.collider.GetComponent<Tile>().changeMat();
                    }
                    else {
                        numPublicMarkers += 1;
                        hit.collider.GetComponent<Tile>().marked = false;
                        PhotonView photonView = hit.collider.GetComponent<PhotonView>();
                        photonView.RPC("revertMat", RpcTarget.All);
                        //hit.collider.GetComponent<Tile>().revertMat();
                    }
                    /*Vector3 pos = hit.point + new Vector3(0, 0.3f, 0);
                    GameObject go = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PublicMarkerPrefab"), pos, Quaternion.identity, 0);
                    go.GetComponent<MeshRenderer>().material = markerMat;*/
                }
            }
        }
    }

    [PunRPC]
    public void DestroyOnNetwork(int pvID)
    {
        PhotonNetwork.Destroy(PhotonView.Find(pvID));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!PV.IsMine)
        {
            return;
        }
        Collider collider = collision.collider;
        if (collider.CompareTag("Tile"))
        {
            Tile tile = collider.gameObject.GetComponent<Tile>();
            if (!tile.isMelting)
            {
                tile.onStepped();
            }
        }
        else if (collider.CompareTag("ClockItem"))
        {
            numClockItems++;

            int pvID = collider.gameObject.GetComponent<PhotonView>().ViewID;
            PV.RPC("DestroyOnNetwork", RpcTarget.MasterClient, pvID);
            PhotonNetwork.Destroy(collider.gameObject);
        }
        else if (collider.CompareTag("TileChecker"))
        {
            numTileCheckers++;
            Destroy(collider.gameObject);
        }
        else if (collider.CompareTag("DistortionResetter"))
        {
            shaderCoolTime = 15f;
            collider.gameObject.GetComponent<DistortionResetter>().onHit();

            int pvID = collider.gameObject.GetComponent<PhotonView>().ViewID;
            PV.RPC("DestroyOnNetwork", RpcTarget.MasterClient, pvID);
            PhotonNetwork.Destroy(collider.gameObject);
        }
        else if (collider.CompareTag("FinishLine"))
        {
            // endGame();
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("gameOver", RpcTarget.All);
        }
    }

    [PunRPC]
    public void gameOver() {
        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.LoadLevel(2);
        }
    }

    [PunRPC]
    public void createGameOverUI()
    {
        GameObject ui = Instantiate(GameOverUIPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        GameOver gameOverUI = ui.GetComponent<GameOver>();
        gameOverUI.winnerText.text = "You lost";
        gameObject.transform.GetChild(3).gameObject.SetActive(false);
    }

    [PunRPC]
    private void setPCFreeze(bool freezeState)
    {
        isFrozen = freezeState;
    }

    public GameObject GameOverUIPrefab;

    private void endGame()
    {
        isFrozen = true;

        GameObject[] pcs;
        pcs = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject pc in pcs)
        {
            PlayerController player = pc.GetComponent<PlayerController>();
            player.isFrozen = true;
            player.PV.RPC("setPCFreeze", RpcTarget.All, true);

            if (!player.PV.IsMine)
            {
                player.PV.RPC("createGameOverUI", RpcTarget.Others);
            }
        }

        GameObject ui = Instantiate(GameOverUIPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        GameOver gameOverUI = ui.GetComponent<GameOver>();
        gameOverUI.winnerText.text = "You won";

        gameObject.transform.GetChild(3).gameObject.SetActive(false);
    }


    void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            clockItemText.text = "";
            publicMarkerText.text = "";
            tileCheckerText.text = "";
            return;
        }
        clockItemText.text = "Clock Items: " + numClockItems;
        publicMarkerText.text = "Public Markers: " + numPublicMarkers;
        tileCheckerText.text = "Tile Checkers: " + numTileCheckers;
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
            // stream.SendNext(numTileCheckers);
        }
        else if (stream.IsReading)
        {
            numClockItems = (int) stream.ReceiveNext();
            numPublicMarkers = (int) stream.ReceiveNext();
        }
    }
}

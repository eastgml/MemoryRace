using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Tile : MonoBehaviour, IPunObservable
{
    PhotonView PV;

    public MeshRenderer mesh;

    public bool isBad; // true if tile is a bad tile
    public bool isInstaDeath; // true if tile is insta death, false if regular
    public bool timeExtended; // true if clock item is used

    private float meltPeriod; // time it takes to melt once stepped on
    public float meltTimer; // timer that tracks how far tile is in melt period. Starts at = meltPeriod, counts down until 0
    public bool isMelting; // true if tile is currently melting
    private float regenTimer; // after melting, time before it regenerates
    public bool isRegenerating; // true if tile is currently waiting to reappear
    public bool marked;
    public float clockItemGlowPeriod; // time it glows for
    public float clockItemGlowTimer; // timer that tracks how far tile is in glow period. Starts at 0, counts up until glowPeriod
    public bool isGlowing; // tile is currently glowing to show clock item was used
    public bool blink; // true if tile is gone during the current blink
    public float blinkTimer;
    public float blinkPeriod; // how long in between tile blinks when a bad tile is stepped on


    public Material badTileMat; // just for testing purposes
    public Material originalMat; // just for testing purposes
    public Material hoverMat;
    public Material clockItemMat;
    
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (!PV.IsMine)
        {
            return;
        }

        object[] tileTypeData = PV.InstantiationData;
        if ((bool)tileTypeData[0])
        {
            isInstaDeath = true;
        }
        else
        {
            isInstaDeath = false;
        }

        // tiles have 0.4 chance to be bad tiles
        // float rand = Random.Range(0.0f, 10.0f);
        if (isBad)
        {
            // isBad = true;

            if ((bool)tileTypeData[0])
            {
                // if bad tile is an instant death tile, it melts in 0 seconds
                meltPeriod = 0.0f;
            }
            else
            {
                // otherwise bad tile takes anywhere from 0.2 to 1.0 seconds to melt
                meltPeriod = Random.Range(0.2f, 1.0f);
            }

            PV.RPC("setTileInfo", RpcTarget.All, true, meltPeriod, isInstaDeath);
        }
        else
        {
            // isBad = false;
            PV.RPC("setTileInfo", RpcTarget.All, false, 0.0f, isInstaDeath);
        }

        isGlowing = false;
        clockItemGlowPeriod = 0.5f;
        clockItemGlowTimer = clockItemGlowPeriod;
        //meltTimer = meltPeriod;
        regenTimer = 3.0f;
        isMelting = false;
        isRegenerating = false;
        timeExtended = false;
        marked = false;
        blink = true;
        blinkPeriod = 0.2f;
        blinkTimer = blinkPeriod;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGlowing && clockItemGlowTimer > 0.0f)
        {
            clockItemGlowTimer -= Time.deltaTime;
        }
        else if (isGlowing && clockItemGlowTimer <= 0.0f)
        {
            isGlowing = false;
            clockItemGlowTimer = clockItemGlowPeriod;
            PV.RPC("revertMat", RpcTarget.All);
        }

        if (isBad)
        {
            //         JUST FOR TESTING PURPOSES BAD TILES WILL SHOW UP RED
            //         comment the line below out if you don't want them to be distinguishable
            // mesh.GetComponent<MeshRenderer>().material = badTileMat;

            if (isMelting)
            {
                meltTimer -= Time.deltaTime;
                blinkTimer -= Time.deltaTime;
            }

            if (isRegenerating)
            {
                regenTimer -= Time.deltaTime;
            }

            if (isMelting && blinkTimer <= 0)
            {
                if (blink)
                {
                    PV.RPC("setTileActive", RpcTarget.All, false, true);
                    blink = false;
                }
                else
                {
                    PV.RPC("setTileActive", RpcTarget.All, true, true);
                    blink = true;
                }
                
                blinkTimer = blinkPeriod;
            }

            // tile has melted, so it falls through
            if (isMelting && meltTimer <= 0)
            {
                isMelting = false;
                meltTimer = meltPeriod;
                PV.RPC("setTileActive", RpcTarget.All, false, false);
                blinkTimer = blinkPeriod;
                blink = true;
                isRegenerating = true;
            }

            // tile has finishsed waiting to regenerate, so it reappears
            if (isRegenerating && regenTimer <= 0)
            {
                isRegenerating = false;
                regenTimer = 3.0f;
                PV.RPC("setTileActive", RpcTarget.All, true, true);

            }
        }
        else
        {
            //       JUST FOR TESTING PURPOSES BAD TILES WILL SHOW UP RED
            //       comment the line below out if you don't want them to be distinguishable
            //mesh.GetComponent<MeshRenderer>().material = originalMat;
        }
    }

    // execute when a player steps on it
    public void onStepped()
    {
        if (isBad)
        {
            isMelting = true;
            regenTimer = 3.0f;
        }
    }

    [PunRPC]
    public void changeMat() {
        mesh.GetComponent<MeshRenderer>().material = hoverMat;
    }

    [PunRPC]
    public void revertMat()
    {
        mesh.GetComponent<MeshRenderer>().material = originalMat;
    }

    public void OnMouseOver()
    {
        if (!isGlowing)
        {
            mesh.GetComponent<MeshRenderer>().material = hoverMat;
        }    
    }

    public void OnMouseExit()
    {
        if (!marked && !isGlowing) {
            mesh.GetComponent<MeshRenderer>().material = originalMat;
        }
    }

    public AudioClip clockSound;

    [PunRPC]
    public void OnClockItemUsed()
    {
        AudioSource.PlayClipAtPoint(clockSound, gameObject.transform.position, 2f);
        isGlowing = true;
        mesh.GetComponent<MeshRenderer>().material = clockItemMat;
    }


    [PunRPC]
    private void setTileInfo(bool isBadTile, float meltTime, bool isInsta)
    {
        isBad = isBadTile;
        meltPeriod = meltTime;
        isInstaDeath = isInsta;
        meltTimer = meltPeriod;
    }

    [PunRPC]
    private void setTileActive(bool meshActive, bool colliderActive)
    {
        mesh.enabled = meshActive;
        gameObject.GetComponent<BoxCollider>().enabled = colliderActive;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(gameObject.activeSelf);
        }
        else
        {
            //gameObject.SetActive((bool)stream.ReceiveNext());
        }
    }
}

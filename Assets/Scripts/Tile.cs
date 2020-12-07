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

    private float meltPeriod; // time it takes to melt once stepped on
    public float meltTimer; // timer that tracks how far tile is in melt period
    public bool isMelting; // true if tile is currently melting
    private float regenTimer; // after melting, time before it regenerates
    public bool isRegenerating; // true if tile is currently waiting to reappear

    public Material badTileMat; // just for testing purposes
    public Material originalMat; // just for testing purposes
    public Material hoverMat;

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

        //meltTimer = meltPeriod;
        regenTimer = 3.0f;
        isMelting = false;
        isRegenerating = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBad)
        {
            //         JUST FOR TESTING PURPOSES BAD TILES WILL SHOW UP RED
            //         comment the line below out if you don't want them to be distinguishable
            // mesh.GetComponent<MeshRenderer>().material = badTileMat;

            if (isMelting)
            {
                meltTimer -= Time.deltaTime;
            }

            if (isRegenerating)
            {
                regenTimer -= Time.deltaTime;
            }

            // tile has melted, so it falls through
            if (isMelting && meltTimer <= 0)
            {
                isMelting = false;
                meltTimer = meltPeriod;
                PV.RPC("setTileActive", RpcTarget.All, false);
                isRegenerating = true;
            }

            // tile has finishsed waiting to regenerate, so it reappears
            if (isRegenerating && regenTimer <= 0)
            {
                isRegenerating = false;
                regenTimer = 3.0f;
                PV.RPC("setTileActive", RpcTarget.All, true);

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


    public void OnMouseOver()
    {
        mesh.GetComponent<MeshRenderer>().material = hoverMat;
    }

    public void OnMouseExit()
    {
        mesh.GetComponent<MeshRenderer>().material = originalMat;
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
    private void setTileActive(bool active)
    {
        mesh.enabled = active;
        gameObject.GetComponent<BoxCollider>().enabled = active;
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

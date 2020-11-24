using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Tile : MonoBehaviour, IPunObservable
{
    PhotonView PV;

    public MeshRenderer mesh;

    public bool isBad; // true if tile is a bad tile

    private float meltPeriod; // time it takes to melt once stepped on
    private float meltTimer; // timer that tracks how far tile is in melt period
    private bool isMelting; // true if tile is currently melting
    private float regenTimer; // after melting, time before it regenerates
    private bool isRegenerating; // true if tile is currently waiting to reappear

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        object[] tileTypeData = PV.InstantiationData;

        // decide if it's bad in the first place
        float rand = Random.Range(0.0f, 10.0f);
        if (rand < 2.0f)
        {
            isBad = true;
        }
        else
        {
            isBad = false;
        }

        if ((bool) tileTypeData[0])
        {
            // if tile is an instant death tile, it melts in 0 seconds
            meltPeriod = 0.0f;
        }
        else
        {
            // tile takes anywhere from 0.5 to 3.0 seconds to melt
            meltPeriod = Random.Range(0.5f, 3.0f);
        }

        meltTimer = meltPeriod;
        regenTimer = 3.0f;
        isMelting = false;
        isRegenerating = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBad)
        {
            if (isMelting)
            {
                meltTimer -= Time.deltaTime;
            }

            if (isRegenerating)
            {
                regenTimer -= Time.deltaTime;
            }

            // tile has melted, so it falls through
            if (meltTimer <= 0)
            {
                isMelting = false;
                meltTimer = meltPeriod;
                PV.RPC("setTileActive", RpcTarget.All, false);
                isRegenerating = true;
            }

            // tile has finishsed waiting to regenerate, so it reappears
            if (regenTimer <= 0)
            {
                isRegenerating = false;
                regenTimer = 3.0f;
                PV.RPC("setTileActive", RpcTarget.All, true);

            }
        }
    }

    // execute when a player steps on it
    public void onStepped()
    {
        if (isBad)
        {
            isMelting = true;
        }
    }

    [PunRPC]
    private void setTileActive(bool active)
    {
        mesh.enabled = active;
        gameObject.GetComponent<BoxCollider>().enabled = active;
        gameObject.GetComponent<BoxCollider>().isTrigger = active;
        //gameObject.SetActive(active);
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

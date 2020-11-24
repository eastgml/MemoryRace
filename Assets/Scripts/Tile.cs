using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Tile : MonoBehaviour, IPunObservable
{
    PhotonView PV;

    private float meltTimer; // time it takes to melt once stepped on
    private bool isMelting; // true if tile is currently melting
    private float regenTimer; // after melting, time before it regenerates
    private bool isRegenerating; // true if tile is currently waiting to reappear

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();

        meltTimer = 3.0f;
        regenTimer = 1.0f;
        isMelting = false;
        isRegenerating = false;
    }

    // Update is called once per frame
    void Update()
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
            meltTimer = 3.0f;
            //gameObject.SetActive(false);
            PV.RPC("setTileActive", RpcTarget.All, false);
        }

        // tile has finishsed waiting to regenerate, so it reappears
        if (regenTimer <= 0)
        {
            isRegenerating = false;
            regenTimer = 1.0f;
            //gameObject.SetActive(true);
            PV.RPC("setTileActive", RpcTarget.All, true);

        }
    }

    // execute when a player steps on it
    public void onStepped()
    {
        isMelting = true;
    }

    [PunRPC]
    private void setTileActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //Debug.Log("tile is writing");
            //stream.SendNext(gameObject.activeSelf);
        }
        else
        {
            //Debug.Log("tile is reading");
            //gameObject.SetActive((bool)stream.ReceiveNext());
        }
    }
}

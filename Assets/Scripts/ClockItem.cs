using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ClockItem : MonoBehaviour, IPunObservable
{
    PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (!PV.IsMine)
        {
            return;
        }

    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().numClockItems += 1;
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                int pvID = gameObject.GetComponent<PhotonView>().ViewID;
                PhotonView photonView = PhotonView.Get(this);
                photonView.RPC("DestroyOnNetwork", RpcTarget.MasterClient, pvID);
            }
        }
    }

    [PunRPC]
    public void DestroyOnNetwork(int pvID)
    {
        PhotonNetwork.Destroy(PhotonView.Find(pvID));
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

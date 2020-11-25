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

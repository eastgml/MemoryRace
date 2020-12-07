using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameEnd : MonoBehaviourPunCallbacks
{
    void Start() {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void QuitGame() {
        Application.Quit();
    }


    public void Restart() {
        this.photonView.RPC("LoadGameLevel", RpcTarget.All);
    }

    [PunRPC]
    public void LoadGameLevel() {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameEnd : MonoBehaviourPunCallbacks
{
    public void QuitGame() {
        Application.Quit();
    }

    public void Restart() {
        PhotonNetwork.LoadLevel(1);
    }
}

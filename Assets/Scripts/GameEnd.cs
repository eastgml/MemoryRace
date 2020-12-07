using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameEnd : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ReturnMainMenu() {
        PhotonNetwork.LoadLevel(0);
    }

    public void RestartGame() {
        PhotonNetwork.LoadLevel(1);
    }

    public void QuitGame() {
        Application.Quit();
    }
}

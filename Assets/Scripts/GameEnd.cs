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

<<<<<<< HEAD
<<<<<<< Updated upstream
    public void ReturnMainMenu() {
        PhotonNetwork.LoadLevel(0);
    }

    public void RestartGame() {
        PhotonNetwork.LoadLevel(1);
    }

    public void QuitGame() {
        Application.Quit();
=======
    // Update is called once per frame
    void Update()
    {

    }

    void RestartGame() {
        PhotonNetwork.LoadLevel(1);
>>>>>>> Stashed changes
=======
    // Update is called once per frame
    void Update()
    {
        
>>>>>>> parent of e4e1cde... tpm fixed
    }

    void QuitGame() {
        Application.Quit();
    }

}

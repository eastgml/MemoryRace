using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    void Awake() {
        PV = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine) {
            CreateController();
        }
    }

    void CreateController() {
        // spawn player one here
        if (PV.Owner.IsMasterClient)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), new Vector3(5, 0, 0), Quaternion.identity);
        }
        // spawn player two here
        else
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), new Vector3(25, 0, 0), Quaternion.identity);

        }
    }
}

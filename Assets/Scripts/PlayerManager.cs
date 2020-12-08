using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    public Material mat1;
    public Material mat2;

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
            GameObject go = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), new Vector3(13, 0, 0), Quaternion.identity);
            go.GetComponent<PlayerController>().publicMarkerColor = new Color(0.9f, 0.1f, 0);
            go.GetComponent<PlayerController>().privateMarkerColor = new Color(0.9f, 0.3f, 0.5f);
            go.GetComponent<PlayerController>().markerMat = mat1;
        }
        // spawn player two here
        else
        {
            GameObject go = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), new Vector3(17, 0, 0), Quaternion.identity);
            go.GetComponent<PlayerController>().publicMarkerColor = new Color(0, 0.2f, 0.9f);
            go.GetComponent<PlayerController>().privateMarkerColor = new Color(0, 0.5f, 0.9f);
            go.GetComponent<PlayerController>().markerMat = mat2;
        }
    }
}

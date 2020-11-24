using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;


public class ObjectManager : MonoBehaviour
{
    PhotonView PV;
    private int numTileRows = 5;
    private int tilesPerRow = 10;
    
    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (PV.Owner.IsMasterClient)
        {
            spawnTiles();
        }
    }

    void spawnTiles()
    {
        for (int i = 1; i < numTileRows + 1; i++)
        {
            for (int j = 0; j < tilesPerRow; j++)
            {
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "TilePrefab"), new Vector3(j * 3, 0, i * 5), Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

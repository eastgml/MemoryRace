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
        if (PV.IsMine && PV.Owner.IsMasterClient)
        {
            spawnTiles();
        }
    }

    void spawnTiles()
    {
        // ZONE: regular tiles
        object[] defaultTileData = new object[1]; // lets the tiles know they are regular tiles
        defaultTileData[0] = false;
        

        for (int i = 0; i < numTileRows; i++)
        {
            for (int j = 0; j < tilesPerRow; j++)
            {
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "TilePrefab"), new Vector3(j * 4, 0, i * 5 + 8), Quaternion.identity, 0, defaultTileData);
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "ClockItemPrefab"), new Vector3(j * 4, 1, i * 5 + 8), Quaternion.identity, 0);
            }
        }

        // ZONE: instant-death tiles
        object[] deathTileData = new object[1]; // lets the tiles know they are instant death tiles
        deathTileData[0] = true;

        for (int i = numTileRows; i < numTileRows * 2; i++)
        {
            for (int j = 0; j < tilesPerRow; j++)
            {
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "InstaDeathTilePrefab"), new Vector3(j * 4, 0, i * 5 + 8), Quaternion.identity, 0, deathTileData);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

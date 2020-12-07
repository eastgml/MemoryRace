using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;


public class ObjectManager : MonoBehaviour
{
    PhotonView PV;
    private int numTileRows = 4;
    private int tilesPerRow = 8;

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
                GameObject go = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "TilePrefab"), new Vector3(j * 5, 0, i * 5 + 8), Quaternion.identity, 0, defaultTileData);
                if (Random.Range(0, 1f) < 0.4)
                {
                    go.GetComponent<Tile>().isBad = true;
                }
                else
                {
                    go.GetComponent<Tile>().isBad = false;
                }

                // Randomly generate clock items
                if (Random.Range(0, 1f) < 0.2)
                {
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "ClockItemPrefab"), new Vector3(j * 5, 1, i * 5 + 8), Quaternion.Euler(0, 180f, 0), 0);
                }
            }
        }

        // ZONE: instant-death tiles
        object[] deathTileData = new object[1]; // lets the tiles know they are instant death tiles
        deathTileData[0] = true;

        for (int i = numTileRows; i < numTileRows * 2; i++)
        {
            int maxBadTileCnt = 6;
            for (int j = 0; j < tilesPerRow; j++)
            {
                GameObject go = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "InstaDeathTilePrefab"), new Vector3(j * 5, 0, i * 5 + 8), Quaternion.identity, 0, deathTileData);
                if (Random.Range(0, 1f) < 0.4 && maxBadTileCnt > 0)
                {
                    go.GetComponent<Tile>().isBad = true;
                    maxBadTileCnt -= 1;
                }
                else
                {
                    go.GetComponent<Tile>().isBad = false;
                }

                if (Random.Range(0, 1f) < 0.2)
                {
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "ClockItemPrefab"), new Vector3(j * 5, 1, i * 5 + 8), Quaternion.Euler(0, 180f, 0), 0);
                }
            }
        }

        // ZONE: regular tiles
        for (int i = numTileRows * 2; i < numTileRows * 3; i++)
        {
            for (int j = 0; j < tilesPerRow; j++)
            {
                GameObject go = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "TilePrefab"), new Vector3(j * 5, 0, i * 5 + 8), Quaternion.identity, 0, defaultTileData);
                if (Random.Range(0, 1f) < 0.4)
                {
                    go.GetComponent<Tile>().isBad = true;
                }
                else
                {
                    go.GetComponent<Tile>().isBad = false;
                }

                if (Random.Range(0, 1f) < 0.2)
                {
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "ClockItemPrefab"), new Vector3(j * 5, 1, i * 5 + 8), Quaternion.Euler(0, 180f, 0), 0);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
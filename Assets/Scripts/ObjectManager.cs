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
            int maxClockItems = 2;
            int maxDistortionResetters = 2;
            for (int j = 0; j < tilesPerRow; j++)
            {
                // randomize how tile is rotated
                float xRot, yRot, zRot;
                if (Random.Range(0, 1f) < 0.5f)
                {
                    xRot = 0f;
                }
                else
                {
                    xRot = 180f;
                }
                if (Random.Range(0, 1f) < 0.5f)
                {
                    yRot = 0f;
                }
                else
                {
                    yRot = 180f;
                }
                if (Random.Range(0, 1f) < 0.5f)
                {
                    zRot = 0f;
                }
                else
                {
                    zRot = 180f;
                }

                GameObject go = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "TilePrefab1"), new Vector3(j * 5, 0, i * 5 + 8), Quaternion.Euler(xRot, yRot, zRot), 0, defaultTileData);

                // randomize whether tile is bad or not
                if (Random.Range(0, 1f) < 0.4)
                {
                    go.GetComponent<Tile>().isBad = true;
                }
                else
                {
                    go.GetComponent<Tile>().isBad = false;
                }

                // Randomly generate clock items
                float rand = Random.Range(0, 1f);
                if (rand < 0.3 && maxClockItems > 0)
                {
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "ClockItemPrefab"), new Vector3(j * 5, 1, i * 5 + 8), Quaternion.Euler(0, 180f, 0), 0);
                    maxClockItems -= 1;
                }
                // Randomly generate distortion resetters
                else if (rand < 0.6 && maxDistortionResetters > 0)
                {
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "GlassesPrefab"), new Vector3(j * 5, 1, i * 5 + 8), Quaternion.Euler(0, -45f, 0), 0);
                    maxDistortionResetters -= 1;
                }
            }
        }

        // ZONE: instant-death tiles
        object[] deathTileData = new object[1]; // lets the tiles know they are instant death tiles
        deathTileData[0] = true;

        for (int i = numTileRows; i < numTileRows * 2; i++)
        {
            int maxBadTileCnt = 6;
            int maxClockItems = 2;
            int maxDistortionResetters = 2;

            for (int j = 0; j < tilesPerRow; j++)
            {
                // randomize how tile is rotated
                float xRot, yRot, zRot;
                if (Random.Range(0, 1f) < 0.5f)
                {
                    xRot = 0f;
                }
                else
                {
                    xRot = 180f;
                }
                if (Random.Range(0, 1f) < 0.5f)
                {
                    yRot = 0f;
                }
                else
                {
                    yRot = 180f;
                }
                if (Random.Range(0, 1f) < 0.5f)
                {
                    zRot = 0f;
                }
                else
                {
                    zRot = 180f;
                }

                GameObject go = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "InstaDeathTilePrefab1"), new Vector3(j * 5, 0, i * 5 + 8), Quaternion.Euler(xRot, yRot, zRot), 0, deathTileData);

                // randomize whether tile is bad or not
                if (Random.Range(0, 1f) < 0.4 && maxBadTileCnt > 0)
                {
                    go.GetComponent<Tile>().isBad = true;
                    maxBadTileCnt -= 1;
                }
                else
                {
                    go.GetComponent<Tile>().isBad = false;
                }

                // generate clock items
                float rand = Random.Range(0, 1f);
                if (rand < 0.2 && maxClockItems > 0)
                {
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "ClockItemPrefab"), new Vector3(j * 5, 1, i * 5 + 8), Quaternion.Euler(0, 180f, 0), 0);
                    maxClockItems -= 1;
                }
                // generate distortion resetters
                else if (rand < 0.4 && maxDistortionResetters > 0)
                {
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "GlassesPrefab"), new Vector3(j * 5, 1, i * 5 + 8), Quaternion.Euler(0, -45f, 0), 0);
                    maxDistortionResetters -= 1;
                }
            }
        }

        // ZONE: regular tiles
        for (int i = numTileRows * 2; i < numTileRows * 3; i++)
        {
            int maxClockItems = 2;
            int maxDistortionResetters = 2;

            for (int j = 0; j < tilesPerRow; j++)
            {
                // randomize how tile is rotated
                float xRot, yRot, zRot;
                if (Random.Range(0, 1f) < 0.5f)
                {
                    xRot = 0f;
                }
                else
                {
                    xRot = 180f;
                }
                if (Random.Range(0, 1f) < 0.5f)
                {
                    yRot = 0f;
                }
                else
                {
                    yRot = 180f;
                }
                if (Random.Range(0, 1f) < 0.5f)
                {
                    zRot = 0f;
                }
                else
                {
                    zRot = 180f;
                }

                GameObject go = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "TilePrefab1"), new Vector3(j * 5, 0, i * 5 + 8), Quaternion.Euler(xRot, yRot, zRot), 0, defaultTileData);

                // randomize whether tile is bad or not
                if (Random.Range(0, 1f) < 0.4)
                {
                    go.GetComponent<Tile>().isBad = true;
                }
                else
                {
                    go.GetComponent<Tile>().isBad = false;
                }

                // generate clock items 
                float rand = Random.Range(0, 1f);
                if (rand < 0.3 && maxClockItems > 0)
                {
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "ClockItemPrefab"), new Vector3(j * 5, 1, i * 5 + 8), Quaternion.Euler(0, 180f, 0), 0);
                    maxClockItems -= 1;
                }
                // generate distortion resetters
                else if (rand < 0.6 && maxDistortionResetters > 0)
                {
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "GlassesPrefab"), new Vector3(j * 5, 1, i * 5 + 8), Quaternion.Euler(0, -45f, 0), 0);
                    maxDistortionResetters -= 1;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
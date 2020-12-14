using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Tracker : MonoBehaviour
{
    public Material p1Mat;
    public Material p2Mat;
    public MeshRenderer mesh;

    // Start is called before the first frame update
    void Start()
    {
        PhotonView pv = gameObject.transform.parent.gameObject.GetComponent<PlayerController>().PV;
        if (pv.Owner.IsMasterClient)
        {
            mesh.GetComponent<MeshRenderer>().material = p1Mat;
        }
        else
        {
            mesh.GetComponent<MeshRenderer>().material = p2Mat;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

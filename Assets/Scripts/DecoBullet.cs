using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoBullet : MonoBehaviour
{
    public bool towardStart;

    Vector3 startPos;
    Vector3 endPos;

    public float speed;

    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float totalDistance;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;    
    }
    
    public void setValues()
    {
        startPos = gameObject.transform.position;

        if (towardStart)
        {
            endPos = gameObject.transform.position;
            endPos.z -= 73.7f; 
        }
        else
        {
            endPos = gameObject.transform.position;
            endPos.z += 73.7f;
        }

        totalDistance = Vector3.Distance(startPos, endPos);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == endPos)
        {
            Destroy(gameObject);
        }

        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - startTime) * speed;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / totalDistance;

        // Set our position as a fraction of the distance between the markers.
        transform.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoBulletManager : MonoBehaviour
{
    public GameObject bullet;
    float spawnPeriod = 1.0f;
    float timer = 0f;
    int maxBulletsPerGroup = 9;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnPeriod)
        {
            timer = 0f;

            // spawn school of decoration bullets every second
            float xPos = Random.Range(-2.5f, 37.0f);
            float yPos = Random.Range(-4.0f, -0.3f);
            float zPos = 0f;

            if (Random.Range(0, 1f) > 0.5)
            {
                // heading toward start line
                zPos = 76.5f;
                float speed = Random.Range(2.5f, 5f);

                /* GameObject bulletObj = Instantiate(bullet, new Vector3(xPos, yPos, zPos), Quaternion.Euler(90f, 0, 0));
                 bulletObj.GetComponent<DecoBullet>().towardStart = true;
                 bulletObj.GetComponent<DecoBullet>().speed = speed;
                 bulletObj.GetComponent<DecoBullet>().setValues();*/

                for (int i = 0; i < maxBulletsPerGroup; i++)
                {
                    GameObject bulletObj = Instantiate(bullet, new Vector3(xPos, yPos, zPos), Quaternion.Euler(90f, 0, 0));
                    bulletObj.GetComponent<DecoBullet>().towardStart = true;
                    bulletObj.GetComponent<DecoBullet>().speed = speed;
                    bulletObj.GetComponent<DecoBullet>().setValues();

                    zPos += Random.Range(-2f, 2f);
                    xPos += Random.Range(-1.2f, 1.2f);
                    yPos += Random.Range(-1f, 1f);
                    yPos = Mathf.Min(yPos, -0.3f);
                    speed += Random.Range(0f, 0.3f);

                    if (Random.Range(0, 1f) < 0.2f)
                    {
                        break;
                    }
                }
            }
            else
            {
                // heading away from start line
                zPos = 2.8f;
                float speed = Random.Range(2.5f, 5f);

                /*GameObject bulletObj = Instantiate(bullet, new Vector3(xPos, yPos, zPos), Quaternion.Euler(90f, 0, 0));
                bulletObj.GetComponent<DecoBullet>().towardStart = false;
                bulletObj.GetComponent<DecoBullet>().speed = speed;
                bulletObj.GetComponent<DecoBullet>().setValues();*/

                for (int i = 0; i < maxBulletsPerGroup; i++)
                {
                    GameObject bulletObj = Instantiate(bullet, new Vector3(xPos, yPos, zPos), Quaternion.Euler(90f, 0, 0));
                    bulletObj.GetComponent<DecoBullet>().towardStart = false;
                    bulletObj.GetComponent<DecoBullet>().speed = speed;
                    bulletObj.GetComponent<DecoBullet>().setValues();

                    zPos += Random.Range(-2f, 2f);
                    xPos += Random.Range(-1.2f, 1.2f);
                    yPos += Random.Range(-1f, 1f);
                    yPos = Mathf.Min(yPos, -0.3f);
                    speed += Random.Range(0f, 0.3f);

                    if (Random.Range(0, 1f) < 0.2f)
                    {
                        break;
                    }
                }
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickExplode : MonoBehaviour
{
    public bool superBall = false;

    public GameObject bulletPrefab;
    public GameObject powerUp;

    public Material mat;

    


    Color superColor = Color.yellow;

    // Start is called before the first frame update
    void Start()
    {
        if (superBall) { 
            gameObject.GetComponent<Renderer>().material = mat;
            gameObject.GetComponent<Renderer>().material.color = superColor;
        }
    }

    public void explode()
    {
        if (UnityEngine.Random.value <= 0.25f && !superBall)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, 90));

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDirection(transform.forward);
            }
        }

        if (superBall) {
            GameObject super = Instantiate(powerUp, transform.position, Quaternion.identity);

            PowerUp powerUpScript = super.GetComponent<PowerUp>();
            if (powerUpScript != null)
            {
                powerUpScript.SetDirection(transform.forward);
            }
        }





        Destroy(gameObject);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BrickExplode : MonoBehaviour
{
    public bool superBall = false;

    public GameObject bulletPrefab;
    public GameObject powerUp;

    public Material mat;

    public int layerNum = 0;

    List<Color> layerColors = new List<Color>
{
    new Color(1f, 0.992f, 0.588f),
    new Color(1f, 0.788f, 0.067f), 
    new Color(1f, 0.467f, 0.09f),
    new Color(1, 0.306f, 0.094f)
};


    Color superColor = Color.green;

    // Start is called before the first frame update


    private void OnEnable()
    {
        GM.instance.BrickList.Add(this);
    }


    void Start()
    {
        gameObject.GetComponent<Renderer>().material = mat;

        Color brickColor = layerColors[layerNum];


        if (superBall)
        {

            gameObject.GetComponent<Renderer>().material.color = superColor;
            gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", superColor);
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.color = brickColor;
            gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", brickColor);
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

        GM.instance.BrickList.Remove(this);
        GM.instance.didWeWin();


        Destroy(gameObject);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}

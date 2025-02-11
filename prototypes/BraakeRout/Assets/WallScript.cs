using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour
{
    private Material wallMaterial;
    private Color originalColor;
    List<Bullet> bullets = new List<Bullet>();


    void Start()
    {
        wallMaterial = GetComponent<Renderer>().material;
        originalColor = wallMaterial.color;
    }

    void Update()
    {
        BulletGone();

    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            wallMaterial.color = new Color(1f, 0.6f, 0.6f, originalColor.a);
            bullets.Add(other.gameObject.GetComponent<Bullet>());
        }
    }

    private void BulletGone()
    {
        bullets.RemoveAll(bullet => bullet == null);

        if (bullets.Count == 0)
        {
            wallMaterial.color = originalColor;
        }
    }



}

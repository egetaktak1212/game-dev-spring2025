using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    float speed = 3f;
    Vector3 direction;
    Rigidbody rb;
    bool wall = false;
    public Coroutine waitCoroutine;
    GameObject sphere;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = direction * speed;

        sphere = GameObject.Find("Sphere");
        UnityEngine.Debug.Log(sphere.GetComponentCount());

    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = direction * speed;
    }

    public void SetDirection(Vector3 inputdir) { 
        direction = inputdir;
    }

    private void OnTriggerEnter(Collider other)
    {
        UnityEngine.Debug.Log("trigger entered");
        if (other.CompareTag("Respawn")) {
            UnityEngine.Debug.Log("a");
            GM.instance.bigMode();
            
            Destroy(gameObject);
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Finish"))
        {
            waitCoroutine = StartCoroutine(waitthree(collision));
        }
    }

    private IEnumerator waitthree(Collision collision)
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }


}

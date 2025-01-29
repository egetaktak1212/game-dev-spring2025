using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 2f;
    public float acceleration = 0.6f; 
    public Vector3 direction;
    bool wall = false;
    private Coroutine waitCoroutine;
    public Transform camera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        direction = new Vector3(-1, 0.5f, 0.1f).normalized; 
        rb.velocity = direction * speed; 
    }

    void Update()
    {
        
        if (!wall)
        {
            speed += acceleration * Time.deltaTime;
            rb.velocity = direction * speed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Finish"))
        {
            direction = Vector3.Reflect(direction, collision.contacts[0].normal).normalized;
            if (collision.collider.CompareTag("Block")) {
                Vector3 normaldir = collision.contacts[0].normal;
                Destroy(collision.gameObject);
                direction = Vector3.Reflect(direction, normaldir).normalized;
            }


        }
        else if (!wall) { 
            waitCoroutine = StartCoroutine(waitthree(collision));
        }
    }

    private IEnumerator waitthree(Collision collision)
    {
        wall = true;
        Vector3 oldvelocity = rb.velocity;
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(1f);
        Debug.Log("DEAD");
        Time.timeScale = 0f;
        //direction = Vector3.Reflect(direction, collision.contacts[0].normal).normalized;
        //rb.velocity = direction * speed;
        //wall = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Respawn")) { 
            //exit numerator
            StopAllCoroutines();
            direction = new Vector3(Camera.main.transform.forward.x, Camera.main.transform.forward.y, Camera.main.transform.forward.z).normalized;
            rb.velocity = direction * speed;
            wall = false;
        }
    }


}

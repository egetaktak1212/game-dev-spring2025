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

    private Renderer ballRender;

    Color safeColor = Color.green;
    Color deadColor = Color.red;






    void Start()
    {
        ballRender = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        direction = new Vector3(-1, 0.5f, 0.1f).normalized; 
        rb.velocity = direction * speed; 
        ballRender.material.color = safeColor;
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
                Destroy(collision.gameObject);
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

        float duration = 1f;
        float timeElapsed = 0f;

        while (timeElapsed < duration) {
            float t = timeElapsed / duration;
            Color current = Color.Lerp(safeColor, deadColor, t);

            ballRender.material.color = current;

            timeElapsed += Time.deltaTime;
            yield return null;

        }
        Debug.Log("DEAD");
        Time.timeScale = 0f;

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Respawn")) { 
            //exit numerator
            StopAllCoroutines();
            direction = new Vector3(Camera.main.transform.forward.x, Camera.main.transform.forward.y, Camera.main.transform.forward.z).normalized;
            rb.velocity = direction * speed;
            wall = false;
            ballRender.material.color = safeColor;
        }
    }


}

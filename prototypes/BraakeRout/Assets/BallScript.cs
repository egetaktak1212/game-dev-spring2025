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
    Coroutine superCoroutine;
    public Transform camera;

    private Renderer ballRender;

    Color safeColor = Color.green;
    Color deadColor = Color.red;

    public ItemHandler itemHandler;
    bool magnetActiveLast = false;
    float initialSpeed;
    float maxSpeed = 8f;
    bool superBall = false;
    Vector3 originalSize;


    private void OnEnable()
    {
        GM.goBigMode += enterSuperBall;
    }

    private void OnDisable()
    {
        GM.goBigMode -= enterSuperBall;
    }

    void Start()
    {
        originalSize = transform.localScale;
        ballRender = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        direction = new Vector3(-1, 0.5f, 0.1f).normalized; 
        rb.velocity = direction * speed; 
        ballRender.material.color = safeColor;        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            enterSuperBall();
        }




        if (itemHandler.actionState["Magnet"])
        {
            if (!magnetActiveLast)
            {
                initialSpeed = speed;
                magnetActiveLast = true;
            }

            Vector3 magnetDir = Camera.main.transform.position - transform.position;
            magnetDir.Normalize();

            float pullStrength = 5f;
            speed = Mathf.MoveTowards(speed, maxSpeed, 2f * Time.deltaTime);

            rb.velocity = Vector3.Lerp(rb.velocity, magnetDir * speed, pullStrength * Time.deltaTime);
        }
        else
        {
            if (magnetActiveLast) 
            {
                speed = initialSpeed; 
                magnetActiveLast = false;  
            }

            if (!wall)
            {
                if (speed < 5f)
                {
                    speed += acceleration * Time.deltaTime;
                }
                rb.velocity = direction * speed;
            }
        }
    }

  

    public void enterSuperBall()
    {
        if (!superBall)
        {
            superBall = true;
            transform.localScale = originalSize * 3f;
            if (superCoroutine != null)
            {
                StopCoroutine(superCoroutine);
            }
            superCoroutine = StartCoroutine(SuperBallDuration());
        }
        else {
            //StopCoroutine(superCoroutine);
            //StartCoroutine(SuperBallDuration());
        
        }
    }

    public void leaveSuperBall()
    {
        Debug.Log("A");
        superBall = false;
        transform.localScale = originalSize; 
    }

    private IEnumerator SuperBallDuration()
    {
        Debug.Log("B");
        yield return new WaitForSeconds(3f);
        Debug.Log("c");
        leaveSuperBall(); 
    }




    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Finish"))
        {
            if (!collision.collider.CompareTag("Block"))
            {
                direction = Vector3.Reflect(direction, collision.contacts[0].normal).normalized;
            }
            else {
                if (!superBall)
                {
                    direction = Vector3.Reflect(direction, collision.contacts[0].normal).normalized;
                }
                BrickExplode brickCode = collision.gameObject.GetComponent<BrickExplode>();
                brickCode.explode();
                //Destroy(collision.gameObject);
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
            StopCoroutine(waitCoroutine);
            direction = new Vector3(Camera.main.transform.forward.x, Camera.main.transform.forward.y, Camera.main.transform.forward.z).normalized;
            rb.velocity = direction * speed;
            wall = false;
            ballRender.material.color = safeColor;
        }
    }


}

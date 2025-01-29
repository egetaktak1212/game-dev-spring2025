using System.Collections;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;

    public float minYReflection = 0.4f;

    public float initialSpeed = 200;

    public float distanceFromCenterInfluenceOnReflection = 10f;

    float acc = 1f;

    public bool superBall = false;
    Vector3 originalScale;

    public PhysicMaterial brickBounce;


    Vector3 previousVelocity;
    Vector3 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.velocity = new Vector3(0, -initialSpeed, 0);
        startPosition = transform.position;
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        initialSpeed += acc * Time.deltaTime;

        if (transform.position.y < -8) {
            transform.position = startPosition;
            rb.velocity = new Vector3(0, -initialSpeed, 0);
        }
    }

    void FixedUpdate()
    {
        previousVelocity = rb.velocity;
    }

    void OnCollisionEnter(Collision collision) 
    {
        if (collision.gameObject.CompareTag("brick") || collision.gameObject.CompareTag("supabrick")) {
            bool isSuperBrick = collision.gameObject.CompareTag("supabrick");
            Destroy(collision.gameObject);
            if (isSuperBrick) {
                StartCoroutine(supaBall());
            }
            
            //if we are supa ball, dont bounce off. just destroy
            if (!superBall) { 
                rb.velocity = new Vector3(previousVelocity.x, previousVelocity.y * -1, previousVelocity.z); 
            }

        } else if (collision.gameObject.CompareTag("paddle")) {
            ContactPoint c = collision.GetContact(0);
            Vector3 paddlePos = collision.gameObject.transform.position;
            Vector3 paddleScale = collision.gameObject.transform.localScale;
            float percent = Map(c.point.x, paddlePos.x - paddleScale.x / 2, paddlePos.x + paddleScale.x / 2, -1, 1);
            Vector3 newVelocity = new Vector3(previousVelocity.x, previousVelocity.y * -1, previousVelocity.z);
            newVelocity.x += distanceFromCenterInfluenceOnReflection * percent;
            newVelocity.y = Mathf.Max(newVelocity.y, minYReflection);
            newVelocity = newVelocity.normalized * previousVelocity.magnitude;
            rb.velocity = newVelocity;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("brick") || other.CompareTag("supabrick")) {
            Destroy(other.gameObject);
        }
    }


    private IEnumerator supaBall()
    {
        superBall = true;
        transform.localScale *= 3f;
        brickBounce.bounciness = 0;

        yield return new WaitForSeconds(8f);
        transform.localScale = originalScale;
        superBall = false;
        brickBounce.bounciness = 1;
    }
    

    public float Map(float valueOld, float oldMin, float oldMax, float newMin, float newMax)
    {
        float oldRange = oldMax - oldMin;
        float newRange = newMax - newMin;
        float valueOldPercent = (valueOld - oldMin) / oldRange;
        return newRange * valueOldPercent + newMin;
    }
}

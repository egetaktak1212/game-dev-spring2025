using UnityEngine;

public class PaddleScript : MonoBehaviour
{
    public Rigidbody rb;
    float maxSpeed = 7;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxLinearVelocity = maxSpeed;
    }

    private void Update()
    {
        float hAxis = Input.GetAxis("Horizontal");
        Vector3 velocity = rb.velocity;
        velocity.x = hAxis * maxSpeed;
        rb.velocity = velocity;

    }



    //void FixedUpdate()
    //{
    //    float hAxis = Input.GetAxis("Horizontal");
    //    rb.AddForce(Vector3.right * hAxis * 1500 * Time.deltaTime);

    //    if (Input.anyKey)
    //    {
    //        rb.linearDamping = 0;
    //    }
    //    else
    //    {
    //        rb.linearDamping = maxSpeed/6;
    //    }
    //}
}

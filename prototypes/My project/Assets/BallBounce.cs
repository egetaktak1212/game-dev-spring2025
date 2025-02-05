using UnityEngine;

public class BallBounce : MonoBehaviour
{

    public GameObject empty1;
    public GameObject empty2;


    Transform locationOne;
    Transform locationTwo;

    public Rigidbody rb;

    float pullMult = 1f;
    public float speed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distance = locationTwo.position - transform.position;
        bool ballIsCloseToPlayer = distance.magnitude < 1f;

        if (!ballIsCloseToPlayer)
        {
            regularPull();
        }
        else {
            transform.position += distance * 0.1f * Time.deltaTime;
        }


    }

    public void setTarget(Transform target) {
        locationTwo = target.transform;
    }
    public void setOrigin(Transform origin)
    {
        locationOne = origin.transform;
    }

    void regularPull() {
        Mathf.Clamp(pullMult, 0, 1);

        Vector3 pull = new Vector3(0, 1, 0);
        Vector3 velocity = locationTwo.position - transform.position;
        velocity = velocity.normalized;


        transform.position += (velocity + pull * pullMult) * speed * Time.deltaTime;

        pullMult -= 0.004f;
        if (pullMult < 0)
        {
            pullMult = 0;
        }
    }

}

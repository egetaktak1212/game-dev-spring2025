using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickScript : MonoBehaviour
{
    public GameObject ball;
    Collider bounceCollide; 
    Collider triggerCollide; 
    private BallScript ballScript; 

    // Start is called before the first frame update
    void Start()
    {

        ballScript = ball.GetComponent<BallScript>();
        bounceCollide = GetComponent<Collider>();
        triggerCollide = GetComponents<Collider>()[1];

    }

    // Update is called once per frame
    void Update()
    {
        if (ballScript.superBall)
        {
            triggerCollide.enabled = true;
            bounceCollide.enabled = false;
        }
        else { 
            triggerCollide.enabled=false;
            bounceCollide.enabled = true;
        }
    }
}

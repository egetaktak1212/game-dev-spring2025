using System.Collections;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public CharacterController cc;
    public GameObject camEmpty;
    Coroutine jumpWaiter;


    bool follow = true;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 followPoint = camEmpty.transform.position;
        //if the player is lower than where they jumped from, we should start following again
        bool isCamlower = followPoint.y < transform.position.y;
        if (!follow && !isCamlower) { 
            followPoint.y = transform.position.y;
        }
        transform.position = followPoint;

    }

    public void jumped() {

        jumpWaiter = StartCoroutine(jumpWait());
    
    }

    public void landed() {
        if (jumpWaiter != null)
        {
            StopCoroutine(jumpWaiter);
        }
        follow = true;
    }

    public void jumpedInAir() { 
        transform.position = camEmpty.transform.position;
    }


    IEnumerator jumpWait() {

        follow = false;

        yield return new WaitForSeconds(3f);

        follow = true;
    
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboarding : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LateUpdate()
    {
         transform.LookAt(Camera.main.transform.position, Vector3.up);   
    }

}

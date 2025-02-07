using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{
    public static GM instance;

    public static Action goBigMode;



    // Start is called before the first frame update
    void Start()
    {
        if (GM.instance == null)
        {
            GM.instance = this;
        }
        else
        {
            Destroy(this);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void bigMode() { 
        goBigMode.Invoke();
    }

}

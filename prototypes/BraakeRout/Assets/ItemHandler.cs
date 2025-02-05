using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    public Collider hitBox;

    Dictionary<string, bool> actionState = new Dictionary<string, bool>
    {
    {"Racket", true},
    {"Gun", false}, 
    {"Magnet", false}
    };


    // Start is called before the first frame update
    void Start()
    {
        hitBox.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //enables racket hitbox when clicking
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            hitBox.enabled = true;
        }
    }
}

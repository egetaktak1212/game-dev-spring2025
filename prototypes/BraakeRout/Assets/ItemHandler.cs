using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    public Collider hitBox;

    public Dictionary<string, bool> actionState = new Dictionary<string, bool>
    {
    {"Racket", true},
    {"Gun", false}, 
    {"Magnet", false}
    };

    void SwitchTo(string tool)
    {
        foreach (var key in actionState.Keys.ToList())
        {
            if (key == tool)
            {
                actionState[key] = true;
            }
            else
            {
                actionState[key] = false;
            }
        }
    }

    void HandleInputs()
    {
        if (Input.GetMouseButton(1))
        {
            switchToGun();
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            switchToMagnet();
        }
        else
        {
            switchToRacket();
        }
    }

    void switchToGun() {
        SwitchTo("Gun");
        hitBox.enabled = false;
        //code here for what needs to happen
    }

    void switchToRacket() {
        SwitchTo("Racket");
        hitBox.enabled = true;
    }

    void switchToMagnet() {
        SwitchTo("Magnet");
        hitBox.enabled = false;
    
    }




    void Start()
    {
        hitBox.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInputs();


        foreach (var kvp in actionState)
        {
            if (kvp.Value)
            {
                Debug.Log(kvp.Key);
            }
        }

        if (actionState["Gun"] && Input.GetMouseButtonDown(0)) {
            ShootRay();
        }

    }

    void ShootRay()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;


        ray.origin -= ray.direction * 0.5f;
 
        LayerMask bulletLayer = ~LayerMask.GetMask("Finish");

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 0.1f);

        if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f, bulletLayer))
        {
            Debug.Log($"Ray hit: {hit.collider.name} | Tag: {hit.collider.tag} | Position: {hit.collider.transform.position} | Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            if (hit.collider.CompareTag("Bullet"))
            {
                hit.collider.gameObject.GetComponent<Bullet>().StopAllCoroutines();
                Destroy(hit.collider.gameObject);
            }
        }
    }



}

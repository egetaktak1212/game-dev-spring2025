using UnityEngine;
using UnityEngine.InputSystem;

public class LeverScript : MonoBehaviour
{
    public GameObject UI;
    public Animator targetAnimator;
    public Collider triggerCollider;
    InputAction Interact;

    bool nearLever = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Interact = InputSystem.actions.FindAction("Interact");
    }

    // Update is called once per frame
    void Update()
    {
        if (nearLever)
        {
            if (Interact.WasPressedThisFrame()) { 
                nearLever = false;
                triggerCollider.enabled = false;
                targetAnimator.SetTrigger("StartSpin");
                UI.SetActive(false);

            }

        
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) { 
            nearLever = true;
            UI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) { 
            nearLever = false;
            UI.SetActive(false);
        }
    }




}

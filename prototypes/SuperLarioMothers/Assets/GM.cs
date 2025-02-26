using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour
{
    InputAction restart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        restart = InputSystem.actions.FindAction("Restart");
    }

    // Update is called once per frame
    void Update()
    {
        if (restart.WasPressedThisFrame()) { 
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
    }
}

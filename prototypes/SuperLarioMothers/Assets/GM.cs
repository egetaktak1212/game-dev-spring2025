using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour
{
    InputAction restart;

    public GameObject originalCheckpoint;
    public PlayerControls player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.setCheckpoint(originalCheckpoint.transform.position);

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

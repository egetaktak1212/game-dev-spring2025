using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetCamera : MonoBehaviour
{

    CinemachineOrbitalFollow freeLookCamera;

    InputAction resetCam;

    public GameObject player;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resetCam = InputSystem.actions.FindAction("ResetCamera");
        freeLookCamera = GetComponent<CinemachineOrbitalFollow>();
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Vector3.SignedAngle(Vector3.forward, player.transform.forward, Vector3.up);
        if (resetCam.WasPressedThisFrame())
        {
            freeLookCamera.HorizontalAxis.Value = angle;
        }
    }
}

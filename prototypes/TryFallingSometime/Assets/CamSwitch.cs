using Unity.Cinemachine;
using UnityEngine;

public class CamSwitch : MonoBehaviour
{
    public CinemachineCamera switchFrom;
    public CinemachineCamera switchTo;
    public GameObject player;
    CinemachineOrbitalFollow switchFromCoord;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (switchFrom.CompareTag("MainCinemachine"))
        {
            switchFromCoord = switchFrom.gameObject.GetComponent<CinemachineOrbitalFollow>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("x angle: " + Vector3.SignedAngle(Vector3.forward, switchTo.gameObject.transform.forward, Vector3.up) + " y angle: " +
        Vector3.SignedAngle(Vector3.forward, switchTo.gameObject.transform.forward, Vector3.forward));
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("AAAA");
        if (other.CompareTag("Player")) {
            switchTo.Priority = 1;
            switchFrom.Priority = 0;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (switchFrom.CompareTag("MainCinemachine"))
            {
                setSwitchFromRotation();
            }
            switchTo.Priority = 0;
            switchFrom.Priority = 1;
        }
    }

    void setSwitchFromRotation() { 
        float xangle = Vector3.SignedAngle(Vector3.forward, switchTo.gameObject.transform.forward, Vector3.up);
        float yangle = Vector3.SignedAngle(Vector3.forward, switchTo.gameObject.transform.forward, Vector3.forward);

        switchFromCoord.HorizontalAxis.Value = xangle;
        switchFromCoord.VerticalAxis.Value = yangle;

  
    }


}

using UnityEngine;
using UnityEngine.UIElements;

public class Checkpoint : MonoBehaviour
{
    public Collider triggerCollider;
    public GameObject point;

    PlayerControls player;
    Material material;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerControls>();
        material = gameObject.GetComponent<Material>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            player.setCheckpoint(point.transform.position);
            triggerCollider.enabled = false;
        }
    }
}

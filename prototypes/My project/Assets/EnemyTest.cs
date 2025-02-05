using UnityEngine;

public class EnemyTest : MonoBehaviour
{

    public GameObject projectilePrefab;
    public GameObject player;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            BallBounce ballScript = projectile.GetComponent<BallBounce>();
            ballScript.setOrigin(transform);
            ballScript.setTarget(player.transform);
        }    
    }




}

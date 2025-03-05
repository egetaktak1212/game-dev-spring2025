using UnityEngine;

public class RandomAnimationPlatform : MonoBehaviour
{

    Animator anim;
    float randomOffset;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        randomOffset = Random.Range(0f, 1f);

        anim.Play("MovingWipeout", 0, 0.5f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

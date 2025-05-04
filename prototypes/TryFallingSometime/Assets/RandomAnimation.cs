using UnityEngine;

public class RandomAnimation : MonoBehaviour
{

    Animator anim;
    float randomOffset;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        randomOffset = Random.Range(0f, 1f);

        anim.Play("WipeOutPush", 0, randomOffset);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using UnityEngine;

public class SideToSideOffset : MonoBehaviour
{

    Animator anim;
    public float offset;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.Play("SidetoSide", 0, offset);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
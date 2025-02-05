using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    float speed = 3f;
    Vector3 direction;
    Rigidbody rb;
    Renderer bulletRender;
    Color safeColor = Color.white;
    Color deadColor = Color.black;
    bool wall = false;
    public Coroutine waitCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        bulletRender = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        rb.velocity = direction * speed;
        bulletRender.material.color = safeColor;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = direction * speed;
    }

    public void SetDirection(Vector3 inputdir) { 
        direction = inputdir;
    }
    private void OnCollisionEnter(Collision collision)
    {
    if (!wall && collision.transform.CompareTag("Finish"))
        {
            waitCoroutine = StartCoroutine(waitthree(collision));
        }
    }
    private IEnumerator waitthree(Collision collision)
    {
        wall = true;
        Vector3 oldvelocity = rb.velocity;
        rb.velocity = Vector3.zero;

        float duration = 5f;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            Color current = Color.Lerp(safeColor, deadColor, t);

            bulletRender.material.color = current;

            timeElapsed += Time.deltaTime;
            yield return null;

        }
        Debug.Log("DEAD");
        Time.timeScale = 0f;

    }



}

using UnityEngine;
using System.Collections;

public class FallThrough : MonoBehaviour
{
    private Renderer cubeRenderer;
    private Collider cubeCollider;
    private Color solidColor;
    private Color transparentColor;

    public float solidTime = 2f;
    public float flashDuration = 1f;
    public float flashInterval = 0.2f;
    public float invisibleTime = 2f;

    public float offset = 0f;


    void Start()
    {
        cubeRenderer = GetComponent<Renderer>();
        cubeCollider = GetComponent<Collider>();

        solidColor = cubeRenderer.material.color;
        transparentColor = new Color(solidColor.r, solidColor.g, solidColor.b, 0.2f);

        StartCoroutine(FallThroughCycle());
    }

    IEnumerator FallThroughCycle()
    {
        yield return new WaitForSeconds(offset);

        while (true)
        {
            cubeRenderer.material.color = solidColor;
            cubeCollider.enabled = true;
            yield return new WaitForSeconds(solidTime);

            float elapsedTime = 0f;
            while (elapsedTime < flashDuration)
            {
                cubeRenderer.material.color = transparentColor;
                yield return new WaitForSeconds(flashInterval);
                cubeRenderer.material.color = solidColor;
                yield return new WaitForSeconds(flashInterval);
                elapsedTime += flashInterval * 2;
            }

            cubeRenderer.material.color = transparentColor;
            cubeCollider.enabled = false;
            yield return new WaitForSeconds(invisibleTime);
        }
    }
}

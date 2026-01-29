using UnityEngine;

public class BackgroundLayer : MonoBehaviour
{
    [Header("Settings")]
    [Range(0f, 1f)] public float parallaxFactor;

    private Transform mainCamera;
    private Vector3 startPos;
    private float length;

    private void Start()
    {
        mainCamera = Camera.main.transform;
        startPos = transform.position;

        SpriteRenderer render = GetComponent<SpriteRenderer>();
        if (render != null)
        {
            length = render.bounds.size.x;
        }
    }

    private void FixedUpdate()
    {
        float dist = mainCamera.position.x * parallaxFactor;
        float temp = mainCamera.position.x * (1 - parallaxFactor);

        transform.position = new Vector3(startPos.x + dist, mainCamera.position.y, transform.position.z);

        if (temp > startPos.x + length) startPos.x += length;
        else if (temp < startPos.x - length) startPos.x -= length;
    }
}
using System;
using UnityEngine;

public class BackgroundLayer : MonoBehaviour
{
    [Header("Settings")]
    [Range(0f, 1f)] public float parallaxFactor;
    public GameObject background;

    private Transform mainCamera;
    private Vector3 startPos;
    private float length;

    private void Awake()
    {
        // Get important variables
        mainCamera = Camera.main.transform;
        startPos = transform.position;

        // Initialise background segments
        for (int i = -15; i <= 15; i++)
        {
            // Place the flush to each other
            GameObject obj = Instantiate(background, transform);
            length = obj.GetComponent<SpriteRenderer>().bounds.size.x;
            obj.transform.position = new Vector2(i * length, mainCamera.position.y);
        }

        // Store the length of each sprite
        SpriteRenderer render = GetComponentInChildren<SpriteRenderer>();
        if (render != null)
        {
            length = render.bounds.size.x;
        }
    }

    private void LateUpdate()
    {
        // Work out what changes need to occur
        float dist = mainCamera.position.x * parallaxFactor;
        float temp = mainCamera.position.x * (1 - parallaxFactor);

        // Move by the distance factor
        transform.position = new Vector3(startPos.x + dist, mainCamera.position.y, transform.position.z);

        // Check if the background needs to be moved
        if (temp > startPos.x + length) startPos.x += length;
        else if (temp < startPos.x - length) startPos.x -= length;
    }
}
using UnityEngine;

public class BackgroundLayer : MonoBehaviour
{
    [Header("Settings")]
    [Range(0f, 1f)] public float parallaxFactor;
    public GameObject background;

    private Transform mainCamera;
    private Vector3 startPos;
    private float length;

    private void Start()
    {
        mainCamera = Camera.main.transform;
        startPos = transform.position;

        for (int i = -15; i <= 15; i++)
        {
            GameObject obj = Instantiate(background, transform);
            length = obj.GetComponent<SpriteRenderer>().bounds.size.x;
            obj.transform.position = new Vector2(i * length, mainCamera.position.y);
        }

        SpriteRenderer render = GetComponentInChildren<SpriteRenderer>();
        if (render != null)
        {
            length = render.bounds.size.x;
        }
    }

    private void LateUpdate()
    {
        float dist = mainCamera.position.x * parallaxFactor;
        float temp = mainCamera.position.x * (1 - parallaxFactor);

        transform.position = new Vector3(startPos.x + dist, mainCamera.position.y, transform.position.z);

        if (temp > startPos.x + length) startPos.x += length;
        else if (temp < startPos.x - length) startPos.x -= length;
    }
}
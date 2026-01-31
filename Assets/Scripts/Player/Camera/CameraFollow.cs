using System.Xml.Serialization;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public Camera mainCamera;
    public WorldGenerator worldGenerator;

    [Header("Settings")]
    public float speed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    public void CalculateBounds()
    {
        float height = mainCamera.orthographicSize;
        float width = height * Screen.width / Screen.height;

        minX = width - worldGenerator.worldWidth / 2f;
        maxX = worldGenerator.worldWidth / 2f - width;

        minY = height - worldGenerator.worldHeight / 2f;
        maxY = worldGenerator.worldHeight / 2 - height;
    }

    private void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;

        float x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        float y = Mathf.Clamp(desiredPosition.y, minY, maxY);

        Vector3 smoothPos = Vector3.Lerp(transform.position, new Vector3(x, y, offset.z), speed * Time.deltaTime);
        transform.position = smoothPos;
    }
}
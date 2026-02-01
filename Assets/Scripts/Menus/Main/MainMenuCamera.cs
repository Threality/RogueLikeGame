using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float maxSpeed = 2f;
    public float acceleration = 0.5f;
    public float dirChangeProb = 0.004f;
    public float maxHeight = -100f;


    [Header("References")]
    public Rigidbody2D rb;

    // Data
    private Vector2 destination = new Vector2(0, 0);


    private void Awake()
    {
        rb.position = new Vector3(Random.Range(-400, 400), Random.Range(0, -200), 0);
        destination = new Vector2(Random.Range(-400, 400), Random.Range(0, -200));
    }


    void Update()
    {
        Vector2 direction = destination - (Vector2) transform.position;
        Vector2 normDirection = direction.normalized;
        
        rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, maxSpeed * normDirection.x, acceleration * Time.deltaTime);
        rb.linearVelocityY = Mathf.Lerp(rb.linearVelocityY, maxSpeed * normDirection.y, acceleration * Time.deltaTime);

        if (direction.magnitude < 0.5f || Random.value < dirChangeProb)
        {
            destination = new Vector2(Random.Range(-400, 400), Random.Range(0, -200));
        }
    }
}

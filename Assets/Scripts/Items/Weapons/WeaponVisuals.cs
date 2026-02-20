using UnityEngine;
using System.Collections;

public class WeaponVisuals : MonoBehaviour
{
    [Header("Settings")]
    public float rotationSpeed = 15f;
    public SpriteRenderer render;
    public Sprite sprite;

    [Header("Swing Settings")]
    public float swingAngle = 90f;
    public float swingOffset = 0f;
    public AnimationCurve swingCurve;

    private bool isSwinging = false;

    private void Start()
    {
        // Create a default animation curve to control swing
        swingCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.2f, 1), new Keyframe(1, 0));
    }

    private void Update()
    {
        if (!isSwinging) FaceMouse();
    }

    private void FaceMouse()
    {
        // Get the angle to move to
        Vector2 direction = GameManager.instance.GetMouseDirection();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + swingOffset;

        // Move towards the angle smoothly
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void Swing(float swingDuration)
    {
        if (isSwinging) return;
        StartCoroutine(PerformSwing(swingDuration));
    }

    private IEnumerator PerformSwing(float swingDuration)
    {
        // Mark swinging and show sword
        isSwinging = true;
        render.enabled = true;

        // Get initial angles
        Quaternion startRotation = transform.rotation;
        float startAngleOffset = swingAngle / 2f;

        float timer = 0f;
        while (timer < swingDuration)
        {
            // Get swing value for current time
            timer += Time.deltaTime;
            float progress = timer / swingDuration;
            float curveValue = swingCurve.Evaluate(progress);

            // Work out angle to adjust by
            float currentAngleAddition = Mathf.Lerp(startAngleOffset, -startAngleOffset, curveValue);

            // Modify current angle
            transform.rotation = startRotation * Quaternion.Euler(0, 0, currentAngleAddition);

            yield return null;
        }

        // Reset
        transform.rotation = startRotation;
        render.enabled = false;
        isSwinging = false;
    }
}
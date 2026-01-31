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
        swingCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.2f, 1), new Keyframe(1, 0));
    }

    private void Update()
    {
        if (!isSwinging) FaceMouse();
    }

    private void FaceMouse()
    {
        Vector2 direction = GameManager.instance.GetMouseDirection();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + swingOffset;

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
        isSwinging = true;
        render.enabled = true;

        Quaternion startRotation = transform.rotation;
        float startAngleOffset = swingAngle / 2f;

        float timer = 0f;
        while (timer < swingDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / swingDuration;

            float curveValue = swingCurve.Evaluate(progress);

            float currentAngleAddition = Mathf.Lerp(startAngleOffset, -startAngleOffset, curveValue);

            transform.rotation = startRotation * Quaternion.Euler(0, 0, currentAngleAddition);

            yield return null;
        }

        transform.rotation = startRotation;
        render.enabled = false;
        isSwinging = false;
    }
}
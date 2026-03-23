using UnityEngine;
using UnityEngine.Splines;

public class MovingObjectSpline : MonoBehaviour
{
    public SplineContainer splineContainer;

    [Header("Movement")]
    public float baseSpeed = 5f;
    public bool loop = true;
    public bool alignRotation = true;

    [Header("Options")]
    public bool playOnStart = true;

    [HideInInspector] public float normalizedPosition = 0f;
    private bool isMoving = false;

    float splineLength;
    float speedMultiplier = 1f;

    const float END_OFFSET = 0.1f; //POURQUOI TU FONCTIONNES PAS !!
    //Spline weird movement at the end of the spline when not looping, c'est chiant

    void Start()
    {
        if (splineContainer != null)
            splineLength = splineContainer.Spline.GetLength();

        if (playOnStart)
            isMoving = true;
    }

    void Update()
    {
        if (splineContainer == null || !isMoving) return;

        float delta = (baseSpeed / splineLength) * speedMultiplier * Time.deltaTime;
        normalizedPosition += delta;

        if (loop)
        {
            normalizedPosition = Mathf.Repeat(normalizedPosition, 1f);
        }
        else
        {
            normalizedPosition = Mathf.Clamp(normalizedPosition, 0f, 1f - END_OFFSET);
        }

        ApplyPosition();
    }

    public void ApplyPosition()
    {
        float t = normalizedPosition;

        Vector3 pos = splineContainer.EvaluatePosition(t);
        transform.position = pos;

        if (alignRotation)
        {
            Vector3 tangent = splineContainer.EvaluateTangent(t);

            if (tangent.sqrMagnitude > 0.00001f)
                transform.rotation = Quaternion.LookRotation(tangent);
        }
    }

    public void SetSpeedMultiplier(float value)
    {
        speedMultiplier = value;
    }

    public float GetNormalizedPosition()
    {
        return normalizedPosition;
    }

    public void SetNormalizedPosition(float value)
    {
        if (loop)
        {
            normalizedPosition = Mathf.Repeat(value, 1f);
        }
        else
        {
            normalizedPosition = Mathf.Clamp(value, 0f, 1f - END_OFFSET);
        }

        ApplyPosition();
    }

    public void SetMovementActive(bool active)
    {
        isMoving = active;
    }
}
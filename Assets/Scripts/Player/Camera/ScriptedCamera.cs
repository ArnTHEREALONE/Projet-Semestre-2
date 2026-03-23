using UnityEngine;

public class ScriptedCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 5f, -8f);

    [Header("Vitesse de rattrapage")]
    [Range(0.01f, 10f)]
    public float followSpeed = 5f;
    public bool lookAtTarget = true;

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset;

        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        transform.position = smoothedPosition;

        if (lookAtTarget)
        {
            transform.LookAt(target);
        }
    }
}
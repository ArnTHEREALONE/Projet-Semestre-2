using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;

    [Header("Rotation")]
    public float mouseSensitivity = 3f;
    public float controllerSensitivity = 200f;
    public float minYAngle = -60f;
    public float maxYAngle = 80f;

    [Header("Zoom")]
    public float distance = 5f;
    public float minDistance = 2f;
    public float maxDistance = 8f;
    public float zoomSpeed = 2f;

    [Header("Collision")]
    public float cameraRadius = 0.3f;
    public LayerMask collisionMask;
    public float collisionOffset = 0.1f;

    private float yaw;
    private float pitch;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    void LateUpdate()
    {
        if (!target)
        {
            TryFindPlayer();
            return;
        }

        Vector2 lookInput = Vector2.zero;

        lookInput.x = Input.GetAxis("Mouse X");
        lookInput.y = Input.GetAxis("Mouse Y");

        if (Gamepad.current != null)
        {
            Vector2 stick = Gamepad.current.rightStick.ReadValue();
            lookInput += stick * controllerSensitivity * Time.deltaTime;
        }

        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minYAngle, maxYAngle);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        float zoomInput = Input.GetAxis("Mouse ScrollWheel");

        // ajouter un moyen pour zoom/dezoom à la manette.

        distance -= zoomInput * zoomSpeed * Time.deltaTime * 10f;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Vector3 desiredCameraPos = target.position + rotation * new Vector3(0f, 0f, -distance);
        Vector3 direction = desiredCameraPos - target.position;
        float targetDistance = direction.magnitude;
        direction.Normalize();

        float finalDistance = targetDistance;

        if (Physics.SphereCast(target.position, cameraRadius, direction, out RaycastHit hit, targetDistance, collisionMask))
        {
            finalDistance = hit.distance - collisionOffset;
        }

        finalDistance = Mathf.Clamp(finalDistance, minDistance, distance);

        transform.position = target.position + direction * finalDistance;
        transform.rotation = rotation;
    }

    void TryFindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            target = player.transform;
            yaw = transform.eulerAngles.y;
            pitch = transform.eulerAngles.x;
        }
    }
}

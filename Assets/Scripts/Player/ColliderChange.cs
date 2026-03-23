using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(CharacterControler))]
public class CapsuleAirScaler : MonoBehaviour
{
    [Header("Air Settings")]
    public float airHeight = 1.0f;
    public Vector3 airCenter = new Vector3(0f, 0.5f, 0f);

    private CapsuleCollider capsule;
    private CharacterControler controller;

    private float defaultHeight;
    private Vector3 defaultCenter;

    private void Awake()
    {
        capsule = GetComponent<CapsuleCollider>();
        controller = GetComponent<CharacterControler>();

        // Sauvegarde des valeurs initiales
        defaultHeight = capsule.height;
        defaultCenter = capsule.center;
    }

    private void Update()
    {
        if (controller.isGrounded)
        {
            ResetCollider();
        }
        else
        {
            ApplyAirCollider();
        }
    }

    void ApplyAirCollider()
    {
        capsule.height = airHeight;
        capsule.center = airCenter;
    }

    void ResetCollider()
    {
        capsule.height = defaultHeight;
        capsule.center = defaultCenter;
    }
}
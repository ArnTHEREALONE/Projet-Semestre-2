using UnityEngine;

public class JadeAnimator : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private CharacterControler controler;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        controler = GetComponent<CharacterControler>();
    }

    void Update()
    {
        if (!controler.isDashing)
            animator.SetBool("IsDashing", false);

        UpdateLocomotion();
        UpdateAirState();
    }

    void UpdateLocomotion()
    {
        Vector3 horizontalVelocity = rb.linearVelocity;
        horizontalVelocity.y = 0f;

        float speed = horizontalVelocity.magnitude;

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsRunning", speed > controler.walkSpeed + 0.1f);
    }

    public void ResetJumpAnimation()
    {
        animator.Play("Jump", 0, 0f);
    }

    void UpdateAirState()
    {
        animator.SetBool("IsGrounded", controler.isGrounded);
    }

    public void PlayJump()
    {
        animator.SetTrigger("Jump");
    }

    public void PlayDash()
    {
        animator.SetBool("IsDashing", true);
        animator.SetTrigger("Dash");
    }
}

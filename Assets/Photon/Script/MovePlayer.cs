using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class MovePlayer : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private Vector3 velocity = Vector3.zero;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;

    [Networked, OnChangedRender(nameof(OnSpeedChanged))]
    private float AnimationSpeed { get; set; }

    [Networked, OnChangedRender(nameof(OnPlayerMove))]
    private bool IsMoving { get; set; }

    [Networked, OnChangedRender(nameof(OnPlayerJump))]
    private bool IsJumping { get; set; }

    [Networked, OnChangedRender(nameof(OnPlayerDie))]
    private bool IsDead { get; set; }

    [Networked, OnChangedRender(nameof(OnPlayerAttack))]
    public int IsAttacking { get; set; } = 0;

    void OnSpeedChanged()
    {
        animator.SetFloat("Speed", AnimationSpeed);
    }

    void OnPlayerMove()
    {
        animator.SetBool("IsMove", IsMoving);
    }

    void OnPlayerJump()
    {
        animator.SetBool("IsJumping", IsJumping);
    }

    void OnPlayerDie()
    {
        animator.SetBool("IsDead", IsDead);
    }

    void OnPlayerAttack()
    {
        animator.SetInteger("IsAttacking", IsAttacking);
    }

    public override void FixedUpdateNetwork()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var jump = Input.GetAxis("Jump");

        Vector3 movement = new Vector3(horizontal, 0, vertical);

        if (characterController.isGrounded)
        {
            if (IsJumping) IsJumping = false;
            velocity.y = 0f;

            if (jump > 0)
            {
                velocity.y = jumpForce;
                IsJumping = true;
            }
            //CheckGround();
        }
        else
        {
            velocity.y -= gravity * Runner.DeltaTime;
        }

        characterController.Move((movement * speed + velocity) * Runner.DeltaTime);
        AnimationSpeed = characterController.velocity.magnitude;
        IsMoving = vertical != 0 || horizontal != 0;

        if (movement != Vector3.zero)
        {
            RotatorPlayer(movement);
        }
    }

    private void CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.2f))
        {
            if (hit.collider.CompareTag("Plane"))
            {
                Debug.Log("kill");
            }
        }
    }

    private void RotatorPlayer(Vector3 direction)
    {
        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, speed * Runner.DeltaTime * 100);
    }
}

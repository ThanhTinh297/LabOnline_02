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

    private ChatUI chatUI;

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

    void Awake()
    {
        chatUI = FindObjectOfType<ChatUI>();
    }

    public override void FixedUpdateNetwork()
    {
        if (chatUI != null && chatUI.IsChatInputActive)
        {
            if (IsMoving || AnimationSpeed > 0.1f)
            {
                IsMoving = false;
                AnimationSpeed = 0f;
            }

            velocity.y -= gravity * Runner.DeltaTime;
            characterController.Move(velocity * Runner.DeltaTime);
            return;
        }

        if (!HasInputAuthority) return;
        if (characterController == null) return;

        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var jump = Input.GetAxis("Jump");

        Vector3 movementInput = new Vector3(horizontal, 0, vertical);
        Vector3 moveDirection = movementInput.normalized;

        if (characterController.isGrounded)
        {
            velocity.y = -0.1f;
            if (jump != 0)
            {
                velocity.y = jumpForce;
                IsJumping = true;
            }
        }
        else
        {
            velocity.y -= gravity * Runner.DeltaTime;
            IsJumping = false;
        }

        Vector3 finalMovement = moveDirection * speed + velocity;

        characterController.Move(finalMovement * Runner.DeltaTime);

        Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);
        AnimationSpeed = horizontalVelocity.magnitude;
        IsMoving = movementInput.sqrMagnitude > 0.01f;

        if (movementInput != Vector3.zero)
        {
            RotatePlayer(moveDirection);
        }
    }

    private void RotatePlayer(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        float rotationSpeed = 720f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation,
            rotationSpeed * Runner.DeltaTime);
    }
}

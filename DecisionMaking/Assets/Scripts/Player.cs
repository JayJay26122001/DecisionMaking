using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
public class Player : Character
{
    public Transform directionAnchor;
    [System.NonSerialized] public Vector2 directionInput;
    [SerializeField] protected float coyoteTime, jumpBuffering;
    public UnityEvent StartedPushing, StoppedPushing;
    public PushableObject pushing;
    bool jumpBuffer;
    //public PlayerData data;
    protected override void Awake()
    {
        base.Awake();
        Fell.AddListener(CoyoteTime);
        HitGround.AddListener(StopCoyoteTime);
        HitGround.AddListener(PeformJumpBuffering);
    }
    private void Update()
    {
        Move(directionAnchor.forward * directionInput.y + directionAnchor.right * directionInput.x, speed);
    }
    void CoyoteTime()
    {
        StartCoroutine(CoyoteTimer());
    }
    void StopCoyoteTime()
    {
        StopCoroutine(CoyoteTimer());
        canJumpOnAir = false;
    }
    void PeformJumpBuffering()
    {
        if (jumpBuffer)
        {
            Jump(true);
            jumpBuffer = false;
        }
    }
    public void MoveInput(InputAction.CallbackContext context)
    {
        if (!movePaused)
        {
            directionInput = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1);
        }
        else
        {
            directionInput = Vector2.zero;
        }
    }
    public void JumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if(!movePaused)
            {
                if (vState != VerticalState.grounded && canJumpOnAir == false)
                {
                    StartCoroutine(JumpBuffer());
                }
                Jump(true);
                if (canJumpOnAir == true)
                {
                    canJumpOnAir = false;
                }
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            Jump(false);
            jumpBuffer = false;
        }
    }
    IEnumerator CoyoteTimer()
    {
        canJumpOnAir = true;
        yield return new WaitForSeconds(coyoteTime);
        canJumpOnAir = false;
    }
    IEnumerator JumpBuffer()
    {
        jumpBuffer = true;
        yield return new WaitForSeconds(jumpBuffering);
        jumpBuffer = false;
    }
}

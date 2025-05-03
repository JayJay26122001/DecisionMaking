using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Character : FloatingRigidbody
{
    [SerializeField]protected float speed, jumpForce, jumpTime, jumpingSpeedMultiplier, jumpCooldown;
    protected bool jumpOnCooldown = false;
    protected bool canJumpOnAir = false;
    Timer jumpTimer = new Timer();
    [SerializeField] protected UnityEvent Jumped, FellOnJump;
    public enum CharVerticalState { falling, grounded, jumping };
    [SerializeField] CharVerticalState cvState; // N�O USE ESTA VARIAVEL use PvState ao inv�s
    public CharVerticalState CvState
    {
        get
        {
            switch (vState)
            {
                case VerticalState.falling: cvState = CharVerticalState.falling; break;
                case VerticalState.grounded: cvState = CharVerticalState.grounded; break;
                default: cvState = CharVerticalState.jumping; break;
            }
            return cvState;
        }
        set
        {
            cvState = value;
            switch (value)
            {
                case CharVerticalState.falling: vState = VerticalState.falling; break;
                case CharVerticalState.grounded: vState = VerticalState.grounded; break;
                default: vState = VerticalState.none; break;
            }
        }
    }
    protected override void Awake()
    {
        base.Awake();
        jumpTimer.timedEvent.AddListener(JumpForce);
    }
    
    protected override void FixedUpdate()
    {
        if (CvState != CharVerticalState.jumping)
        {
            Float();
        }
        else if (!jumpTimer.timer(jumpTime, Time.fixedDeltaTime, true, false))
        {
            stopJump();
        }
        RotateFoward();
        UpdateVelocity();
    }
    protected void Jump(bool pressed)
    {
        if (pressed && !movePaused)
        {
            if ((CvState == CharVerticalState.grounded|| canJumpOnAir) && !jumpOnCooldown)
            {
                jumpTimer.SetTimer(0);
                jumpTimer.Paused = false;
                CvState = CharVerticalState.jumping;
                Jumped.Invoke();
                StartCoroutine(JumpCooldown());
            }
        }
        else if (CvState == CharVerticalState.jumping)
        {
            jumpTimer.SetTimer(jumpTime);
        }
    }
    void stopJump()
    {
        CvState = CharVerticalState.falling;
        jumpTimer.SetTimer(0);
        FellOnJump.Invoke();
        jumpTimer.Paused = true;
    }
    void JumpForce()
    {
        //pular
        localVelocity.y = 0;
        localVelocity += Vector3.up * Mathf.Sqrt(jumpForce * -Physics.gravity.y * gravityMultiplier);
        //parar de pular se bater a cabe�a
        if (Physics.Raycast(rb.position, Vector3.up, (height / 2) * 1.1f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            Jump(false);
        }
    }
    IEnumerator JumpCooldown()
    {
        jumpOnCooldown = true;
        yield return new WaitForSeconds(jumpCooldown);
        jumpOnCooldown = false;
    }
    float rotationAux;
    protected void RotateFoward()
    {
        float angle = Vector3.SignedAngle(Vector3.forward, lastHorizontalDirection, Vector3.up);
        if (Mathf.Abs(Mathf.Abs(transform.eulerAngles.y) - angle) > 0.1f)
        {
            float SmoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref rotationAux, 0.2f/*tempo q leva pra terminar rotação*/);
            transform.rotation = Quaternion.Euler(0, SmoothAngle, 0);
        }
    }
    public void Move(Vector3 dir, float speed)
    {
        if (movePaused) dir = Vector3.zero;
        dir = Vector3.ClampMagnitude(dir, 1);
        if (CvState != CharVerticalState.grounded)
        {
            speed *= jumpingSpeedMultiplier;
        }
        SetVelocity(new Vector3(dir.x*speed, localVelocity.y, dir.z*speed));
    }

}

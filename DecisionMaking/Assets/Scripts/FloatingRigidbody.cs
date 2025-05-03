using UnityEngine;
using UnityEngine.Events;

public class FloatingRigidbody : MonoBehaviour
{
    [SerializeField] protected float floatingHeight, gravityMultiplier, maxVelocity, terrainBuffer;
    [System.NonSerialized] public Vector3 localVelocity, worldVelocity, parentVelocity, lastHorizontalDirection, hDir;
    public Vector3 HolrizontalDirection { get { return hDir; } }
    [System.NonSerialized] public Rigidbody rb;
    protected float height;
    public enum HorizontalState { moving, idle, none };
    public HorizontalState movingState;
    public enum VerticalState { falling, grounded, none };
    public VerticalState vState;
    [SerializeField] protected UnityEvent Fell, HitGround, StartedMoving, StoppedMoving;
    [SerializeField] QueryTriggerInteraction RayTriggerInteraction;
    [SerializeField] LayerMask RayMasks;
    [SerializeField] int raycastNumber;
    [SerializeField] float raycastRadius;
    public GameObject OnTopOf { get; private set; }
    [System.NonSerialized] public bool movePaused, gravityPaused;
    protected virtual void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        height = gameObject.GetComponent<Collider>().bounds.size.y;
        lastHorizontalDirection = transform.forward;
    }
    protected virtual void FixedUpdate()
    {
        Float();
        UpdateVelocity();
    }
    protected RaycastHit RaycastGround()
    {
        RaycastHit hitInfo;
        RaycastHit[] hits;
        if (raycastNumber <= 1)
        {
            Physics.Raycast(rb.position, Vector3.down, out hitInfo, floatingHeight + terrainBuffer + height / 2, RayMasks, RayTriggerInteraction);
        }
        else
        {
            hits = new RaycastHit[raycastNumber + 1];
            for (int i = 0; i < raycastNumber; i++)
            {
                Quaternion rot = Quaternion.Euler(0, (360 / raycastNumber) * i, 0);
                Vector3 raypoint = rb.position + Vector3.forward * raycastRadius;
                var v = raypoint - rb.position;
                v = rot * v;
                raypoint = rb.position + v;
                Physics.Raycast(raypoint, Vector3.down, out hits[i], floatingHeight + terrainBuffer + height / 2, RayMasks, RayTriggerInteraction);
                Debug.DrawRay(raypoint, Vector3.down * (floatingHeight + terrainBuffer + height / 2), Color.red, Time.fixedDeltaTime);
            }
            Physics.Raycast(rb.position, Vector3.down, out hits[raycastNumber], floatingHeight + terrainBuffer + height / 2, RayMasks, RayTriggerInteraction);
            int shorter = 0;
            for (int i = 1; i < hits.Length; i++)
            {
                if ((hits[i].collider != null && hits[i].distance < hits[shorter].distance) || hits[shorter].collider == null)
                {
                    shorter = i;
                }
            }
            hitInfo = hits[shorter];
        }
        if (OnTopOf != hitInfo.collider?.gameObject && vState == VerticalState.grounded)
        {
            OnTopOf?.GetComponent<IMovingGround>()?.FRig.Remove(this);
            OnTopOf = hitInfo.collider?.gameObject;
            OnTopOf?.GetComponent<IMovingGround>()?.FRig.Add(this);
        }
        else if (vState != VerticalState.grounded)
        {
            OnTopOf?.GetComponent<IMovingGround>()?.FRig.Remove(this);
            OnTopOf = null;
        }
        return hitInfo;
    }
    protected void Float()
    {
        RaycastHit hitInfo = RaycastGround();
        float groundDistance = hitInfo.distance - (floatingHeight + (height / 2));
        if (!gravityPaused)
        {
            if (hitInfo.collider == null || Mathf.Sign(groundDistance) > 0)
            {
                if (hitInfo.collider != null || (groundDistance <= terrainBuffer && vState == VerticalState.grounded))
                {
                    localVelocity.y = Mathf.Clamp(groundDistance, 0, 1) * Physics.gravity.y * gravityMultiplier;
                }
                else
                {
                    localVelocity += Vector3.up * Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
                }
            }
            else
            {
                localVelocity.y = Mathf.Clamp(groundDistance, -1, 0) * Physics.gravity.y * gravityMultiplier;
            }
        }
        else
        {
            localVelocity.y = 0;
        }
        if (hitInfo.collider != null)
        {
            if (vState == VerticalState.falling)
            {
                vState = VerticalState.grounded;
                HitGround.Invoke();
            }
            else
            {
                vState = VerticalState.grounded;
            }
        }
        else
        {
            if (vState == VerticalState.grounded)
            {
                vState = VerticalState.falling;
                Fell.Invoke();
            }
            else
            {
                vState = VerticalState.falling;
            }
        }
        
    }
    protected void SetVelocity(Vector3 vel)
    {
        if (movePaused) vel = new Vector3(0, vel.y, 0);
        hDir = new Vector3(vel.x, 0, vel.z).normalized;
        if (hDir != Vector3.zero)
        {
            lastHorizontalDirection = hDir;
            if(movingState != HorizontalState.moving)
            {
                movingState = HorizontalState.moving;
                StartedMoving.Invoke();
            }
        }
        else if(movingState != HorizontalState.idle)
        {
            movingState = HorizontalState.idle;
            StoppedMoving.Invoke();
        }
        localVelocity = vel;
    }
    protected void UpdateVelocity()
    {
        if (OnTopOf?.GetComponent<IMovingGround>() != null)
        {
            parentVelocity = OnTopOf.GetComponent<IMovingGround>().GetVelocity();
        }
        else
        {
            parentVelocity = Vector3.zero;
        }
        worldVelocity = parentVelocity + Vector3.ClampMagnitude(localVelocity, maxVelocity);
        rb.linearVelocity = worldVelocity;
    }
}

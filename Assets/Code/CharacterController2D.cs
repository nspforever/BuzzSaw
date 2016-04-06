using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
    private const float SkinWidth = .002f;
    private const int TotalHorizontalRays = 8;
    private const int TotalVericalRays = 4;
    private static readonly float SlopeLimitTangant = Mathf.Tan(75f * Mathf.Deg2Rad);

    public LayerMask PlatformMask;
    public ControllerParameter2D DefaultParameters;

    public ControllerState2D State { get; private set; }

    public Vector2 Velocity { get { return _velocity; } }

    private Vector2 _velocity;
    private Transform _transform;
    private Vector3 _localScale;
    private BoxCollider2D _boxCollider;

    private float _verticalDistanceBetweenRays;
    private float _horizontalDistanceBetweenRays;

    

    public bool CanJump { get { return false; } }
    public bool HandleCollisions { get; set; }
    public ControllerParameter2D Parameters { get { return _overrideParameters ?? DefaultParameters; } }

    public ControllerParameter2D _overrideParameters;


    public void Awake()
    {
        HandleCollisions = true;
        State = new ControllerState2D();
        _transform = transform;
        _localScale = transform.localScale;
        _boxCollider = GetComponent<BoxCollider2D>();

        var colliderWith = _boxCollider.size.x * Mathf.Abs(transform.localScale.x) - (2 * SkinWidth);
        _horizontalDistanceBetweenRays = colliderWith / (TotalVericalRays - 1);

        var colliderHeight = _boxCollider.size.y * Mathf.Abs(transform.localScale.y) - (2 * SkinWidth);
        _verticalDistanceBetweenRays = colliderHeight / (TotalHorizontalRays - 1);

        Debug.Log(string.Format("_verticalDistanceBetweenRays.x:{0}", _verticalDistanceBetweenRays));

    }

    public void AddForce(Vector2 force)
    {
        _velocity += force;
    }

    public void SetForce(Vector2 force)
    {
        _velocity = force;
    }

    public void SetHorizontalForce(float x)
    {
        //Debug.Log(string.Format("SetHorizontalForce:{0}", x));
        _velocity.x = x;
    }

    public void SetVerticalForce(float y)
    {
        _velocity.y = y;
    }

    public void Jump()
    {

    }

    public void LateUpdate()
    {   
        Move(Velocity * Time.deltaTime);
    }

    private void Move(Vector2 deltaMovement)
    {
        var wasGrounded = State.IsCollidingBlow;
        State.Reset();
        //Debug.Log(string.Format("deltaMovement.x: {0}", deltaMovement.x));
        if(HandleCollisions)
        {   
            HandlePlatforms();
            CalculateRayOrigins();

            if (deltaMovement.y < 0 && wasGrounded)
            {
                HandleVerticalSlope(ref deltaMovement);
            }

            if (Mathf.Abs(deltaMovement.x) > .00001f)
            {
                MoveHorizontally(ref deltaMovement);
            }

            MoveVertically(ref deltaMovement);
        }

        // TODO: Additional moving platform code

        _transform.Translate(deltaMovement,  Space.World);
        //Debug.Log(string.Format("DeltaMovement.x:{0}", deltaMovement.x));

        _velocity.x = Mathf.Min(_velocity.x, Parameters.MaxVelocity.x);
        _velocity.y = Mathf.Min(_velocity.y, Parameters.MaxVelocity.y);

        if (State.IsMovingUpSlope)
        {
            _velocity.y = 0;
        }
    }

    private void HandlePlatforms()
    {

    }

    private Vector3 _raycastTopLeft, _raycastBottomRight, _raycastBottomLeft;

    private void CalculateRayOrigins()
    {
        /*var size = new Vector2(_boxCollider.size.x * Mathf.Abs(_localScale.x), _boxCollider.size.y * Mathf.Abs(_localScale.y)) / 2;
        var center = new Vector2(_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);

        _raycastTopLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y + size.y - SkinWidth);
        _raycastBottomRight = _transform.position + new Vector3(center.x + size.x - SkinWidth, center.y - size.y + SkinWidth);
        _raycastBottomLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y - size.y + SkinWidth);*/
        Bounds bounds = _boxCollider.bounds;

        bounds.Expand(SkinWidth * -2);
        _raycastTopLeft = new Vector3(bounds.min.x - SkinWidth, bounds.max.y + SkinWidth);
        _raycastBottomRight = new Vector3(bounds.max.x + SkinWidth, bounds.min.y - SkinWidth);
        _raycastBottomLeft = new Vector3(bounds.min.x - SkinWidth, bounds.min.y - SkinWidth);

    }

    private void MoveHorizontally(ref Vector2 deltaMovement)
    {
        
        var isGoingRight = deltaMovement.x > 0;
        var rayDistance = Mathf.Abs(deltaMovement.x) + SkinWidth;
        var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
        var rayOrigin = isGoingRight ? _raycastBottomRight : _raycastBottomLeft;
        Debug.LogFormat("rayOrigin:{0}", rayOrigin);

        for (var i = 0; i < TotalHorizontalRays; i++)
        {
            var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));
            Debug.Log(string.Format("i:{0}", i));
            Debug.Log(string.Format("deltaMovement.x:{0}", deltaMovement.x));
            Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);
            Debug.LogFormat("rayVector:{0}, rayDirection:{1}, rayDistance:{2}", rayVector, rayDirection, rayDistance);


            var rayCastHit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, PlatformMask);


            //Bounds bounds = _boxCollider.bounds;

            //bounds.Expand(SkinWidth * -2);
            //_raycastTopLeft = new Vector3(bounds.min.x - SkinWidth, bounds.max.y + SkinWidth);
            //_raycastBottomRight = new Vector3(bounds.max.x + SkinWidth, bounds.min.y - SkinWidth);
            //_raycastBottomLeft = new Vector3(bounds.min.x - SkinWidth, bounds.min.y - SkinWidth);

            Debug.LogFormat("rayOriginLoop:{0}", rayOrigin);
            Debug.LogFormat("rayCastHit:{0}", rayCastHit.point);

            Debug.Log(PlatformMask.value.ToString());



            if (rayCastHit.collider == null || rayCastHit.collider == _boxCollider)
            {
                Debug.Log("Continue");
                continue;
            }

            if (i == 0 && HandleHorizontalSlope(ref deltaMovement, Vector2.Angle(rayCastHit.normal, Vector2.up), isGoingRight))
            {
                break;
            }

            deltaMovement.x = rayCastHit.point.x - rayVector.x;
            rayDistance = Mathf.Abs(deltaMovement.x);

            if (isGoingRight)
            {
                deltaMovement.x -= SkinWidth;
                State.IsCollidingRight = true;
            }
            else
            {
                deltaMovement.x += SkinWidth;
                State.IsCollidingLeft = true;
            }

            if (rayDistance < SkinWidth + .0001f)
            {
                Debug.Log(string.Format("rayDistance:{0}", rayDistance));
                Debug.Log(string.Format("SkinWidth:{0}", SkinWidth));

                break;
            }
        }

    }

    private void MoveVertically(ref Vector2 deltaMovement)
    {

    }

    private bool HandleHorizontalSlope(ref Vector2 deltaMovement, float angle, bool isGoingRight)
    {
        return false;
    }

    private void HandleVerticalSlope(ref Vector2 deltaMovement)
    {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {

    }

    public void OnTriggerExit2D(Collider2D other)
    {

    }
}

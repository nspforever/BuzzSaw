using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private bool _isFacingRight;
    private CharacterController2D _controller;
    private float _normalizedHorizontalSpeed;
    public float MaxSpeed;
    public float SpeedAccelerationOnGround = 10f;
    public float SpeedAccelerationInAir = 5f;

    public void Start()
    {
        _controller = GetComponent<CharacterController2D>();        
        _isFacingRight = transform.localScale.x > 0;
    }

    public void Update()
    {
        HandleInput();

        var movementFactor = _controller.State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;
        //Debug.Log(string.Format("movementFactory:{0}", movementFactory));
        //Debug.Log(string.Format("_controller.Velocity.x:{0}", _controller.Velocity.x));
        //Debug.Log(string.Format("_normalizedHorizontalSpeed * MaxSpeed:{0}", _normalizedHorizontalSpeed * MaxSpeed));

        _controller.SetHorizontalForce(Mathf.Lerp(_controller.Velocity.x, _normalizedHorizontalSpeed * MaxSpeed, Time.deltaTime * movementFactor));
        
    }

    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            _normalizedHorizontalSpeed = 1;
            Debug.Log(string.Format("_controller.Velocity.x:{0}", _controller.Velocity.x));
            Debug.Log(string.Format("_normalizedHorizontalSpeed * MaxSpeed:{0}", _normalizedHorizontalSpeed * MaxSpeed));

            if (!_isFacingRight)
            {
                Flip();
            }
        }
        else if(Input.GetKey(KeyCode.A))
        {
            _normalizedHorizontalSpeed = -1;
            if(_isFacingRight)
            {
                Flip();
            }

        }
        else
        {
            _normalizedHorizontalSpeed = 0;
        }
        
        if (_controller.CanJump && Input.GetKey(KeyCode.J))
        {
            _controller.Jump();
        }
    }

    private void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        _isFacingRight = transform.localScale.x > 0;
    }

}

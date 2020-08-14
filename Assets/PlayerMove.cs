using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed;
    public Transform movePoint;
    public LayerMask movementStopper;

    private float _horizontalMovement;
    private float _verticalMovement;
    private Vector3 _turnStartPosition;

    private void Start()
    {
        // don't have the movePoint as a child of the player itself
        movePoint.parent = null;
        _turnStartPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        PlayerMovementInput();
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerMovementInput()
    {
        var playerNearTurnStartPosition = Vector3.Distance(_turnStartPosition, movePoint.position) <= 0.5f;

        // only change horizontal movement when it is not already being moved on
        if (Math.Abs(_horizontalMovement) < 0.01)
        {
            var horizontalInput = Input.GetAxisRaw("Horizontal");
            if (Math.Abs(horizontalInput) > 0.999)
            {
                Vector3 newPosition;
                if (playerNearTurnStartPosition)
                {
                    newPosition = _turnStartPosition + new Vector3(horizontalInput, 0f, 0f);
                }
                else
                {
                    newPosition = movePoint.position + new Vector3(horizontalInput, 0f, 0f);
                }

                if (!Physics2D.OverlapCircle(newPosition, .2f, movementStopper))
                {
                    _horizontalMovement = horizontalInput;
                    _verticalMovement = 0;
                    movePoint.position = newPosition;
                }
            }
        }

        // only change vertical movement when it is not already being moved on
        if (Math.Abs(_verticalMovement) < 0.01)
        {
            var verticalInput = Input.GetAxisRaw("Vertical");
            if (Math.Abs(verticalInput) > 0.999)
            {
                Vector3 newPosition;
                if (playerNearTurnStartPosition)
                {
                    newPosition = _turnStartPosition + new Vector3(0f, verticalInput, 0f);
                }
                else
                {
                    newPosition = movePoint.position + new Vector3(0f, verticalInput, 0f);
                }

                if (!Physics2D.OverlapCircle(newPosition, .2f, movementStopper))
                {
                    _verticalMovement = verticalInput;
                    _horizontalMovement = 0;
                    movePoint.position = newPosition;
                }
            }
        }
    }

    private void PlayerMovement()
    {
        if (Vector3.Distance(transform.position, movePoint.position) <= 0.001f)
        {
            var newPosition = movePoint.position + new Vector3(_horizontalMovement, _verticalMovement, 0f);
            if (!Physics2D.OverlapCircle(newPosition, 0.2f, movementStopper))
            {
                _turnStartPosition = movePoint.position;
                movePoint.position = newPosition;
            }
            else
            {
                _horizontalMovement = 0;
                _verticalMovement = 0;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed);
    }
}
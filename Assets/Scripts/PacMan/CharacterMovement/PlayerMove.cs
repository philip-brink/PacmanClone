using System;
using UnityEngine;

namespace PacMan.CharacterMovement
{
    public class PlayerMove : MonoBehaviour
    {
        public float moveSpeed;
        public Transform movePoint;
        public Transform futureMovePoint;
        public LayerMask movementLayer;

        private float _horizontalMovement;
        private float _verticalMovement;
        private Vector3 _turnStartPosition;
        private bool _movementSet = false;

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
            var playerNearTurnStartPosition = Vector3.Distance(_turnStartPosition, transform.position) <= 0.2f;
            var horizontalInput = Input.GetAxisRaw("Horizontal");
            var verticalInput = Input.GetAxisRaw("Vertical");
            bool pathExists = false;

            if (Math.Abs(horizontalInput) > 0.999)
            {
                Vector3 newPosition = Vector3.zero;

                if (playerNearTurnStartPosition || _movementSet)
                {
                    var preferredPosition = _turnStartPosition + new Vector3(horizontalInput, 0f, 0f);
                    if (Physics2D.OverlapCircle(preferredPosition, .2f, movementLayer))
                    {
                        newPosition = preferredPosition;
                        pathExists = true;
                    }
                    else if (playerNearTurnStartPosition)
                    {
                        newPosition = movePoint.position + new Vector3(horizontalInput, 0f, 0f);
                        pathExists = Physics2D.OverlapCircle(newPosition, .2f, movementLayer);
                    }
                }
                else
                {
                    newPosition = movePoint.position + new Vector3(horizontalInput, 0f, 0f);
                    pathExists = Physics2D.OverlapCircle(newPosition, .2f, movementLayer);
                }

                if (pathExists)
                {
                    _horizontalMovement = horizontalInput;
                    _verticalMovement = 0;
                    _turnStartPosition = newPosition - new Vector3(horizontalInput, 0f, 0f);
                    movePoint.position = newPosition;
                    futureMovePoint.position = newPosition + new Vector3(horizontalInput, 0f, 0f);
                    _movementSet = true;
                }
            }
            else if (Math.Abs(verticalInput) > 0.999)
            {
                Vector3 newPosition = Vector3.zero;

                if (playerNearTurnStartPosition || _movementSet)
                {
                    var preferredPosition = _turnStartPosition + new Vector3(0f, verticalInput, 0f);
                    if (Physics2D.OverlapCircle(preferredPosition, .2f, movementLayer))
                    {
                        newPosition = preferredPosition;
                        pathExists = true;
                    }
                    else if (playerNearTurnStartPosition)
                    {
                        newPosition = movePoint.position + new Vector3(0f, verticalInput, 0f);
                        pathExists = Physics2D.OverlapCircle(newPosition, .2f, movementLayer);
                    }
                }
                else
                {
                    newPosition = movePoint.position + new Vector3(0f, verticalInput, 0f);
                    pathExists = Physics2D.OverlapCircle(newPosition, .2f, movementLayer);
                }

                if (pathExists)
                {
                    _verticalMovement = verticalInput;
                    _horizontalMovement = 0;
                    _turnStartPosition = newPosition - new Vector3(0f, verticalInput, 0f);
                    movePoint.position = newPosition;
                    futureMovePoint.position = newPosition + new Vector3(0f, verticalInput, 0f);
                    _movementSet = true;
                }
            }
        }

        private void PlayerMovement()
        {
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.001f)
            {
                var newPosition = movePoint.position + new Vector3(_horizontalMovement, _verticalMovement, 0f);
                if (Physics2D.OverlapCircle(newPosition, 0.2f, movementLayer))
                {
                    _turnStartPosition = movePoint.position;
                    movePoint.position = newPosition;
                }
                else
                {
                    _horizontalMovement = 0;
                    _verticalMovement = 0;
                }

                _movementSet = false;
            }

            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed);
        }
    }
}
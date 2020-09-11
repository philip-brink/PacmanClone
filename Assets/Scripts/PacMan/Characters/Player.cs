using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PacMan.Characters
{
    public class Player : MonoBehaviour
    {
        public float moveSpeed;
        public Transform movePoint;
        public Transform futureMovePoint;
        public Transform startPoint;
        public LayerMask movementLayer;
        public LayerMask teleportationLayer;
        public Tilemap railTiles;
        public GameObject aggressiveEnemy;
        public GameObject pincerEnemy;
        public GameObject whimsicalEnemy;
        public GameObject shyEnemy;
        public Animator animator;

        private float _horizontalMovement;
        private float _verticalMovement;
        private int _animatorHorizontalId;
        private int _animatorVerticalId;
        private int _animatorMovingId;
        private int _animatorKilledId;
        private Vector3 _turnStartPosition;
        private bool _movementSet;
        private Vector3Int _lastTileCheckedForDot = Vector3Int.zero;
        private Dictionary<GameObject, Enemy.Enemy> _enemies;
        private float _enemyCollisionDistance = 0.8f;
        private bool _killed;

        private GameController _gameController;
        private DotsController _dotsController;

        private void Start()
        {
            // initialize animator ids
            _animatorHorizontalId = Animator.StringToHash("Horizontal");
            _animatorVerticalId = Animator.StringToHash("Vertical");
            _animatorMovingId = Animator.StringToHash("Moving");
            _animatorKilledId = Animator.StringToHash("Killed");
            
            // don't have the movePoint as a child of the player itself
            movePoint.parent = null;
            _turnStartPosition = transform.position;

            _gameController = GameObject.Find("GameController").GetComponent<GameController>();
            _dotsController = GameObject.Find("Dots").GetComponent<DotsController>();

            _enemies = new Dictionary<GameObject, Enemy.Enemy>
            {
                {aggressiveEnemy, aggressiveEnemy.GetComponent<Enemy.Enemy>()},
                {pincerEnemy, pincerEnemy.GetComponent<Enemy.Enemy>()},
                {shyEnemy, shyEnemy.GetComponent<Enemy.Enemy>()},
                {whimsicalEnemy, whimsicalEnemy.GetComponent<Enemy.Enemy>()}
            };
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_killed) PlayerMovementInput();
        }

        private void FixedUpdate()
        {
            if (!_killed) PlayerMovement();
        }
        
        private void SetMovement(float x, float y, bool moving)
        {
            animator.SetInteger(_animatorHorizontalId, moving ? (int)x : (int)_horizontalMovement);
            animator.SetInteger(_animatorVerticalId, moving ? (int)y : (int)_verticalMovement);
            animator.SetBool(_animatorMovingId, moving);
                
            _horizontalMovement = x;
            _verticalMovement = y;
        }

        private void PlayerMovementInput()
        {
            var playerNearTurnStartPosition = Vector3.Distance(_turnStartPosition, transform.position) <= 0.3f;
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
                    SetMovement(horizontalInput, 0, true);
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
                    SetMovement(0, verticalInput, true);
                    _turnStartPosition = newPosition - new Vector3(0f, verticalInput, 0f);
                    movePoint.position = newPosition;
                    futureMovePoint.position = newPosition + new Vector3(0f, verticalInput, 0f);
                    _movementSet = true;
                }
            }
        }

        private void SetKilled()
        {
            _killed = true;
            animator.SetBool(_animatorKilledId, true);
        }
        
        public void Reincarnate()
        {
            animator.SetBool(_animatorKilledId, false);
            SetMovement(0, 0, false);
            var startPosition = startPoint.position;
            transform.position = startPosition;
            movePoint.position = startPosition;
            futureMovePoint.position = startPosition;
            _killed = false;
        }

        private void PlayerMovement()
        {
            // Check for any collisions with an enemy
            foreach (var enemy in _enemies)
            {
                if (Vector3.Distance(transform.position, enemy.Key.transform.position) <= _enemyCollisionDistance)
                {
                    if (_gameController.EnemyCollision(enemy.Value)) SetKilled();
                }
            }

            // If near the center of the next node, calculate the next move position
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.001f)
            {
                // teleportation
                if (Physics2D.OverlapCircle(movePoint.position, 0.2f, teleportationLayer))
                {
                    var cellBounds = railTiles.cellBounds;
                    var xMin = cellBounds.xMin + 1;
                    var xMax = cellBounds.xMax;
                    var yMin = cellBounds.yMin + 1;
                    var yMax = cellBounds.yMax;

                    var position = movePoint.position;
                    var currentPos = Vector3Int.RoundToInt(position);
                    Vector3 newPos;
                    if (currentPos.x == xMin) newPos = new Vector3(xMax, currentPos.y, currentPos.z);
                    else if (currentPos.x == xMax) newPos = new Vector3(xMin, currentPos.y, currentPos.z);
                    else if (currentPos.y == yMin) newPos = new Vector3(currentPos.x, yMax, currentPos.z);
                    else if (currentPos.y == yMax) newPos = new Vector3(currentPos.x, yMin, currentPos.z);
                    else newPos = currentPos;

                    transform.position = newPos;
                    movePoint.position = newPos;
                }

                // standard movement
                var newPosition = movePoint.position + new Vector3(_horizontalMovement, _verticalMovement, 0f);
                if (Physics2D.OverlapCircle(newPosition, 0.2f, movementLayer))
                {
                    _turnStartPosition = movePoint.position;
                    movePoint.position = newPosition;
                }
                else
                {
                    SetMovement(0, 0, false);
                }

                _movementSet = false;
            }

            // pickup dots
            var positionInt = Vector3Int.RoundToInt(transform.position) - new Vector3Int(1, 1, 0);
            if (positionInt != _lastTileCheckedForDot)
            {
                var dotConsumed = _dotsController.PlayerMovedToPosition(positionInt);
                switch (dotConsumed)
                {
                    case DotType.Normal:
                        _gameController.DotCollected();
                        break;
                    case DotType.Power:
                        _gameController.PowerDotCollected();
                        break;
                }
                
                _lastTileCheckedForDot = positionInt;
            }

            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed);
        }
    }
}
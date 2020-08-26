using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace PacMan.CharacterMovement.Enemy
{
    public abstract class EnemyMovement : MonoBehaviour
    {
        public Transform playerPoint;
        public Transform playerFuturePoint;
        public Transform movePoint;
        public Transform cornerPoint;
        public LayerMask movementLayer;
        public float moveSpeed;
        public Animator animator;

        public MovementMode MovementMode
        {
            get => _movementMode;
            set
            {
                _movementMode = value;

                // reverse movement
                if (_movementMode == MovementMode.Flee)
                {
                    ReverseMovement();
                }
            }
        }

        protected Vector3 PreviousPosition;
        private MovementMode _movementMode = MovementMode.Chase;
        private int _verticalMovement;
        private int _horizontalMovement;
        private readonly Random _random = new Random();
        private int _animatorHorizontalId;
        private int _animatorVerticalId;

        private void Start()
        {
            // don't have the movePoint as a child of the player itself
            movePoint.parent = null;
            _animatorHorizontalId = Animator.StringToHash("Horizontal");
            _animatorVerticalId = Animator.StringToHash("Vertical");
        }

        private void FixedUpdate()
        {
            MoveEnemy();
        }

        private void MoveEnemy()
        {
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.01f)
            {
                var nextIntersection = CheckForIntersection(movePoint.position);
                if (nextIntersection.HasIntersection)
                {
                    var newDirection = TargetPosition();
                    var position = movePoint.position;
                    SetDirectionComponents(Vector3Int.RoundToInt(position),
                        Vector3Int.RoundToInt(newDirection));
                    PreviousPosition = position;
                    position = newDirection;
                    movePoint.position = position;
                }
                else
                {
                    if (_horizontalMovement == 0 && _verticalMovement == 0)
                    {
                        var newDirection = GetInitialTarget();
                        var position = movePoint.position;
                        SetDirectionComponents(Vector3Int.RoundToInt(position),
                            Vector3Int.RoundToInt(newDirection));
                        PreviousPosition = position;
                        position = newDirection;
                        movePoint.position = position;
                    }
                    else
                    {
                        var newDirection = movePoint.position + new Vector3(_horizontalMovement, _verticalMovement, 0f);
                        // make sure movement is possible
                        if (Physics2D.OverlapCircle(newDirection, 0.2f, movementLayer))
                        {
                            var position = movePoint.position;
                            SetDirectionComponents(Vector3Int.RoundToInt(position),
                                Vector3Int.RoundToInt(newDirection));
                            PreviousPosition = position;
                            position = newDirection;
                            movePoint.position = position;
                        }
                        else
                        {
                            ReverseMovement();
                        }
                    }
                }
            }

            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed);
        }

        private Vector3 TargetPosition()
        {
            switch (MovementMode)
            {
                case MovementMode.Chase:
                    return GetChaseTarget();
                case MovementMode.Scatter:
                    return GetScatterTarget();
                case MovementMode.Flee:
                    return GetFrightenedTarget();
                default:
                    return GetChaseTarget();
            }
        }

        protected abstract Vector3 GetChaseTarget();

        private void SetDirectionComponents(Vector3Int a, Vector3Int b)
        {
            if (a.x == b.x)
            {
                _horizontalMovement = 0;
            }
            else if (a.x < b.x)
            {
                _horizontalMovement = 1;
            }
            else
            {
                _horizontalMovement = -1;
            }

            if (a.y == b.y)
            {
                _verticalMovement = 0;
            }
            else if (a.y < b.y)
            {
                _verticalMovement = 1;
            }
            else
            {
                _verticalMovement = -1;
            }

            animator.SetInteger(_animatorHorizontalId, _horizontalMovement);
            animator.SetInteger(_animatorVerticalId, _verticalMovement);
        }
        
        private Vector3 GetScatterTarget()
        {
            var position = movePoint.position;
            var movePointPosition = position;
            var path = Pathfinding.GetPath(Vector3Int.RoundToInt(movePointPosition),
                Vector3Int.RoundToInt(cornerPoint.position), movementLayer, Vector3Int.RoundToInt(PreviousPosition));

            return path.Count > 1 ? path.ElementAt(1) : position;
        }

        private Vector3 GetFrightenedTarget()
        {
            var nextIntersection = CheckForIntersection(movePoint.position);
            if (nextIntersection.HasIntersection)
            {
                var validPoints = new List<Vector3>();
                if (nextIntersection.Left != Vector3.zero && _horizontalMovement != -1)
                    validPoints.Add(nextIntersection.Left);
                if (nextIntersection.Up != Vector3.zero && _verticalMovement != 1) validPoints.Add(nextIntersection.Up);
                if (nextIntersection.Right != Vector3.zero && _horizontalMovement != 1)
                    validPoints.Add(nextIntersection.Right);
                if (nextIntersection.Down != Vector3.zero && _verticalMovement != -1)
                    validPoints.Add(nextIntersection.Down);
                int randomIndex = _random.Next(validPoints.Count);
                return validPoints[randomIndex];
            }
            else
            {
                var newPosition = movePoint.position + new Vector3(_horizontalMovement, _verticalMovement, 0f);
                // make sure movement is possible in this direction
                if (Physics2D.OverlapCircle(newPosition, 0.2f, movementLayer))
                {
                    return newPosition;
                }

                ReverseMovement();
                return movePoint.position + new Vector3(_horizontalMovement, _verticalMovement, 0f);
            }
        }

        private Vector3 GetInitialTarget()
        {
            var nextIntersection = CheckForIntersection(movePoint.position);
            var validPoints = new List<Vector3>();
            if (nextIntersection.Left != Vector3.zero) validPoints.Add(nextIntersection.Left);
            if (nextIntersection.Up != Vector3.zero) validPoints.Add(nextIntersection.Up);
            if (nextIntersection.Right != Vector3.zero) validPoints.Add(nextIntersection.Right);
            if (nextIntersection.Down != Vector3.zero) validPoints.Add(nextIntersection.Down);
            int randomIndex = _random.Next(validPoints.Count);
            return validPoints[randomIndex];
        }

        private IntersectionDirections CheckForIntersection(Vector3 target)
        {
            var left = target + Vector3.left;
            var up = target + Vector3.up;
            var right = target + Vector3.right;
            var down = target + Vector3.down;

            bool leftOpen = Physics2D.OverlapCircle(left, 10.9f, movementLayer);
            bool upOpen = Physics2D.OverlapCircle(up, 10.9f, movementLayer);
            bool rightOpen = Physics2D.OverlapCircle(right, 10.9f, movementLayer);
            bool downOpen = Physics2D.OverlapCircle(down, 10.9f, movementLayer);
            //
            // Debug.Log($"leftOpen: {leftOpen}");
            // Debug.Log($"rightOpen: {rightOpen}");
            // Debug.Log($"downOpen: {downOpen}");
            // Debug.Log($"upOpen: {upOpen}");

            return new IntersectionDirections(leftOpen ? left : Vector3.zero, upOpen ? up : Vector3.zero,
                rightOpen ? right : Vector3.zero, downOpen ? down : Vector3.zero);
        }

        private void ReverseMovement()
        {
            _horizontalMovement *= -1;
            _verticalMovement *= -1;
        }
    }

    public class IntersectionDirections
    {
        public readonly bool HasIntersection;
        public readonly Vector3 Left;
        public readonly Vector3 Up;
        public readonly Vector3 Right;
        public readonly Vector3 Down;

        public IntersectionDirections(Vector3 left, Vector3 up, Vector3 right, Vector3 down)
        {
            this.Left = left;
            this.Up = up;
            this.Right = right;
            this.Down = down;

            var corner = left != Vector3.zero && up != Vector3.zero || left != Vector3.zero && down != Vector3.zero ||
                         right != Vector3.zero && up != Vector3.zero || right != Vector3.zero && down != Vector3.zero;
            HasIntersection = corner;
        }
    }

    public enum MovementMode
    {
        Chase,
        Scatter,
        Flee,
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using PacMan.Characters.Enemy.States;
using UnityEngine;

namespace PacMan.Characters.Enemy
{
    public class Enemy : MonoBehaviour
    {
        public Transform playerPoint;
        public Transform playerFuturePoint;
        public Transform movePoint;
        public Transform cornerPoint;
        public Transform startPoint;
        public Transform startExitPoint;
        public Transform exitPoint;
        public LayerMask movementLayer;
        public SpriteRenderer spriteRenderer;
        public float moveSpeed;
        public float initialWaitTime;
        public float killedWaitTime;
        public Animator animator;
        public EnemyType enemyType;
        [NonSerialized] public Vector3 previousPosition;
        [NonSerialized] public Enemy AggressiveEnemy;

        private bool _fleeing;

        public bool Fleeing
        {
            get => _fleeing;
            set
            {
                if (!(_stateMachine.CurrentState is Waiting))
                {
                    _fleeing = value;
                }
            }
        }

        private bool _killed;

        public bool Killed
        {
            get => _killed;
            set
            {
                if (value) _fleeing = false;
                _killed = value;
            }
        }

        private int _verticalMovement;
        private int _horizontalMovement;
        private int _animatorHorizontalId;
        private int _animatorVerticalId;
        private StateMachine<IEnemyState> _stateMachine;

        public Vector3 MovementVector
        {
            get
            {
                if (_horizontalMovement == -1) return Vector3.left;
                if (_horizontalMovement == 1) return Vector3.right;
                if (_verticalMovement == -1) return Vector3.down;
                if (_verticalMovement == 1) return Vector3.up;

                return Vector3.zero;
            }
        }

        public void Reset()
        {
            var position = startPoint.position;
            transform.position = position;
            movePoint.position = position;
            
            Start();
        }

        private void Start()
        {
            AggressiveEnemy = GameObject.Find("EnemyAggressive").GetComponent<Enemy>();

            // don't have the movePoint as a child of the player itself
            movePoint.parent = null;
            _animatorHorizontalId = Animator.StringToHash("Horizontal");
            _animatorVerticalId = Animator.StringToHash("Vertical");

            _stateMachine = new StateMachine<IEnemyState>();

            var chasing = new Chasing(this);
            var scattering = new Scattering(this);
            var fleeing = new Fleeing(this);
            var killed = new Killed(this);
            var waiting = new Waiting(this);

            At(waiting, chasing, WaitTimeUp());
            At(chasing, scattering, ChaseTimeUp());
            At(scattering, chasing, ScatterTimeUp());
            At(fleeing, chasing, FleeTimeUp());
            At(killed, waiting, KilledArrivedAtStart());

            AtAny(fleeing, EnemyFleeing());
            AtAny(killed, EnemyKilled());

            _stateMachine.SetState(waiting);

            void At(IEnemyState from, IEnemyState to, Func<bool> condition) =>
                _stateMachine.AddTransition(from, to, condition);

            void AtAny(IEnemyState to, Func<bool> condition) => _stateMachine.AddAnyTransition(to, condition);
            Func<bool> WaitTimeUp() => () => waiting.TimeUp;
            Func<bool> KilledArrivedAtStart() => () => killed.ArrivedAtStartPosition;
            Func<bool> ChaseTimeUp() => () => chasing.TimeUp;
            Func<bool> ScatterTimeUp() => () => scattering.TimeUp;
            Func<bool> FleeTimeUp() => () => fleeing.TimeUp;
            Func<bool> EnemyFleeing() => () => Fleeing;
            Func<bool> EnemyKilled() => () => Killed;
        }

        private void FixedUpdate()
        {
            _stateMachine.Tick();
            MoveEnemy();
        }

        private void MoveEnemy()
        {
            if (_stateMachine.CurrentState is Waiting || _stateMachine.CurrentState is Killed) return;

            if (Vector3.Distance(transform.position, movePoint.position) <= 0.01f)
            {
                var nextIntersection = CheckForIntersection(movePoint.position);
                if (nextIntersection.HasIntersection)
                {
                    var newDirection = _stateMachine.CurrentState.TargetPosition();
                    var position = movePoint.position;
                    SetDirectionComponents(Vector3Int.RoundToInt(position),
                        Vector3Int.RoundToInt(newDirection));
                    previousPosition = position;
                    position = newDirection;
                    movePoint.position = position;
                }
                else
                {
                    if (_horizontalMovement == 0 && _verticalMovement == 0)
                    {
                        var newDirection = GetValidDirections(nextIntersection, Vector3.zero).First();
                        var position = movePoint.position;
                        SetDirectionComponents(Vector3Int.RoundToInt(position),
                            Vector3Int.RoundToInt(newDirection));
                        previousPosition = position;
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
                            previousPosition = position;
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

        public List<Vector3> GetValidDirections(IntersectionDirections intersection, Vector3 movementVector)
        {
            var movement = movementVector;
            bool notMoving = movement == Vector3.zero;
            var validPoints = new List<Vector3>();

            if (intersection.HasIntersection)
            {
                if (intersection.Left != Vector3.zero && (notMoving || movement != Vector3.right))
                    validPoints.Add(intersection.Left);
                if (intersection.Up != Vector3.zero && (notMoving || movement != Vector3.down))
                    validPoints.Add(intersection.Up);
                if (intersection.Right != Vector3.zero && (notMoving || movement != Vector3.left))
                    validPoints.Add(intersection.Right);
                if (intersection.Down != Vector3.zero && (notMoving || movement != Vector3.up))
                    validPoints.Add(intersection.Down);
            }

            return validPoints;
        }

        public IntersectionDirections CheckForIntersection(Vector3 target)
        {
            var left = target + Vector3.left;
            var up = target + Vector3.up;
            var right = target + Vector3.right;
            var down = target + Vector3.down;

            bool leftOpen = Physics2D.OverlapCircle(left, 0.2f, movementLayer);
            bool upOpen = Physics2D.OverlapCircle(up, 0.2f, movementLayer);
            bool rightOpen = Physics2D.OverlapCircle(right, 0.2f, movementLayer);
            bool downOpen = Physics2D.OverlapCircle(down, 0.2f, movementLayer);

            return new IntersectionDirections(leftOpen ? left : Vector3.zero, upOpen ? up : Vector3.zero,
                rightOpen ? right : Vector3.zero, downOpen ? down : Vector3.zero);
        }

        public void ReverseMovement()
        {
            previousPosition = Vector3.zero;
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
            Left = left;
            Up = up;
            Right = right;
            Down = down;

            var corner = left != Vector3.zero && up != Vector3.zero || left != Vector3.zero && down != Vector3.zero ||
                         right != Vector3.zero && up != Vector3.zero || right != Vector3.zero && down != Vector3.zero;
            HasIntersection = corner;
        }
    }

    public enum EnemyType
    {
        Aggressive,
        Pincer,
        Shy,
        Whimsical
    }
}
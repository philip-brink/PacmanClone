using System.Linq;
using UnityEngine;

namespace PacMan.Characters.Enemy.States
{
    public class Chasing : IEnemyState
    {
        private float _timeLeft;
        private int _roundNumber; 
        private readonly float[] _chaseTimes = {7f, 7f, 5f, 5f};
        private Enemy _enemy;

        public Chasing(Enemy enemy)
        {
            _enemy = enemy;
        }
        
        public void Tick()
        {
            _timeLeft -= Time.deltaTime;
        }

        public bool TimeUp => _timeLeft <= 0;

        public void OnEnter()
        {
            _timeLeft = _chaseTimes[_roundNumber < _chaseTimes.Length ? _roundNumber : _chaseTimes.Length - 1];
        }

        public void OnExit()
        {
            _roundNumber++;
        }

        public Vector3 TargetPosition()
        {
            switch (_enemy.enemyType)
            {
                case EnemyType.Aggressive:
                    return AggressiveTargetPosition();
                case EnemyType.Pincer:
                    return PincerTargetPosition();
                case EnemyType.Shy:
                    return ShyTargetPosition();
                case EnemyType.Whimsical:
                    return WhimsicalTargetPosition();
                default:
                    return AggressiveTargetPosition();
            }
        }

        private Vector3 AggressiveTargetPosition()
        {
            var movePointPosition = _enemy.movePoint.position;
            var path = Pathfinding.GetPath(Vector3Int.RoundToInt(movePointPosition),
                Vector3Int.RoundToInt(_enemy.playerPoint.position), _enemy.movementLayer,
                Vector3Int.RoundToInt(_enemy.previousPosition));

            return path.Count > 1 ? path.ElementAt(1) : movePointPosition;
        }
        
        private Vector3 PincerTargetPosition()
        {
            var movePointPosition = _enemy.movePoint.position;

            var position = _enemy.playerPoint.position;
            var playerChange = (_enemy.playerFuturePoint.position - position) * 2;

            var path = Pathfinding.GetPath(Vector3Int.RoundToInt(movePointPosition),
                Vector3Int.RoundToInt(position + playerChange), _enemy.movementLayer,
                Vector3Int.RoundToInt(_enemy.previousPosition));

            if (path.Count > 1)
            {
                return path.ElementAt(1);
            }
            else
            {
                return movePointPosition;
            }
        }
        
        private Vector3 ShyTargetPosition()
        {
            var movePointPosition = _enemy.movePoint.position;
            var playerPosition = _enemy.playerPoint.position;
            bool targetPlayer = Vector3.Distance(movePointPosition, playerPosition) > 8;
            var path = Pathfinding.GetPath(Vector3Int.RoundToInt(movePointPosition),
                Vector3Int.RoundToInt(targetPlayer ? playerPosition : _enemy.cornerPoint.position), _enemy.movementLayer,
                Vector3Int.RoundToInt(_enemy.previousPosition));

            if (path.Count > 1)
            {
                return path.ElementAt(1);
            }
            else
            {
                return _enemy.movePoint.position;
            }
        }
        
        private Vector3 WhimsicalTargetPosition()
        {
            var movePointPosition = _enemy.movePoint.position;

            var playerPosition = _enemy.playerPoint.position;
            var playerChange = (_enemy.playerFuturePoint.position - playerPosition) * 1;
            var pivotPoint = playerPosition + playerChange;
            var targetPoint = pivotPoint + (pivotPoint - _enemy.AggressiveEnemy.movePoint.position);

            var path = Pathfinding.GetPath(Vector3Int.RoundToInt(movePointPosition),
                Vector3Int.RoundToInt(targetPoint), _enemy.movementLayer,
                Vector3Int.RoundToInt(_enemy.previousPosition));

            if (path.Count > 1)
            {
                return path.ElementAt(1);
            }
            else
            {
                return movePointPosition;
            }
        }
    }
}
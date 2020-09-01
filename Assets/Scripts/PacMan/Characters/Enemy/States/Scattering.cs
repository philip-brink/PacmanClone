using System.Linq;
using UnityEngine;

namespace PacMan.Characters.Enemy.States
{
    public class Scattering : IEnemyState
    {
        private float _timeLeft;
        private int _roundNumber; 
        private readonly float[] _scatterTimes = {7f, 7f, 5f, 5f};
        private readonly Enemy _enemy;

        public Scattering(Enemy enemy)
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
            _timeLeft = _scatterTimes[_roundNumber < _scatterTimes.Length ? _roundNumber : _scatterTimes.Length - 1];
        }

        public void OnExit()
        {
            _roundNumber++;
        }

        public Vector3 TargetPosition()
        {
            var position = _enemy.movePoint.position;
            var movePointPosition = position;
            var path = Pathfinding.GetPath(Vector3Int.RoundToInt(movePointPosition),
                Vector3Int.RoundToInt(_enemy.cornerPoint.position), _enemy.movementLayer, Vector3Int.RoundToInt(_enemy.previousPosition));

            return path.Count > 1 ? path.ElementAt(1) : position;
        }
    }
}
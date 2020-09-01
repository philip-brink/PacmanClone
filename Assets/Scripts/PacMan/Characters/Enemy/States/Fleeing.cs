using UnityEngine;
using Random = System.Random;

namespace PacMan.Characters.Enemy.States
{
    public class Fleeing : IEnemyState
    {
        private float _timeLeft;
        private int _roundNumber; 
        private readonly float[] _fleeingTimes = {7f, 7f, 5f, 5f};
        private readonly Enemy _enemy;
        private readonly Random _random = new Random();

        public Fleeing(Enemy enemy)
        {
            _enemy = enemy;
        }

        public void Tick()
        {
            _timeLeft -= Time.deltaTime;
            if (TimeUp)
            {
                _enemy.Fleeing = false;
            }
        }

        public bool TimeUp => _timeLeft <= 0;

        public void OnEnter()
        {
            _timeLeft = _fleeingTimes[_roundNumber < _fleeingTimes.Length ? _roundNumber : _fleeingTimes.Length - 1];
            _enemy.spriteRenderer.color = Color.blue;
        }

        public void OnExit()
        {
            _roundNumber++;
            _enemy.spriteRenderer.color = Color.white;
        }

        public Vector3 TargetPosition()
        {
            var intersection = _enemy.CheckForIntersection(_enemy.movePoint.position);
            var directions = _enemy.GetValidDirections(intersection, _enemy.MovementVector);
            if (directions.Count > 0)
            {
                int randomIndex = _random.Next(directions.Count);
                return directions[randomIndex];
            }
            else
            {
                var newDirections = _enemy.GetValidDirections(intersection, Vector3.zero);
                int randomIndex = _random.Next(newDirections.Count);
                return newDirections[randomIndex];
            }
        }
    }
}
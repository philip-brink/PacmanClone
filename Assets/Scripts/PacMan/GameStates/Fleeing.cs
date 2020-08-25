using System.Linq;
using PacMan.CharacterMovement.Enemy;
using UnityEngine;

namespace PacMan.GameStates
{
    public class Fleeing : IState
    {
        private float _timeLeft;
        private int _roundNumber; 
        private readonly float[] _fleeingTimes = {7f, 7f, 5f, 5f};
        private readonly EnemyMovement[] _enemies;

        public Fleeing(EnemyMovement[] enemies)
        {
            _enemies = enemies;
        }

        public void Tick()
        {
            _timeLeft -= Time.deltaTime;
        }

        public bool TimeUp => _timeLeft <= 0;

        public void OnEnter()
        {
            _timeLeft = _fleeingTimes[_roundNumber < _fleeingTimes.Length ? _roundNumber : _fleeingTimes.Length - 1];
            
            foreach (var enemy in _enemies)
            {
                enemy.MovementMode = MovementMode.Flee;
            }
        }

        public void OnExit()
        {
            _roundNumber++;
        }
    }
}
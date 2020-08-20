using PacMan.CharacterMovement.Enemy;
using UnityEngine;

namespace PacMan.GameStates
{
    public class Chasing : IState
    {
        private float _timeLeft;
        private int _roundNumber; 
        private readonly float[] _chaseTimes = {7f, 7f, 5f, 5f};
        private readonly EnemyMovement[] _enemies;

        public Chasing(EnemyMovement[] enemies)
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
            Debug.Log("Started Chasing");
            
            _timeLeft = _chaseTimes[_roundNumber < _chaseTimes.Length ? _roundNumber : _chaseTimes.Length - 1];
            
            foreach (var enemy in _enemies)
            {
                enemy.MovementMode = MovementMode.Chase;
            }
        }

        public void OnExit()
        {
            _roundNumber++;
            Debug.Log("OnExit");
        }
    }
}
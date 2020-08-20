using System.Linq;
using PacMan.CharacterMovement.Enemy;
using UnityEngine;

namespace PacMan.GameStates
{
    public class Scattering : IState
    {
        private float _timeLeft;
        private int _roundNumber; 
        private readonly float[] _scatterTimes = {7f, 7f, 5f, 5f};
        private readonly EnemyMovement[] _enemies;

        public Scattering(EnemyMovement[] enemies)
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
            Debug.Log("Started Scattering");
            
            _timeLeft = _scatterTimes[_roundNumber < _scatterTimes.Length ? _roundNumber : _scatterTimes.Length - 1];
            
            foreach (var enemy in _enemies)
            {
                enemy.MovementMode = MovementMode.Scatter;
            }
        }

        public void OnExit()
        {
            _roundNumber++;
        }
    }
}
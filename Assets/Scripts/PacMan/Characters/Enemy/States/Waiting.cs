using UnityEngine;

namespace PacMan.Characters.Enemy.States
{
    public class Waiting : IEnemyState
    {
        private float _timeLeft;
        private bool _initialWait = true;
        private readonly Enemy _enemy;

        public Waiting(Enemy enemy)
        {
            _enemy = enemy;
        }
        
        public void Tick()
        {
            _timeLeft -= Time.deltaTime;
            if (_timeLeft < 1)
            {
                _enemy.transform.position = Vector3.MoveTowards(_enemy.transform.position, _enemy.exitPoint.position, _enemy.moveSpeed / 1.8f);
            }
            else if (_timeLeft < 2)
            {
                _enemy.transform.position = Vector3.MoveTowards(_enemy.transform.position, _enemy.startExitPoint.position, _enemy.moveSpeed / 3f);
            }
        }

        public bool TimeUp => _timeLeft <= 0;

        public void OnEnter()
        {
            _timeLeft = _initialWait ? _enemy.initialWaitTime : _enemy.killedWaitTime;
            var position = _enemy.startPoint.position;
            _enemy.transform.position = position;
            _enemy.movePoint.position = position;
        }

        public void OnExit()
        {
            _initialWait = false;
            var position = _enemy.exitPoint.position;
            _enemy.transform.position = position;
            _enemy.movePoint.position = position;
        }

        public Vector3 TargetPosition()
        {
            return _enemy.startPoint.position;
        }
    }
}
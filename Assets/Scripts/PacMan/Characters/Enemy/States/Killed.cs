using System.Linq;
using UnityEngine;

namespace PacMan.Characters.Enemy.States
{
    public class Killed : IEnemyState
    {
        private readonly Enemy _enemy;

        public Killed(Enemy enemy)
        {
            _enemy = enemy;
        }
        
        public void Tick()
        {
            _enemy.transform.position = Vector3.MoveTowards(_enemy.transform.position, TargetPosition(), _enemy.moveSpeed * 1.5f);
            if (ArrivedAtStartPosition)
            {
                _enemy.Killed = false;
                var position = _enemy.startPoint.position;
                _enemy.transform.position = position;
                _enemy.movePoint.position = position;
                _enemy.spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            }
        }

        public bool ArrivedAtStartPosition =>
            Vector3.Distance(_enemy.transform.position, _enemy.startPoint.position) < 0.1f;

        public void OnEnter()
        {
            _enemy.spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        }

        public void OnExit()
        {
        }

        public Vector3 TargetPosition()
        {
            return _enemy.startPoint.position;
        }
    }
}
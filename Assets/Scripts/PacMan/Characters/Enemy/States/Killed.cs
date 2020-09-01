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
        }

        public bool ArrivedAtStartPosition =>
            Vector3.Distance(_enemy.movePoint.position, _enemy.startPoint.position) < 1f;

        public void OnEnter()
        {
        }

        public void OnExit()
        {
            _enemy.Killed = false;
            var position = _enemy.startPoint.position;
            _enemy.transform.position = position;
            _enemy.movePoint.position = position;
        }

        public Vector3 TargetPosition()
        {
            var movePointPosition = _enemy.movePoint.position;
            var path = Pathfinding.GetPath(Vector3Int.RoundToInt(movePointPosition),
                Vector3Int.RoundToInt(_enemy.startPoint.position), _enemy.movementLayer,
                Vector3Int.RoundToInt(_enemy.previousPosition));

            return path.Count > 1 ? path.ElementAt(1) : movePointPosition;
        }
    }
}
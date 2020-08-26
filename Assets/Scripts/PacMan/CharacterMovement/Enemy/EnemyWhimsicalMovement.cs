using System.Linq;
using UnityEngine;

namespace PacMan.CharacterMovement.Enemy
{
    public class EnemyWhimsicalMovement : EnemyMovement
    {
        public Transform enemyAggressivePoint;

        protected override Vector3 GetChaseTarget()
        {
            var movePointPosition = movePoint.position;

            var playerPosition = playerPoint.position;
            var playerChange = (playerFuturePoint.position - playerPosition) * 1;
            var pivotPoint = playerPosition + playerChange;
            var targetPoint = pivotPoint + (pivotPoint - enemyAggressivePoint.position);

            var path = Pathfinding.GetPath(Vector3Int.RoundToInt(movePointPosition),
                Vector3Int.RoundToInt(targetPoint), movementLayer,
                Vector3Int.RoundToInt(PreviousPosition));

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
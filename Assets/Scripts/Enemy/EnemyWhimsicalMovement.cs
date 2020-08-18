using System.Linq;
using UnityEngine;

namespace Enemy
{
    public class EnemyWhimsicalMovement : EnemyMovement
    {
        public Transform enemyAggressivePoint;

        protected override Vector3 GetChaseTarget()
        {
            var movePointPosition = movePoint.position;

            var playerPosition = playerPoint.position;
            var playerChange = (playerFuturePoint.position - playerPosition) * 2;
            var pivotPoint = playerPosition + playerChange;
            var targetPoint = pivotPoint + (pivotPoint - enemyAggressivePoint.position);

            var path = Pathfinding.GetPath(Vector3Int.RoundToInt(movePointPosition),
                Vector3Int.RoundToInt(targetPoint), movementStopper,
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
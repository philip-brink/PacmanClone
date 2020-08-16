using System.Linq;
using UnityEngine;

namespace Enemy
{
    public class EnemyAggressiveMovement : EnemyMovement
    {
        protected override Vector3 GetChaseTarget()
        {
            var movePointPosition = movePoint.position;
            var path = Pathfinding.GetPath(Vector3Int.RoundToInt(movePointPosition),
                Vector3Int.RoundToInt(playerPoint.position), movementStopper, Vector3Int.RoundToInt(PreviousPosition));

            if (path.Count > 1)
            {
                return path.ElementAt(1);
            }
            else
            {
                return movePoint.position;
            }
        }
    }
}
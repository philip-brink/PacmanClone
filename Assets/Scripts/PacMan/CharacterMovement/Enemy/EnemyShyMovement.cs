using System.Linq;
using UnityEngine;

namespace PacMan.CharacterMovement.Enemy
{
    public class EnemyShyMovement : EnemyMovement
    {
        protected override Vector3 GetChaseTarget()
        {
            var movePointPosition = movePoint.position;
            var playerPosition = playerPoint.position;
            bool targetPlayer = Vector3.Distance(movePointPosition, playerPosition) > 8;
            var path = Pathfinding.GetPath(Vector3Int.RoundToInt(movePointPosition),
                Vector3Int.RoundToInt(targetPlayer ? playerPosition : cornerPoint.position), movementLayer,
                Vector3Int.RoundToInt(PreviousPosition));

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
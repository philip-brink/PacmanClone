using System.Linq;
using UnityEngine;

namespace PacMan.CharacterMovement.Enemy
{
    public class EnemyPincerMovement : EnemyMovement
    {
        protected override Vector3 GetChaseTarget()
        {
            var movePointPosition = movePoint.position;

            var position = playerPoint.position;
            var playerChange = (playerFuturePoint.position - position) * 4;

            var path = Pathfinding.GetPath(Vector3Int.RoundToInt(movePointPosition),
                Vector3Int.RoundToInt(position + playerChange), movementStopper,
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
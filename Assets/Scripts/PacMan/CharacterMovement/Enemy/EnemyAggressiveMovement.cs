using System.Linq;
using UnityEngine;

namespace PacMan.CharacterMovement.Enemy
{
    public class EnemyAggressiveMovement : EnemyMovement
    {
        protected override Vector3 GetChaseTarget()
        {
            var movePointPosition = movePoint.position;
            var path = Pathfinding.GetPath(Vector3Int.RoundToInt(movePointPosition),
                Vector3Int.RoundToInt(playerPoint.position), movementLayer, Vector3Int.RoundToInt(PreviousPosition));

            return path.Count > 1 ? path.ElementAt(1) : movePointPosition;
        }
    }
}
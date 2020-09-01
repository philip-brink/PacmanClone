using UnityEngine;

namespace PacMan.Characters.Enemy.States
{
    public interface IEnemyState : IState
    {
        Vector3 TargetPosition();
    }
}
using System;
using PacMan.CharacterMovement.Enemy;
using PacMan.GameStates;
using UnityEngine;

namespace PacMan
{
    public class GameController : MonoBehaviour
    {
        private StateMachine _stateMachine;

        private void Start()
        {
            var enemies = new EnemyMovement[]
            {
                GameObject.Find("EnemyAggressive").GetComponent<EnemyAggressiveMovement>(),
                GameObject.Find("EnemyPincer").GetComponent<EnemyPincerMovement>(),
                GameObject.Find("EnemyShy").GetComponent<EnemyShyMovement>(),
                GameObject.Find("EnemyWhimsical").GetComponent<EnemyWhimsicalMovement>()
            };

            _stateMachine = new StateMachine();

            var chasing = new Chasing(enemies);
            var scattering = new Scattering(enemies);
            var fleeing = new Fleeing(enemies);
            
            At(chasing, scattering, ChaseTimeUp());
            At(scattering, chasing, ScatterTimeUp());
            At(fleeing, chasing, FleeTimeUp());
            
            AtAny(fleeing, PlayerPoweredUp());
            
            _stateMachine.SetState(chasing);

            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
            void AtAny(IState to, Func<bool> condition) => _stateMachine.AddAnyTransition(to, condition);
            Func<bool> ChaseTimeUp() => () => chasing.TimeUp;
            Func<bool> ScatterTimeUp() => () => scattering.TimeUp;
            Func<bool> FleeTimeUp() => () => fleeing.TimeUp;
            Func<bool> PlayerPoweredUp() => () => false;
        }

        private void FixedUpdate()
        {
            _stateMachine.Tick();
        }
    }
}
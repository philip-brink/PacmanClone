using System;
using System.Collections.Generic;
using PacMan.Characters.Enemy;
using PacMan.GameStates;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace PacMan
{
    public class GameController : MonoBehaviour
    {
        public Tilemap dotTiles;
        public Text score;
        public Text lives;
        public const string DotName = "dot";
        public const string PowerDotName = "power_dot";
        private StateMachine<IState> _stateMachine;
        private int _score;
        private int _numberDots;
        private int _numberDotsConsumed;
        private bool _playerKilled;
        private int _lives = 3;
        private List<Enemy> _enemies;

        public void DotCollected()
        {
            _numberDotsConsumed++;
            IncrementScore(1);
        }

        public void PowerDotCollected()
        {
            foreach (var enemy in _enemies)
            {
                enemy.Fleeing = true;
            }
            
            IncrementScore(25);
        }

        public void FruitCollected()
        {
            IncrementScore(200);
        }

        public void EnemyCollision(Enemy enemy)
        {
            if (enemy.Fleeing)
            {
                Debug.Log("CAUGHT GHOST");
                enemy.Killed = true;
            }
            else
            {
                _playerKilled = true;
                _lives--;
                SetLives();
            }
        }

        private void IncrementScore(int increment)
        {
            _score += increment;
            score.text = _score.ToString();
        }

        private void SetLives()
        {
            lives.text = $"Lives: {_lives}";
        }

        private void Start()
        {
            _numberDots = GetTileAmount(DotName);
            
            SetLives();
            
            _enemies = new List<Enemy>
            {
                GameObject.Find("EnemyAggressive").GetComponent<Enemy>(),
                GameObject.Find("EnemyPincer").GetComponent<Enemy>(),
                GameObject.Find("EnemyShy").GetComponent<Enemy>(),
                GameObject.Find("EnemyWhimsical").GetComponent<Enemy>()
            };

            _stateMachine = new StateMachine<IState>();


            var victorious = new Victorious();
            var failed = new Failed();
            var killed = new Killed();
            var playing = new Playing();
            
            At(playing, victorious, Victorious());
            At(playing, failed, Failed());
            At(playing, killed, PlayerKilled());

            _stateMachine.SetState(playing);

            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
            
            Func<bool> Victorious() => () => _numberDotsConsumed == _numberDots;
            Func<bool> Failed() => () => _lives <= 0;
            Func<bool> PlayerKilled() => () =>
            {
                if (_playerKilled)
                {
                    _playerKilled = false;
                    return true;
                }
                return false;
            };
        }

        private void FixedUpdate()
        {
            _stateMachine.Tick();
        }
        
        /// <summary> Get the amount of tiles controlled by a player </summary> 
        private int GetTileAmount(string tileName)
        {
            var amount = 0;
 
            // loop through all of the tiles        
            BoundsInt bounds = dotTiles.cellBounds;
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                Tile tile = dotTiles.GetTile<Tile>(pos);
                if (tile != null)
                {
                    if (tile.name == tileName)
                    {
                        amount += 1;
                    }
                }
            }
            return amount;
        }
    }
}
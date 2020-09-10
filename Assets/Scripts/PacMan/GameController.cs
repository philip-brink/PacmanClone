using System;
using System.Collections.Generic;
using PacMan.Characters;
using PacMan.Characters.Enemy;
using PacMan.GameStates;
using UnityEngine;
using UnityEngine.UI;

namespace PacMan
{
    public class GameController : MonoBehaviour
    {
        public Text score;
        public Text lives;
        public GameObject failureMenu;
        public GameObject victoryMenu;
        private StateMachine<IState> _stateMachine;
        private int _score;
        [NonSerialized] public bool PlayerKilled;
        [NonSerialized] public bool ResetGame;
        private int _lives = 3;
        private List<Enemy> _enemies;
        private Player _player;
        private DotsController _dotsController;

        public void DotCollected()
        {
            IncrementScore(1);
            AudioController.Instance.Play(SoundEffect.Munch);
        }

        public void PowerDotCollected()
        {
            foreach (var enemy in _enemies)
            {
                enemy.Fleeing = true;
            }

            IncrementScore(25);
            AudioController.Instance.Play(SoundEffect.PowerMunch);
        }

        public void FruitCollected()
        {
            IncrementScore(200);
        }

        public bool EnemyCollision(Enemy enemy)
        {
            if (enemy.Killed) return false;
            
            if (enemy.Fleeing)
            {
                switch (enemy.enemyType)
                {
                    case EnemyType.Aggressive:
                        Debug.Log("Aggressive killed");
                        AudioController.Instance.Play(SoundEffect.AggressiveEnemyKilled);
                        break;
                    case EnemyType.Pincer:
                        Debug.Log("Pincer killed");
                        AudioController.Instance.Play(SoundEffect.PincerEnemyKilled);
                        break;
                    case EnemyType.Shy:
                        Debug.Log("Shy killed");
                        AudioController.Instance.Play(SoundEffect.ShyEnemyKilled);
                        break;
                    case EnemyType.Whimsical:
                        Debug.Log("Whimsical killed");
                        AudioController.Instance.Play(SoundEffect.WhimsicalEnemyKilled);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                enemy.Killed = true;
                IncrementScore(400);
                return false;
            }
            else
            {
                switch (enemy.enemyType)
                {
                    case EnemyType.Aggressive:
                        AudioController.Instance.Play(SoundEffect.PlayerKilledByAggressive);
                        break;
                    case EnemyType.Pincer:
                        AudioController.Instance.Play(SoundEffect.PlayerKilledByPincer);
                        break;
                    case EnemyType.Shy:
                        AudioController.Instance.Play(SoundEffect.PlayerKilledByShy);
                        break;
                    case EnemyType.Whimsical:
                        AudioController.Instance.Play(SoundEffect.PlayerKilledByWhimsical);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                PlayerKilled = true;
                _lives--;
                SetLives();
                return true;
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

        public void RestartGame()
        {
            _lives = 3;
            SetLives();
            ResetGame = true;
            _score = 0;
            _dotsController.ResetDots();
            // _player.Reincarnate();
            foreach (var enemy in _enemies)
            {
                enemy.Reset();
            }
        }

        private void Start()
        {
            _dotsController = GameObject.Find("Dots").GetComponent<DotsController>();
            
            SetLives();

            _enemies = new List<Enemy>
            {
                GameObject.Find("EnemyAggressive").GetComponent<Enemy>(),
                GameObject.Find("EnemyPincer").GetComponent<Enemy>(),
                GameObject.Find("EnemyShy").GetComponent<Enemy>(),
                GameObject.Find("EnemyWhimsical").GetComponent<Enemy>()
            };

            _player = GameObject.Find("Player").GetComponent<Player>();

            _stateMachine = new StateMachine<IState>();


            var victorious = new Victorious(victoryMenu);
            var failed = new Failed(failureMenu);
            var killed = new Killed(_player, this);
            var playing = new Playing(this);

            At(playing, victorious, Victorious());
            At(killed, failed, Failed());
            At(playing, killed, PlayerDied());
            At(killed, playing, Reincarnate());
            AtAny(playing, GameRestarted());

            _stateMachine.SetState(playing);

            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
            void AtAny(IState to, Func<bool> condition) => _stateMachine.AddAnyTransition(to, condition);

            Func<bool> Victorious() => () => _dotsController.AllDotsConsumed;
            Func<bool> Failed() => () => killed.TimeUp && _lives <= 0;
            Func<bool> Reincarnate() => () => killed.TimeUp && _lives > 0;
            Func<bool> PlayerDied() => () => PlayerKilled;
            Func<bool> GameRestarted() => () => ResetGame;
        }

        private void FixedUpdate()
        {
            _stateMachine.Tick();
        }


    }
}
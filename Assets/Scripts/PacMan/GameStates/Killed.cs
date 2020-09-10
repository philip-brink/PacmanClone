using PacMan.Characters;
using UnityEngine;

namespace PacMan.GameStates
{
    public class Killed : IState
    {
        private readonly float _stuckTime = 2f;
        private float _timeLeft;
        private readonly Player _player;
        private readonly GameController _gameController;

        public Killed(Player player, GameController gameController)
        {
            _player = player;
            _gameController = gameController;
        }
        
        public bool TimeUp => _timeLeft <= 0;
        
        public void Tick()
        {
            _timeLeft -= Time.deltaTime;
        }


        public void OnEnter()
        {
            _gameController.PlayerKilled = false;
            _timeLeft = _stuckTime;
        }

        public void OnExit()
        {
            _player.Reincarnate();
        }
    }
}
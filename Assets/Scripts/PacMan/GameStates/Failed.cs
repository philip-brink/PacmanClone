using PacMan.Menu;
using UnityEngine;

namespace PacMan.GameStates
{
    public class Failed : IState
    {
        private readonly PauseMenu _pauseMenu;

        public Failed()
        {
            _pauseMenu = GameObject.Find("Canvas").GetComponent<PauseMenu>();        }
        
        public void Tick()
        {
        }


        public void OnEnter()
        {
            _pauseMenu.OpenFailureMenu();
            AudioController.Instance.PlayMusic(Music.Failure);
        }

        public void OnExit()
        {
        }
    }
}
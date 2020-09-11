using PacMan.Menu;
using UnityEngine;

namespace PacMan.GameStates
{
    public class Victorious : IState
    {
        private readonly PauseMenu _pauseMenu;
        
        public Victorious()
        {
            _pauseMenu = GameObject.Find("Canvas").GetComponent<PauseMenu>();
        }
        
        public void Tick()
        {
        }


        public void OnEnter()
        {
            Debug.Log("!!!!!!VICTORY!!!!!");
            _pauseMenu.OpenVictoryMenu();
            AudioController.Instance.PlayMusic(Music.Victory);
        }

        public void OnExit()
        {
        }
    }
}
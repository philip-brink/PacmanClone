using UnityEngine;

namespace PacMan.GameStates
{
    public class Failed : IState
    {
        private readonly GameObject _failedMenu;

        public Failed(GameObject failedMenu)
        {
            _failedMenu = failedMenu;
        }
        
        public void Tick()
        {
        }


        public void OnEnter()
        {
            _failedMenu.SetActive(true);
            AudioController.Instance.PlayMusic(Music.Failure);
        }

        public void OnExit()
        {
            _failedMenu.SetActive(false);
        }
    }
}
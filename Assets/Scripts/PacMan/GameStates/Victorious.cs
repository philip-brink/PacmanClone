using UnityEngine;

namespace PacMan.GameStates
{
    public class Victorious : IState
    {
        private GameObject _victoryMenu;
        
        public Victorious(GameObject victoryMenu)
        {
            _victoryMenu = victoryMenu;
        }
        
        public void Tick()
        {
        }


        public void OnEnter()
        {
            Debug.Log("!!!!!!VICTORY!!!!!");
            _victoryMenu.SetActive(true);
            AudioController.Instance.PlayMusic(Music.Victory);
        }

        public void OnExit()
        {
            _victoryMenu.SetActive(false);
        }
    }
}
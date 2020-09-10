using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PacMan.Menu
{
    public class Menu : MonoBehaviour
    {
        private void Start()
        {
            AudioController.Instance.PlayMusic(Music.Menu);
        }

        public void PlayGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void QuitGame()
        {
            Debug.Log("QUIT");
            Application.Quit();
        }
    }
}

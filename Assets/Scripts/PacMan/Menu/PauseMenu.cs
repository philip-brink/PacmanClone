using UnityEngine;
using UnityEngine.SceneManagement;

namespace PacMan.Menu
{
    public class PauseMenu : MonoBehaviour
    {
        public bool Paused { get; private set; }
        public GameObject pauseMenu;
        
        private void PauseGame()
        {
            Paused = true;
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }
        
        public void ResumeGame()
        {
            Paused = false;
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }

        private void TogglePauseGame()
        {
            if (Paused) ResumeGame();
            else PauseGame();
        }

        public void RestartGame()
        {
            var gameController = GameObject.Find("GameController").GetComponent<GameController>();
            gameController.RestartGame();
        }
        
        public void QuitGame()
        {
            Debug.Log("QUIT");
            Application.Quit();
        }

        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        
        private void Update()
        {
            var escape = Input.GetKeyDown(KeyCode.Escape);
            var q = Input.GetKeyDown(KeyCode.Q);
            var space = Input.GetKeyDown(KeyCode.Space);
            if (escape || q || space)
            {
                TogglePauseGame();
            }
        }
    }
}
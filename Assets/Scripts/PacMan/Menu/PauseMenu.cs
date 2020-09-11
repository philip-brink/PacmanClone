using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace PacMan.Menu
{
    public class PauseMenu : MonoBehaviour
    {
        public bool Paused { get; private set; }
        public GameObject pauseMenu;
        public GameObject victoryMenu;
        public GameObject failureMenu;
        public GameObject pauseFirstButton;
        public GameObject failureFirstButton;
        public GameObject victoryFirstButton;
        public GameObject player;
        
        private void PauseGame()
        {
            Paused = true;
            Time.timeScale = 0f;
            player.SetActive(false);
            pauseMenu.SetActive(true);
            
            // clear selected
            EventSystem.current.SetSelectedGameObject(null);
            // Set new selected button
            EventSystem.current.SetSelectedGameObject(pauseFirstButton);
        }
        
        public void ResumeGame()
        {
            Paused = false;
            Time.timeScale = 1f;
            player.SetActive(true);
            pauseMenu.SetActive(false);
        }

        private void TogglePauseGame()
        {
            if (Paused) ResumeGame();
            else PauseGame();
        }

        public void OpenVictoryMenu()
        {
            Time.timeScale = 0f;
            player.SetActive(false);
            victoryMenu.SetActive(true);
            
            // clear selected
            EventSystem.current.SetSelectedGameObject(null);
            // Set new selected button
            EventSystem.current.SetSelectedGameObject(victoryFirstButton);
        }
        
        private void CloseVictoryMenu()
        {
            Time.timeScale = 1f;
            player.SetActive(true);
            victoryMenu.SetActive(false);
        }

        public void OpenFailureMenu()
        {
            Time.timeScale = 0f;
            player.SetActive(false);
            failureMenu.SetActive(true);
            
            // clear selected
            EventSystem.current.SetSelectedGameObject(null);
            // Set new selected button
            EventSystem.current.SetSelectedGameObject(failureFirstButton);
        }
        
        private void CloseFailureMenu()
        {
            Time.timeScale = 1f;
            player.SetActive(true);
            failureMenu.SetActive(false);
        }

        public void RestartGame()
        {
            if (failureMenu.activeInHierarchy) CloseFailureMenu();
            if (victoryMenu.activeInHierarchy) CloseVictoryMenu();
            
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
namespace PacMan.GameStates
{
    public class Playing : IState
    {
        private readonly GameController _gameController;

        public Playing(GameController gameController)
        {
            _gameController = gameController;
        }
        
        public void Tick()
        {
        }


        public void OnEnter()
        {
            if (!AudioController.Instance.PlayingInGameMusic) AudioController.Instance.PlayMusic(Music.InGame);
            _gameController.ResetGame = false;
        }

        public void OnExit()
        {
        }
    }
}
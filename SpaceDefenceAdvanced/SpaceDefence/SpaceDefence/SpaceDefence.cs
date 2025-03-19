using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceDefence
{
    public class SpaceDefence : Game
    {
        private SpriteBatch _spriteBatch;
        private GraphicsDeviceManager _graphics;
        private GameManager _gameManager;
        private GameState _gameState;
        private SpriteFont _font;
        private bool _isGameOver;

        public SpaceDefence()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;

            // Set the size of the screen
            _graphics.PreferredBackBufferWidth = 1800;
            _graphics.PreferredBackBufferHeight = 900;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Initialize the GameManager
            _gameManager = GameManager.GetGameManager();
            _gameState = GameState.StartScreen;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Text");
            _gameManager.Load(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState mouseState = Mouse.GetState();

            switch (_gameState)
            {
                case GameState.StartScreen:
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        string startText = "Start Game";
                        string quitText = "Quit Game";

                        Vector2 startTextSize = _font.MeasureString(startText);
                        Vector2 quitTextSize = _font.MeasureString(quitText);

                        Vector2 startTextPosition = new Vector2(
                            (GraphicsDevice.Viewport.Width - startTextSize.X) / 2,
                            (GraphicsDevice.Viewport.Height - startTextSize.Y) / 2 - 20
                        );

                        Vector2 quitTextPosition = new Vector2(
                            (GraphicsDevice.Viewport.Width - quitTextSize.X) / 2,
                            (GraphicsDevice.Viewport.Height - quitTextSize.Y) / 2 + 20
                        );

                        Rectangle startTextArea = new Rectangle(startTextPosition.ToPoint(), startTextSize.ToPoint());
                        Rectangle quitTextArea = new Rectangle(quitTextPosition.ToPoint(), quitTextSize.ToPoint());

                        if (IsMouseOver(startTextArea))
                        {
                            StartGame();
                        }
                        else if (IsMouseOver(quitTextArea))
                        {
                            Exit();
                        }
                    }
                    break;
                case GameState.Playing:
                    _gameManager.Update(gameTime);
                    break;
                case GameState.GameOver:
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        _gameState = GameState.StartScreen;
                        _isGameOver = false;
                    }
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            switch (_gameState)
            {
                case GameState.StartScreen:
                    DrawStartScreen();
                    break;
                case GameState.Playing:
                    _gameManager.Draw(gameTime, _spriteBatch);
                    break;
                case GameState.GameOver:
                    DrawGameOverScreen();
                    break;
            }

            base.Draw(gameTime);
        }

        private void DrawStartScreen()
        {
            _spriteBatch.Begin();
            string titleText = "Space Defence Advance DX Game of the Year Edition";
            string startText = "Start Game";
            string quitText = "Quit Game";

            Vector2 titleTextSize = _font.MeasureString(titleText);
            Vector2 startTextSize = _font.MeasureString(startText);
            Vector2 quitTextSize = _font.MeasureString(quitText);

            Vector2 titleTextPosition = new Vector2(
                (GraphicsDevice.Viewport.Width - titleTextSize.X) / 2,
                (GraphicsDevice.Viewport.Height - titleTextSize.Y) / 2 - 100
            );

            Vector2 startTextPosition = new Vector2(
                (GraphicsDevice.Viewport.Width - startTextSize.X) / 2,
                (GraphicsDevice.Viewport.Height - startTextSize.Y) / 2 - 20
            );

            Vector2 quitTextPosition = new Vector2(
                (GraphicsDevice.Viewport.Width - quitTextSize.X) / 2,
                (GraphicsDevice.Viewport.Height - quitTextSize.Y) / 2 + 20
            );

            Rectangle startTextArea = new Rectangle(startTextPosition.ToPoint(), startTextSize.ToPoint());
            Rectangle quitTextArea = new Rectangle(quitTextPosition.ToPoint(), quitTextSize.ToPoint());

            Color startTextColor = IsMouseOver(startTextArea) ? Color.Yellow : Color.White;
            Color quitTextColor = IsMouseOver(quitTextArea) ? Color.Yellow : Color.White;

            _spriteBatch.DrawString(_font, titleText, titleTextPosition, Color.White);
            _spriteBatch.DrawString(_font, startText, startTextPosition, startTextColor);
            _spriteBatch.DrawString(_font, quitText, quitTextPosition, quitTextColor);
            _spriteBatch.End();
        }

        private void DrawGameOverScreen()
        {
            _spriteBatch.Begin();
            string gameOverText = "Game Over";
            string restartText = "Click to return to the Start Screen";

            Vector2 gameOverTextSize = _font.MeasureString(gameOverText);
            Vector2 restartTextSize = _font.MeasureString(restartText);

            Vector2 gameOverTextPosition = new Vector2(
                (GraphicsDevice.Viewport.Width - gameOverTextSize.X) / 2,
                (GraphicsDevice.Viewport.Height - gameOverTextSize.Y) / 2 - 20
            );

            Vector2 restartTextPosition = new Vector2(
                (GraphicsDevice.Viewport.Width - restartTextSize.X) / 2,
                (GraphicsDevice.Viewport.Height - restartTextSize.Y) / 2 + 20
            );

            _spriteBatch.DrawString(_font, gameOverText, gameOverTextPosition, Color.Red);
            _spriteBatch.DrawString(_font, restartText, restartTextPosition, Color.White);
            _spriteBatch.End();
        }

        private void StartGame()
        {
            // Reset the game state
            _gameManager.Reset();

            // Place the player at the center of the screen
            Ship player = new Ship(new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2));

            // Add the starting objects to the GameManager
            _gameManager.Initialize(Content, this, player);
            _gameManager.AddGameObject(player);
            _gameManager.AddGameObject(new Alien());
            _gameManager.AddGameObject(new Supply());

            _gameState = GameState.Playing;
        }

        private bool IsMouseOver(Rectangle area)
        {
            MouseState mouseState = Mouse.GetState();
            return area.Contains(mouseState.X, mouseState.Y);
        }

        public void SetGameOver()
        {
            _gameState = GameState.GameOver;
            _isGameOver = true;
        }
    }
}


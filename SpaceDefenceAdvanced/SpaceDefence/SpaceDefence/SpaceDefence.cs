﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SpaceDefence
{
    public class SpaceDefence : Game
    {
        private SpriteBatch _spriteBatch;
        private GraphicsDeviceManager _graphics;
        private GameManager _gameManager;
        private GameState _gameState;
        private SpriteFont _font;
        private SpriteFont _smallTextFont;
        private bool _isGameOver;
        private StartScreen _startScreen;
        private GameOverScreen _gameOverScreen;
        private PauseScreen _pauseScreen;
        private Texture2D _mainMenuBackground;
        private Texture2D _playAreaBackground;
        private Camera _camera;

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
            _smallTextFont = Content.Load<SpriteFont>("smallText");
            _mainMenuBackground = Content.Load<Texture2D>("MainMenu_background");
            _playAreaBackground = Content.Load<Texture2D>("stars_texture");
            _gameManager.Load(Content);

            _startScreen = new StartScreen(_font, GraphicsDevice, _mainMenuBackground);
            _gameOverScreen = new GameOverScreen(_font, GraphicsDevice);
            _pauseScreen = new PauseScreen(_font, GraphicsDevice);
            _camera = new Camera(GraphicsDevice.Viewport, _gameManager.PlayArea);
        }

        protected override void Update(GameTime gameTime)
        {
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
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        _gameState = GameState.Paused;
                    }
                    _gameManager.Update(gameTime);
                    _camera.Follow(_gameManager.Player.GetPosition().Center.ToVector2());
                    break;
                case GameState.Paused:
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        string continueText = "Continue";
                        string quitText = "Quit Game";

                        Vector2 continueTextSize = _font.MeasureString(continueText);
                        Vector2 quitTextSize = _font.MeasureString(quitText);

                        Vector2 continueTextPosition = new Vector2(
                            (GraphicsDevice.Viewport.Width - continueTextSize.X) / 2,
                            (GraphicsDevice.Viewport.Height - continueTextSize.Y) / 2 - 20
                        );

                        Vector2 quitTextPosition = new Vector2(
                            (GraphicsDevice.Viewport.Width - quitTextSize.X) / 2,
                            (GraphicsDevice.Viewport.Height - quitTextSize.Y) / 2 + 20
                        );

                        Rectangle continueTextArea = new Rectangle(continueTextPosition.ToPoint(), continueTextSize.ToPoint());
                        Rectangle quitTextArea = new Rectangle(quitTextPosition.ToPoint(), quitTextSize.ToPoint());

                        if (IsMouseOver(continueTextArea))
                        {
                            _gameState = GameState.Playing;
                        }
                        else if (IsMouseOver(quitTextArea))
                        {
                            Exit();
                        }
                    }
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
                    _startScreen.Draw(_spriteBatch);
                    break;
                case GameState.Playing:
                    _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
                    DrawBackground();
                    _gameManager.Draw(gameTime, _spriteBatch);
                    _spriteBatch.End();

                    _spriteBatch.Begin();
                    DrawTimer(gameTime);
                    DrawScore();
                    DrawObjective();
                    _spriteBatch.End();
                    break;
                case GameState.Paused:
                    _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
                    DrawBackground();
                    _gameManager.Draw(gameTime, _spriteBatch);
                    _spriteBatch.End();
                    _pauseScreen.Draw(_spriteBatch);
                    break;
                case GameState.GameOver:
                    _gameOverScreen.Draw(_spriteBatch);
                    break;
            }

            base.Draw(gameTime);
        }

        private bool IsMouseOver(Rectangle area)
        {
            MouseState mouseState = Mouse.GetState();
            return area.Contains(mouseState.X, mouseState.Y);
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
            _gameManager.AddGameObject(new Asteroid());

            _gameState = GameState.Playing;
        }

        public void SetGameOver()
        {
            _gameState = GameState.GameOver;
            _isGameOver = true;
        }

        private void DrawTimer(GameTime gameTime)
        {
            string timerText = FormatElapsedTime(_gameManager.ElapsedTime);
            Vector2 timerPosition = new Vector2(GraphicsDevice.Viewport.Width - 10, 10);
            Vector2 timerSize = _smallTextFont.MeasureString(timerText);
            timerPosition.X -= timerSize.X;
            _spriteBatch.DrawString(_smallTextFont, timerText, timerPosition, Color.White);
        }

        private void DrawScore()
        {
            string scoreText = $"Score: {_gameManager.Score}";
            Vector2 scorePosition = new Vector2(GraphicsDevice.Viewport.Width - 10, 30);
            Vector2 scoreSize = _smallTextFont.MeasureString(scoreText);
            scorePosition.X -= scoreSize.X;
            _spriteBatch.DrawString(_smallTextFont, scoreText, scorePosition, Color.White);
        }

        private void DrawObjective()
        {
            string objectiveText = $"Objective: {_gameManager.CurrentObjective.Description}";
            Vector2 objectivePosition = new Vector2(10, 10);
            _spriteBatch.DrawString(_smallTextFont, objectiveText, objectivePosition, Color.White);

            if (_gameManager.CurrentObjective is CollectCargoObjective collectCargoObjective && collectCargoObjective._cargoCollected)
            {
                Vector2 position = new Vector2(10, 50);
                _spriteBatch.Draw(collectCargoObjective.CargoIndicator, position, Color.White);
            }
        }

        private string FormatElapsedTime(float elapsedTime)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime);
            return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }

        private void DrawBackground()
        {
            int textureWidth = _playAreaBackground.Width;
            int textureHeight = _playAreaBackground.Height;

            for (int x = 0; x < _gameManager.PlayArea.Width; x += textureWidth)
            {
                for (int y = 0; y < _gameManager.PlayArea.Height; y += textureHeight)
                {
                    _spriteBatch.Draw(_playAreaBackground, new Rectangle(x, y, textureWidth, textureHeight), Color.White);
                }
            }
        }
    }
}

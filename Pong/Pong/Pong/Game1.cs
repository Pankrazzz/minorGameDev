using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Pong
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _ball, _paddleHorizontal, _paddleVertical, _border;
        private SpriteFont _font;
        private Vector2 ballPosition, ballVelocity;
        private Vector2[] paddlePositions;
        private int[] scores;
        private bool[] isPlayerControlled;
        private const int PaddleHorizontalWidth = 100, PaddleHorizontalHeight = 20, PaddleVerticalWidth = 20, PaddleVerticalHeight = 100, BallSize = 10;
        private const int BorderSize = 700, BorderThickness = 5, PaddleMargin = 50;
        private Random random;
        private float paddleSpeed = 7f;
        private Color[] paddleColors;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.ApplyChanges();

            random = new Random();
            ballPosition = new Vector2(600, 450);
            float speed = 3f;
            float angle = (float)(random.NextDouble() * Math.PI * 2);
            ballVelocity = new Vector2((float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);

            paddlePositions = new Vector2[4]
            {
                // Paddle positions
                new Vector2(600 - PaddleHorizontalWidth / 2, 800 - PaddleMargin - PaddleHorizontalHeight), // Bottom
                new Vector2(600 - PaddleHorizontalWidth / 2, 100 + PaddleMargin), // Top
                new Vector2(250 + PaddleMargin, 450 - PaddleVerticalHeight / 2), // Left
                new Vector2(950 - PaddleMargin - PaddleVerticalWidth, 450 - PaddleVerticalHeight / 2)  // Right
            };

            scores = new int[4] { 5, 5, 5, 5 };
            isPlayerControlled = new bool[4];
            paddleColors = new Color[] { Color.Red, Color.Blue, Color.Green, Color.Yellow };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _ball = new Texture2D(GraphicsDevice, BallSize, BallSize);
            _paddleHorizontal = new Texture2D(GraphicsDevice, PaddleHorizontalWidth, PaddleHorizontalHeight);
            _paddleVertical = new Texture2D(GraphicsDevice, PaddleVerticalWidth, PaddleVerticalHeight);
            _border = new Texture2D(GraphicsDevice, 1, 1);
            _border.SetData(new Color[] { Color.White });
            _font = Content.Load<SpriteFont>("ScoreFont");

            Color[] ballData = new Color[BallSize * BallSize];
            Color[] paddleHorizontalData = new Color[PaddleHorizontalWidth * PaddleHorizontalHeight];
            Color[] paddleVerticalData = new Color[PaddleVerticalWidth * PaddleVerticalHeight];

            for (int i = 0; i < ballData.Length; i++) ballData[i] = Color.White;
            for (int i = 0; i < paddleHorizontalData.Length; i++) paddleHorizontalData[i] = Color.White;
            for (int i = 0; i < paddleVerticalData.Length; i++) paddleVerticalData[i] = Color.White;

            _ball.SetData(ballData);
            _paddleHorizontal.SetData(paddleHorizontalData);
            _paddleVertical.SetData(paddleVerticalData);
        }

        protected override void Update(GameTime gameTime)
        {
            ballPosition += ballVelocity;
            KeyboardState keyboardState = Keyboard.GetState();

            // Player controls
            if (keyboardState.IsKeyDown(Keys.Left)) { paddlePositions[0].X = Math.Max(250, paddlePositions[0].X - paddleSpeed); isPlayerControlled[0] = true; }
            if (keyboardState.IsKeyDown(Keys.Right)) { paddlePositions[0].X = Math.Min(950 - PaddleHorizontalWidth, paddlePositions[0].X + paddleSpeed); isPlayerControlled[0] = true; }
            if (keyboardState.IsKeyDown(Keys.A)) { paddlePositions[1].X = Math.Max(250, paddlePositions[1].X - paddleSpeed); isPlayerControlled[1] = true; }
            if (keyboardState.IsKeyDown(Keys.D)) { paddlePositions[1].X = Math.Min(950 - PaddleHorizontalWidth, paddlePositions[1].X + paddleSpeed); isPlayerControlled[1] = true; }
            if (keyboardState.IsKeyDown(Keys.W)) { paddlePositions[2].Y = Math.Max(100, paddlePositions[2].Y - paddleSpeed); isPlayerControlled[2] = true; }
            if (keyboardState.IsKeyDown(Keys.S)) { paddlePositions[2].Y = Math.Min(800 - PaddleVerticalHeight, paddlePositions[2].Y + paddleSpeed); isPlayerControlled[2] = true; }
            if (keyboardState.IsKeyDown(Keys.Up)) { paddlePositions[3].Y = Math.Max(100, paddlePositions[3].Y - paddleSpeed); isPlayerControlled[3] = true; }
            if (keyboardState.IsKeyDown(Keys.Down)) { paddlePositions[3].Y = Math.Min(800 - PaddleVerticalHeight, paddlePositions[3].Y + paddleSpeed); isPlayerControlled[3] = true; }

            // AI controls
            for (int i = 0; i < 4; i++)
            {
                if (!isPlayerControlled[i] && scores[i] > 0) // Only control paddles with positive scores
                {
                    // Horizontal paddles
                    if (i == 0 || i == 1)
                    {
                        if (ballPosition.X < paddlePositions[i].X)
                            paddlePositions[i].X -= paddleSpeed;
                        else if (ballPosition.X > paddlePositions[i].X + PaddleHorizontalWidth)
                            paddlePositions[i].X += paddleSpeed;
                    }
                    // Vertical paddles
                    else
                    {
                        if (ballPosition.Y < paddlePositions[i].Y)
                            paddlePositions[i].Y -= paddleSpeed;
                        else if (ballPosition.Y > paddlePositions[i].Y + PaddleVerticalHeight)
                            paddlePositions[i].Y += paddleSpeed;
                    }
                }
            }

            // Check collision with paddles and adjust angle
            for (int i = 0; i < 4; i++)
            {
                if (scores[i] > 0) // Only check collision with paddles that are still in the game
                {
                    // Horizontal paddles
                    if (i == 0 || i == 1)
                    {
                        if (ballPosition.Y + BallSize >= paddlePositions[i].Y && ballPosition.Y <= paddlePositions[i].Y + PaddleHorizontalHeight &&
                            ballPosition.X + BallSize >= paddlePositions[i].X && ballPosition.X <= paddlePositions[i].X + PaddleHorizontalWidth)
                        {
                            float hitPosition = (ballPosition.X - paddlePositions[i].X) / PaddleHorizontalWidth - 0.5f;
                            ballVelocity = new Vector2(hitPosition * 5, -ballVelocity.Y).SafeNormalize() * ballVelocity.Length() * 1.1f;
                        }
                    }
                    // Vertical paddles
                    else
                    {
                        if (ballPosition.X + BallSize >= paddlePositions[i].X && ballPosition.X <= paddlePositions[i].X + PaddleVerticalWidth &&
                            ballPosition.Y + BallSize >= paddlePositions[i].Y && ballPosition.Y <= paddlePositions[i].Y + PaddleVerticalHeight)
                        {
                            float hitPosition = (ballPosition.Y - paddlePositions[i].Y) / PaddleVerticalHeight - 0.5f;
                            ballVelocity = new Vector2(-ballVelocity.X, hitPosition * 5).SafeNormalize() * ballVelocity.Length() * 1.1f;
                        }
                    }
                }
            }

            // Check collision with walls and update scores
            if (ballPosition.Y <= 100) // Top wall
            {
                if (scores[1] > 0)
                {
                    scores[1]--;
                    ResetBall();
                }
                else
                {
                    ballVelocity.Y = Math.Abs(ballVelocity.Y); // Bounce off the wall if the player is eliminated
                }
            }
            if (ballPosition.Y + BallSize >= 800) // Bottom wall
            {
                if (scores[0] > 0)
                {
                    scores[0]--;
                    ResetBall();
                }
                else
                {
                    ballVelocity.Y = -Math.Abs(ballVelocity.Y); // Bounce off the wall if the player is eliminated
                }
            }
            if (ballPosition.X <= 250) // Left wall
            {
                if (scores[2] > 0)
                {
                    scores[2]--;
                    ResetBall();
                }
                else
                {
                    ballVelocity.X = Math.Abs(ballVelocity.X); // Bounce off the wall if the player is eliminated
                }
            }
            if (ballPosition.X + BallSize >= 950) // Right wall
            {
                if (scores[3] > 0)
                {
                    scores[3]--;
                    ResetBall();
                }
                else
                {
                    ballVelocity.X = -Math.Abs(ballVelocity.X); // Bounce off the wall if the player is eliminated
                }
            }

            base.Update(gameTime);
        }

        private void ResetBall()
        {
            ballPosition = new Vector2(600, 450);
            float speed = 3f;
            float angle = (float)(random.NextDouble() * Math.PI * 2);
            ballVelocity = new Vector2((float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Draw white square border
            _spriteBatch.Draw(_border, new Rectangle(250, 100, BorderSize, BorderThickness), Color.White); // Top
            _spriteBatch.Draw(_border, new Rectangle(250, 800, BorderSize, BorderThickness), Color.White); // Bottom
            _spriteBatch.Draw(_border, new Rectangle(250, 100, BorderThickness, BorderSize), Color.White); // Left
            _spriteBatch.Draw(_border, new Rectangle(950, 100, BorderThickness, BorderSize), Color.White); // Right

            // Draw the ball
            _spriteBatch.Draw(_ball, ballPosition, Color.White);

            // Draw scores on each side
            _spriteBatch.DrawString(_font, scores[0].ToString(), new Vector2(600, 825), paddleColors[0]); // Bottom
            _spriteBatch.DrawString(_font, scores[1].ToString(), new Vector2(600, 25), paddleColors[1]);  // Top
            _spriteBatch.DrawString(_font, scores[2].ToString(), new Vector2(200, 450), paddleColors[2]); // Left
            _spriteBatch.DrawString(_font, scores[3].ToString(), new Vector2(1000, 450), paddleColors[3]); // Right


            // Draw paddles only if their score is greater than zero
            // Draw paddles only if their score is greater than zero
            for (int i = 0; i < 4; i++)
            {
                if (scores[i] > 0)
                {
                    // Draw the paddle with its corresponding color
                    if (i < 2) // Horizontal paddles (bottom and top)
                    {
                        _spriteBatch.Draw(_paddleHorizontal, paddlePositions[i], paddleColors[i]);
                    }
                    else // Vertical paddles (left and right)
                    {
                        _spriteBatch.Draw(_paddleVertical, paddlePositions[i], paddleColors[i]);
                    }
                }
            }

            _spriteBatch.Draw(_ball, ballPosition, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

public static class Vector2Extensions
{
    public static Vector2 SafeNormalize(this Vector2 vector)
    {
        if (vector == Vector2.Zero) return Vector2.Zero;
        return Vector2.Normalize(vector);
    }
}

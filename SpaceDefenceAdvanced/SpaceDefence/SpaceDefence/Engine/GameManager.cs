using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceDefence
{
    public class GameManager
    {
        private static GameManager gameManager;

        private List<GameObject> _gameObjects;
        private List<GameObject> _toBeRemoved;
        private List<GameObject> _toBeAdded;
        private ContentManager _content;
        private float _asteroidRespawnTimer;
        private List<float> _alienSpawnTimes;
        private float _elapsedTime;
        private int _nextAlienIndex;
        private Rectangle _playArea;
        private float _bombPowerUpSpawnTimer;

        public Random RNG { get; private set; }
        public Ship Player { get; private set; }
        public InputManager InputManager { get; private set; }
        public Game Game { get; private set; }
        public Rectangle PlayArea => _playArea;
        public Camera Camera { get; private set; }
        public float ElapsedTime => _elapsedTime;

        public static GameManager GetGameManager()
        {
            if (gameManager == null)
                gameManager = new GameManager();
            return gameManager;
        }

        public GameManager()
        {
            _gameObjects = new List<GameObject>();
            _toBeRemoved = new List<GameObject>();
            _toBeAdded = new List<GameObject>();
            _asteroidRespawnTimer = 0;
            _alienSpawnTimes = new List<float> { 60, 180, 300, 600 }; // Spawn times in seconds
            _elapsedTime = 0;
            _nextAlienIndex = 0;
            _bombPowerUpSpawnTimer = 5f;
            InputManager = new InputManager();
            RNG = new Random();
            _playArea = new Rectangle(0, 0, 3600, 1800);
        }

        public void Initialize(ContentManager content, Game game, Ship player)
        {
            Game = game;
            _content = content;
            Player = player;
            Camera = new Camera(game.GraphicsDevice.Viewport, _playArea);

            var asteroid = new Asteroid();
            AddGameObject(asteroid);
            _asteroidRespawnTimer = 0; // No respawn timer at start
        }

        public void Load(ContentManager content)
        {
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.Load(content);
            }
        }

        public void HandleInput(InputManager inputManager)
        {
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.HandleInput(this.InputManager);
            }
        }

        public void CheckCollision()
        {
            // Checks once for every pair of 2 GameObjects if they collide.
            for (int i = 0; i < _gameObjects.Count; i++)
            {
                for (int j = i + 1; j < _gameObjects.Count; j++)
                {
                    if (_gameObjects[i].CheckCollision(_gameObjects[j]))
                    {
                        _gameObjects[i].OnCollision(_gameObjects[j]);
                        _gameObjects[j].OnCollision(_gameObjects[i]);
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            InputManager.Update();

            // Handle input
            HandleInput(InputManager);

            // Update
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.Update(gameTime);
            }

            // Check Collision
            CheckCollision();

            foreach (GameObject gameObject in _toBeAdded)
            {
                gameObject.Load(_content);
                _gameObjects.Add(gameObject);
            }
            _toBeAdded.Clear();

            foreach (GameObject gameObject in _toBeRemoved)
            {
                gameObject.Destroy();
                _gameObjects.Remove(gameObject);
            }
            _toBeRemoved.Clear();

            // Handle asteroid respawn timer
            if (_asteroidRespawnTimer > 0)
            {
                _asteroidRespawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_asteroidRespawnTimer <= 0)
                {
                    var newAsteroid = new Asteroid();
                    AddGameObject(newAsteroid);
                }
            }

            // Update elapsed time
            _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Spawn new aliens based on elapsed time
            if (_nextAlienIndex < _alienSpawnTimes.Count && _elapsedTime >= _alienSpawnTimes[_nextAlienIndex])
            {
                AddGameObject(new Alien());
                _nextAlienIndex++;
            }

            _bombPowerUpSpawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_bombPowerUpSpawnTimer <= 0)
            {
                ScheduleBombPowerUpSpawn();
                _bombPowerUpSpawnTimer = 30f; // Reset timer for next spawn
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.Draw(gameTime, spriteBatch);
            }
        }

        /// <summary>
        /// Add a new GameObject to the GameManager. 
        /// The GameObject will be added at the start of the next Update step. 
        /// Once it is added, the GameManager will ensure all steps of the game loop will be called on the object automatically. 
        /// </summary>
        /// <param name="gameObject"> The GameObject to add. </param>
        public void AddGameObject(GameObject gameObject)
        {
            _toBeAdded.Add(gameObject);
        }

        /// <summary>
        /// Remove GameObject from the GameManager. 
        /// The GameObject will be removed at the start of the next Update step and its Destroy() method will be called.
        /// After that the object will no longer receive any updates.
        /// </summary>
        /// <param name="gameObject"> The GameObject to Remove. </param>
        public void RemoveGameObject(GameObject gameObject)
        {
            _toBeRemoved.Add(gameObject);
        }

        /// <summary>
        /// Get a random location in the play area.
        /// </summary>
        public Vector2 RandomScreenLocation()
        {
            return new Vector2(
                RNG.Next(_playArea.Left, _playArea.Right),
                RNG.Next(_playArea.Top, _playArea.Bottom));
        }

        public void Reset()
        {
            _gameObjects.Clear();
            _toBeRemoved.Clear();
            _toBeAdded.Clear();
            _asteroidRespawnTimer = 0;
            _elapsedTime = 0;
            _nextAlienIndex = 0;
        }

        public void ScheduleAsteroidSpawn()
        {
            _asteroidRespawnTimer = RNG.Next(5, 21); // Random time between 5 and 20 seconds
        }

        public void ScheduleBombPowerUpSpawn()
        {
            if (!IsBombPowerUpActive())
            {
                AddGameObject(new Supply(true));
            }
        }

        public bool IsBombPowerUpActive()
        {
            return GetGameObjects().Any(go => go is Supply supply && supply.isBombPowerUp);
        }

        public List<GameObject> GetGameObjects()
        {
            return _gameObjects;
        }

    }
}

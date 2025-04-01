using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
        private float _powerUpSpawnTimer = 20f;
        private Objective _currentObjective;
        private int _score;

        public Random RNG { get; private set; }
        public Ship Player { get; private set; }
        public InputManager InputManager { get; private set; }
        public Game Game { get; private set; }
        public Rectangle PlayArea => _playArea;
        public Camera Camera { get; private set; }
        public float ElapsedTime => _elapsedTime;
        public int Score => _score;
        public Objective CurrentObjective => _currentObjective;

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
            _alienSpawnTimes = new List<float> { 60, 180, 300, 600 };
            _elapsedTime = 0;
            _nextAlienIndex = 0;
            _bombPowerUpSpawnTimer = 5f;
            InputManager = new InputManager();
            RNG = new Random();
            _playArea = new Rectangle(0, 0, 3600, 1800);
            _score = 0;
        }

        public void Initialize(ContentManager content, Game game, Ship player)
        {
            Game = game;
            _content = content;
            Player = player;
            Camera = new Camera(game.GraphicsDevice.Viewport, _playArea);

            var asteroid = new Asteroid();
            AddGameObject(asteroid);
            // No respawn timer at start
            _asteroidRespawnTimer = 0;

            var alienPlanet = new Planet(new Vector2(150, 200), "Alien planet", 96, 96, 77, 0.05f, true, 3.0f, "Alien Planet");
            var earthLikePlanet = new Planet(new Vector2(_playArea.Width - 300, _playArea.Height - 200), "Earth-Like planet", 96, 96, 77, 0.1f, true, 2.0f, "Earth-Like Planet");

            // Add the planets to the game world
            AddGameObject(alienPlanet);
            AddGameObject(earthLikePlanet);

            // Generate the first objective (will use these planets when creating CollectCargoObjective)
            GenerateNewObjective(alienPlanet, earthLikePlanet);
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
            for (int i = 0; i < _gameObjects.Count; i++)
            {
                for (int j = i + 1; j < _gameObjects.Count; j++)
                {
                    if (_gameObjects[i].CheckCollision(_gameObjects[j]))
                    {
                        // System.Diagnostics.Debug.WriteLine($"Collision detected between {_gameObjects[i].GetType().Name} and {_gameObjects[j].GetType().Name}");
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

            if (_asteroidRespawnTimer > 0)
            {
                _asteroidRespawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_asteroidRespawnTimer <= 0)
                {
                    var newAsteroid = new Asteroid();
                    AddGameObject(newAsteroid);
                }
            }

            _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_nextAlienIndex < _alienSpawnTimes.Count && _elapsedTime >= _alienSpawnTimes[_nextAlienIndex])
            {
                AddGameObject(new Alien());
                _nextAlienIndex++;
            }

            _powerUpSpawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_powerUpSpawnTimer <= 0)
            {
                AddGameObject(new Supply(false));
                _powerUpSpawnTimer = 30f;
            }

            _bombPowerUpSpawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_bombPowerUpSpawnTimer <= 0)
            {
                ScheduleBombPowerUpSpawn();
                _bombPowerUpSpawnTimer = 30f;
            }

            // Update the current objective
            _currentObjective?.Update(this);
            if (_currentObjective?.IsCompleted == true)
            {
                _score++;
                GenerateNewObjective();
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.Draw(gameTime, spriteBatch);
            }

            // Draw the current objective
            _currentObjective?.Draw(spriteBatch, _content.Load<SpriteFont>("smallText"));
        }

        public void AddGameObject(GameObject gameObject)
        {
            _toBeAdded.Add(gameObject);
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            _toBeRemoved.Add(gameObject);
        }

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
            _score = 0;
            GenerateNewObjective();
        }

        public void ScheduleAsteroidSpawn()
        {
            _asteroidRespawnTimer = RNG.Next(5, 21);
        }

        public void ScheduleBombPowerUpSpawn()
        {
            if (!Player.HasBombPowerUp && !IsBombPowerUpActive())
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

        private void GenerateNewObjective(Planet alienPlanet = null, Planet earthLikePlanet = null)
        {
            int objectiveType = RNG.Next(3);
            switch (objectiveType)
            {
                case 0:
                    // Use the provided planets or create new ones if not provided
                    var sourcePlanet = alienPlanet ?? new Planet(new Vector2(150, 200), "Alien planet", 96, 96, 77, 0.05f, true, 3.0f, "Alien Planet");
                    var destPlanet = earthLikePlanet ?? new Planet(new Vector2(_playArea.Width - 300, _playArea.Height - 200), "Earth-Like planet", 96, 96, 77, 0.1f, true, 2.0f, "Earth-Like Planet");

                    _currentObjective = new CollectCargoObjective(
                        sourcePlanet,
                        destPlanet,
                        _content
                    );
                    break;
                case 1:
                    _currentObjective = new KillAliensObjective(RNG.Next(3, 6));
                    break;
                case 2:
                    _currentObjective = new DestroyAsteroidsObjective(RNG.Next(1, 4));
                    break;
            }
        }

        public void AlienKilled()
        {
            if (_currentObjective is KillAliensObjective killAliensObjective)
            {
                killAliensObjective.AlienKilled();
            }
        }

        public void AsteroidDestroyed()
        {
            if (_currentObjective is DestroyAsteroidsObjective destroyAsteroidsObjective)
            {
                destroyAsteroidsObjective.AsteroidDestroyed();
            }
        }
    }
}

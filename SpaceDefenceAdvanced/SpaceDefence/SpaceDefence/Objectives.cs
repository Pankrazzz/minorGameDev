using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpaceDefence
{
    public abstract class Objective
    {
        public string Description { get; protected set; }
        public bool IsCompleted { get; set; }

        public abstract void Update(GameManager gameManager);
        public abstract void Draw(SpriteBatch spriteBatch, SpriteFont font);
    }

    public class CollectCargoObjective : Objective
    {
        private Planet _sourcePlanet;
        private Planet _destinationPlanet;
        public bool _cargoCollected;
        private Texture2D _cargoIndicator;

        public Texture2D CargoIndicator => _cargoIndicator;

        public CollectCargoObjective(Planet sourcePlanet, Planet destinationPlanet, ContentManager content)
        {
            _sourcePlanet = sourcePlanet;
            _destinationPlanet = destinationPlanet;
            Description = $"Collect cargo from {_sourcePlanet.Name} and deliver it to {_destinationPlanet.Name}";
            _cargoCollected = false;
            _cargoIndicator = content.Load<Texture2D>("cargo");
        }

        public override void Update(GameManager gameManager)
        {
            if (!_cargoCollected &&
                gameManager.Player.GetCollider().CheckIntersection(_sourcePlanet.GetCollider()))
            {
                _cargoCollected = true;
                gameManager.Player.HasCargo = true;
                Description = $"Deliver cargo to {_destinationPlanet.Name}";
            }
            else if (_cargoCollected &&
                     gameManager.Player.GetCollider().CheckIntersection(_destinationPlanet.GetCollider()))
            {
                gameManager.Player.HasCargo = false;
                IsCompleted = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            if (_cargoCollected)
            {
                Vector2 position = new Vector2(10, 50);
                // spriteBatch.Draw(_cargoIndicator, position, Color.White);
            }
        }
    }

    public class KillAliensObjective : Objective
    {
        private int _targetCount;
        private int _currentCount;

        public KillAliensObjective(int targetCount)
        {
            _targetCount = targetCount;
            Description = $"Kill {_targetCount} aliens";
            _currentCount = 0;
        }

        public void AlienKilled()
        {
            _currentCount++;
            if (_currentCount >= _targetCount)
            {
                IsCompleted = true;
            }
        }

        public override void Update(GameManager gameManager)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {

        }
    }

    public class DestroyAsteroidsObjective : Objective
    {
        private int _targetCount;
        private int _currentCount;

        public DestroyAsteroidsObjective(int targetCount)
        {
            _targetCount = targetCount;
            Description = $"Destroy {_targetCount} asteroids";
            _currentCount = 0;
        }

        public void AsteroidDestroyed()
        {
            _currentCount++;
            if (_currentCount >= _targetCount)
            {
                IsCompleted = true;
            }
        }

        public override void Update(GameManager gameManager)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {

        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpaceDefence
{
    public enum ExplosionType
    {
        Asteroid,
        Alien,
        Ship
    }

    public class Explosion : GameObject
    {
        private SpriteAnimation _animation;
        private Vector2 _position;
        private ExplosionType _explosionType;
        private float _scale;

        public Explosion(Vector2 position, ExplosionType explosionType, float scale = 1.0f)
        {
            _position = position;
            _explosionType = explosionType;
            _scale = scale;
        }

        public override void Load(ContentManager content)
        {
            Texture2D explosionTexture;
            int frameCount;
            float frameTime;

            switch (_explosionType)
            {
                case ExplosionType.Asteroid:
                    // explosionTexture = content.Load<Texture2D>("Asteroid 01 - Explode"); // Had trouble to make this one look nice, sorry
                    explosionTexture = content.Load<Texture2D>("Explosion");
                    frameCount = 16; // 16 frames (Can change depending on explosion type)
                    frameTime = 0.05f;
                    break;
                case ExplosionType.Alien:
                    explosionTexture = content.Load<Texture2D>("Explosion");
                    frameCount = 16; // 16 frames (Can change depending on explosion type)
                    frameTime = 0.05f;
                    break;
                case ExplosionType.Ship:
                    explosionTexture = content.Load<Texture2D>("Explosion");
                    frameCount = 16; // 16 frames (Can change depending on explosion type)
                    frameTime = 0.05f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _animation = new SpriteAnimation(explosionTexture, 64, 64, frameCount, frameTime, false);
        }

        public override void Update(GameTime gameTime)
        {
            _animation.Update(gameTime);
            if (_animation.IsFinished)
            {
                GameManager.GetGameManager().RemoveGameObject(this);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _animation.Draw(spriteBatch, _position, Color.White, _scale);
        }
    }
}

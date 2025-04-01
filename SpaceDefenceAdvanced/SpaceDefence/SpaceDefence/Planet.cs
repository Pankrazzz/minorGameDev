using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpaceDefence.Collision;

namespace SpaceDefence
{
    public class Planet : GameObject, ICollidable
    {
        private SpriteAnimation _animation;
        private Vector2 _position;
        private float _scale;
        private CircleCollider _collider;

        public string Name { get; private set; }

        public Planet(Vector2 position, string textureName, int frameWidth, int frameHeight, int frameCount, float frameTime, bool isLooping, float scale, string name)
        {
            _position = position;
            _animation = new SpriteAnimation(null, frameWidth, frameHeight, frameCount, frameTime, isLooping);
            _animation.TextureName = textureName;
            _scale = scale;
            Name = name;
        }

        public override void Load(ContentManager content)
        {
            _animation.Load(content);
            _collider = new CircleCollider(_position, _animation.FrameWidth / 2 * _scale);
            base.Load(content);
        }

        public override void Update(GameTime gameTime)
        {
            _animation.Update(gameTime);
            _collider.Center = _position;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _animation.Draw(spriteBatch, _position, Color.White, _scale);
            base.Draw(gameTime, spriteBatch);
        }

        public Collider GetCollider() => _collider;
    }
}

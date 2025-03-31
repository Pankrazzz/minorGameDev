using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence
{
    public class Planet : GameObject
    {
        private SpriteAnimation _animation;
        private Vector2 _position;
        private float _scale;

        public Planet(Vector2 position, string textureName, int frameWidth, int frameHeight, int frameCount, float frameTime, bool isLooping, float scale)
        {
            _position = position;
            _animation = new SpriteAnimation(null, frameWidth, frameHeight, frameCount, frameTime, isLooping);
            _animation.TextureName = textureName;
            _scale = scale;
        }

        public override void Load(ContentManager content)
        {
            _animation.Load(content);
            base.Load(content);
        }

        public override void Update(GameTime gameTime)
        {
            _animation.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _animation.Draw(spriteBatch, _position, Color.White, _scale);
            base.Draw(gameTime, spriteBatch);
        }
    }
}

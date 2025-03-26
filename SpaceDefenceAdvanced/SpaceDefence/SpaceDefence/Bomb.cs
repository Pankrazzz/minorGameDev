using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence
{
    internal class Bomb : GameObject
    {
        private CircleCollider _circleCollider;
        private Texture2D _sprite;
        private double _timer = 5.0f;

        public Bomb(Vector2 position)
        {
            _circleCollider = new CircleCollider(position, 20);
            SetCollider(_circleCollider);
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            _sprite = content.Load<Texture2D>("Bomb");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _timer -= gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer <= 0)
            {
                Explode();
                GameManager.GetGameManager().RemoveGameObject(this);
            }
        }

        private void Explode()
        {
            GameManager gm = GameManager.GetGameManager();
            gm.AddGameObject(new Explosion(_circleCollider.Center, ExplosionType.Asteroid, 2.0f));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, _circleCollider.Center, null, Color.White, 0f, new Vector2(_sprite.Width / 2, _sprite.Height / 2), 1f, SpriteEffects.None, 0f);
            base.Draw(gameTime, spriteBatch);
        }

        public CircleCollider GetCollider()
        {
            return _circleCollider;
        }
    }
}

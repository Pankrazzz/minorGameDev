using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpaceDefence.Collision;

namespace SpaceDefence
{
    public class Asteroid : GameObject, ICollidable
    {
        private CircleCollider _circleCollider;
        private Texture2D _texture;
        private float _scale;

        public Asteroid()
        {
            _scale = 2.0f;
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            _texture = content.Load<Texture2D>("Asteroid 01 - Base");
            _circleCollider = new CircleCollider(Vector2.Zero, _texture.Width / 2);
            SetCollider(_circleCollider);
            RandomMove();
        }

        public override void OnCollision(GameObject other)
        {
            if (other is Bullet)
            {
                GameManager.GetGameManager().RemoveGameObject(this);
                GameManager.GetGameManager().RemoveGameObject(other);
                GameManager.GetGameManager().AddGameObject(new Explosion(_circleCollider.Center, ExplosionType.Asteroid, _scale));
                GameManager.GetGameManager().ScheduleAsteroidSpawn();
            }
            else if (other is Laser)
            {
                GameManager.GetGameManager().RemoveGameObject(this);
                GameManager.GetGameManager().AddGameObject(new Explosion(_circleCollider.Center, ExplosionType.Asteroid, _scale));
                GameManager.GetGameManager().ScheduleAsteroidSpawn();
            }
            else if (other is Bomb)
            {
                GameManager.GetGameManager().RemoveGameObject(this);
                GameManager.GetGameManager().AddGameObject(new Explosion(_circleCollider.Center, ExplosionType.Asteroid));
            }
            else if (other is Ship)
            {
                GameManager.GetGameManager().RemoveGameObject(other);
                GameManager.GetGameManager().RemoveGameObject(this);
                GameManager.GetGameManager().AddGameObject(new Explosion(_circleCollider.Center, ExplosionType.Asteroid, _scale));
                ((SpaceDefence)GameManager.GetGameManager().Game).SetGameOver();
                GameManager.GetGameManager().ScheduleAsteroidSpawn();
            }
            base.OnCollision(other);
        }

        public void RandomMove()
        {
            GameManager gm = GameManager.GetGameManager();
            _circleCollider.Center = gm.RandomScreenLocation();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _texture,
                _circleCollider.Center,
                null,
                Color.White,
                0f,
                new Vector2(_texture.Width / 2, _texture.Height / 2),
                _scale,
                SpriteEffects.None,
                0f
            );
            base.Draw(gameTime, spriteBatch);
        }

        public Collider GetCollider() => _circleCollider;
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence
{
    internal class Alien : GameObject
    {
        private CircleCollider _circleCollider;
        private Texture2D _texture;
        private float playerClearance = 100;
        private float speed;
        private static float baseSpeed = 50f;

        public Alien(float speedMultiplier = 1f)
        {
            speed = baseSpeed * speedMultiplier;
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            _texture = content.Load<Texture2D>("Alien");
            _circleCollider = new CircleCollider(Vector2.Zero, _texture.Width / 2);
            SetCollider(_circleCollider);
            RandomMove();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            MoveTowardsPlayer(gameTime);
            // CheckGameOver();
        }

        public override void OnCollision(GameObject other)
        {
            if (other is Bullet || other is Laser)
            {
                GameManager.GetGameManager().RemoveGameObject(this);
                GameManager.GetGameManager().AddGameObject(new Alien(speed / baseSpeed + 0.1f));
            } else if (other is Ship)
            {
                //Game over
                GameManager.GetGameManager().Game.Exit();
            }
                base.OnCollision(other);
        }

        private void MoveTowardsPlayer(GameTime gameTime)
        {
            GameManager gm = GameManager.GetGameManager();
            Vector2 direction = gm.Player.GetPosition().Center.ToVector2() - _circleCollider.Center;
            direction.Normalize();
            _circleCollider.Center += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void RandomMove()
        {
            GameManager gm = GameManager.GetGameManager();
            _circleCollider.Center = gm.RandomScreenLocation();

            Vector2 centerOfPlayer = gm.Player.GetPosition().Center.ToVector2();
            while ((_circleCollider.Center - centerOfPlayer).Length() < playerClearance)
                _circleCollider.Center = gm.RandomScreenLocation();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _circleCollider.GetBoundingBox(), Color.White);
            base.Draw(gameTime, spriteBatch);
        }
    }
}

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SpaceDefence;
using Microsoft.Xna.Framework.Content;
using GameClass = SpaceDefence.SpaceDefence;

internal class Bomb : GameObject
{
    private CircleCollider _circleCollider;
    private Texture2D _sprite;
    private double _timer = 10.0f;
    private float _explosionRadius = 200f;

    public Bomb(Vector2 position)
    {
        _circleCollider = new CircleCollider(position, 20);
        SetCollider(_circleCollider);
    }

    public override void Load(ContentManager content)
    {
        _sprite = content.Load<Texture2D>("Bomb");
        base.Load(content);
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

    public override bool CheckCollision(GameObject other)
    {
        if (other is Ship)
            return false;

        return base.CheckCollision(other);
    }

    public void Explode()
    {
        GameManager gm = GameManager.GetGameManager();
        gm.AddGameObject(new Explosion(_circleCollider.Center, ExplosionType.Asteroid, 10.0f));

        var explosionArea = new CircleCollider(_circleCollider.Center, _explosionRadius);
        foreach (var obj in gm.GetGameObjects())
        {
            if (obj == this) continue;

            if (obj is ICollidable collidable)
            {
                if (explosionArea.CheckIntersection(collidable.GetCollider()))
                {
                    if (obj is Ship ship)
                    {
                        gm.RemoveGameObject(ship);
                        gm.AddGameObject(new Explosion(ship.GetPosition().Center.ToVector2(), ExplosionType.Ship));
                        var game = gm.Game as Game;
                        if (game is GameClass spaceDefenceGame)
                        {
                            spaceDefenceGame.SetGameOver();
                        }
                    }
                    else if (obj is Alien || obj is Asteroid)
                    {
                        gm.RemoveGameObject(obj);
                    }
                }
            }
        }

        gm.ScheduleBombPowerUpSpawn();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_sprite, _circleCollider.Center, null, Color.White, 0f,
            new Vector2(_sprite.Width / 2, _sprite.Height / 2), 1f, SpriteEffects.None, 0f);
    }
}
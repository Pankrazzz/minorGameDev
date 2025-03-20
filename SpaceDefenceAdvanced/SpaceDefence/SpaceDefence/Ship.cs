using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceDefence.Collision;

namespace SpaceDefence
{
    public class Ship : GameObject
    {
        private Texture2D ship_body;
        private Texture2D base_turret;
        private Texture2D laser_turret;
        private float buffTimer = 10;
        private float buffDuration = 10f;
        private RectangleCollider _rectangleCollider;
        private Point target;

        // Movement variables
        private Vector2 velocity;
        private Vector2 acceleration;
        private float accelerationSpeed = 7f;
        private float maxSpeed = 25f;
        private float rotation;
        private float decelerationFactor = 0.75f;

        /// <summary>
        /// The player character
        /// </summary>
        /// <param name="Position">The ship's starting position</param>
        public Ship(Point Position)
        {
            _rectangleCollider = new RectangleCollider(new Rectangle(Position, Point.Zero));
            SetCollider(_rectangleCollider);

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
            rotation = 0f;
        }

        public override void Load(ContentManager content)
        {
            // Ship sprites from: https://zintoki.itch.io/space-breaker
            ship_body = content.Load<Texture2D>("ship_body");
            base_turret = content.Load<Texture2D>("base_turret");
            laser_turret = content.Load<Texture2D>("laser_turret");
            _rectangleCollider.shape.Size = ship_body.Bounds.Size;
            _rectangleCollider.shape.Location -= new Point(ship_body.Width / 2, ship_body.Height / 2);
            base.Load(content);
        }

        public override void HandleInput(InputManager inputManager)
        {
            base.HandleInput(inputManager);
            target = inputManager.CurrentMouseState.Position;

            // Handle movement input
            acceleration = Vector2.Zero;
            if (inputManager.IsKeyDown(Keys.W))
                acceleration.Y -= 1;
            if (inputManager.IsKeyDown(Keys.S))
                acceleration.Y += 1;
            if (inputManager.IsKeyDown(Keys.A))
                acceleration.X -= 1;
            if (inputManager.IsKeyDown(Keys.D))
                acceleration.X += 1;

            // Normalize the acceleration vector to ensure smooth 360-degree movement
            if (acceleration != Vector2.Zero)
            {
                acceleration.Normalize();
                acceleration *= accelerationSpeed;
            }

            if (inputManager.LeftMousePress())
            {
                Vector2 aimDirection = LinePieceCollider.GetDirection(GetPosition().Center, target);
                Vector2 turretExit = _rectangleCollider.shape.Center.ToVector2() + aimDirection * base_turret.Height / 2f;
                int screenWidth = GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Width;
                if (buffTimer <= 0)
                {
                    GameManager.GetGameManager().AddGameObject(new Bullet(turretExit, aimDirection, 150));
                }
                else
                {
                    GameManager.GetGameManager().AddGameObject(new Laser(new LinePieceCollider(turretExit, target.ToVector2()), screenWidth));
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update velocity based on acceleration
            velocity += acceleration * deltaTime;

            // Limit speed to maxSpeed
            if (velocity.Length() > maxSpeed)
            {
                velocity.Normalize();
                velocity *= maxSpeed;
            }

            // Apply gradual deceleration when no acceleration is being applied
            if (acceleration == Vector2.Zero)
            {
                velocity *= (float)Math.Pow(decelerationFactor, deltaTime); // Gradually reduce velocity
            }

            // Update position based on velocity
            _rectangleCollider.shape.Location += velocity.ToPoint();

            // Update rotation based on velocity direction
            if (velocity != Vector2.Zero)
            {
                rotation = (float)Math.Atan2(velocity.Y, velocity.X) + MathHelper.PiOver2;
            }

            // Update collider position and rotation
            _rectangleCollider.shape.Location = _rectangleCollider.shape.Center + velocity.ToPoint();
            _rectangleCollider.shape.Location -= new Point(ship_body.Width / 2, ship_body.Height / 2);

            // Handle screen wrapping
            HandleScreenWrapping();

            // Update the Buff timer
            if (buffTimer > 0)
                buffTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Ensure the camera follows the ship
            GameManager.GetGameManager().Camera.Follow(_rectangleCollider.shape.Center.ToVector2());

            base.Update(gameTime);
        }

        private void HandleScreenWrapping()
        {
            Rectangle playArea = GameManager.GetGameManager().PlayArea;

            if (_rectangleCollider.shape.Right < playArea.Left)
                _rectangleCollider.shape.X = playArea.Right;
            else if (_rectangleCollider.shape.Left > playArea.Right)
                _rectangleCollider.shape.X = playArea.Left - _rectangleCollider.shape.Width;

            if (_rectangleCollider.shape.Bottom < playArea.Top)
                _rectangleCollider.shape.Y = playArea.Bottom;
            else if (_rectangleCollider.shape.Top > playArea.Bottom)
                _rectangleCollider.shape.Y = playArea.Top - _rectangleCollider.shape.Height;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw the ship body with rotation
            spriteBatch.Draw(
                ship_body,
                _rectangleCollider.shape.Center.ToVector2(),
                null,
                Color.White,
                rotation,
                new Vector2(ship_body.Width / 2, ship_body.Height / 2),
                1f, // Scale
                SpriteEffects.None,
                0
            );

            float aimAngle = LinePieceCollider.GetAngle(LinePieceCollider.GetDirection(GetPosition().Center, target));
            if (buffTimer <= 0)
            {
                Rectangle turretLocation = base_turret.Bounds;
                turretLocation.Location = _rectangleCollider.shape.Center;
                spriteBatch.Draw(base_turret, turretLocation, null, Color.White, aimAngle, turretLocation.Size.ToVector2() / 2f, SpriteEffects.None, 0);
            }
            else
            {
                Rectangle turretLocation = laser_turret.Bounds;
                turretLocation.Location = _rectangleCollider.shape.Center;
                spriteBatch.Draw(laser_turret, turretLocation, null, Color.White, aimAngle, turretLocation.Size.ToVector2() / 2f, SpriteEffects.None, 0);
            }
            base.Draw(gameTime, spriteBatch);
        }

        public void Buff()
        {
            buffTimer = buffDuration;
        }

        public Rectangle GetPosition()
        {
            return _rectangleCollider.shape;
        }
    }
}

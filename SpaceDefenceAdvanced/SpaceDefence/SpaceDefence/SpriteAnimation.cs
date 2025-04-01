using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence
{
    public class SpriteAnimation
    {
        private Texture2D _spriteSheet;
        private int _frameWidth;
        private int _frameHeight;
        private int _frameCount;
        private int _currentFrame;
        private float _frameTime;
        private float _elapsedTime;
        private bool _isLooping;

        public string TextureName { get; set; }

        public bool IsFinished => !_isLooping && _currentFrame >= _frameCount;

        public int FrameWidth => _frameWidth;
        public int FrameHeight => _frameHeight;

        public SpriteAnimation(Texture2D spriteSheet, int frameWidth, int frameHeight, int frameCount, float frameTime, bool isLooping)
        {
            _spriteSheet = spriteSheet;
            _frameWidth = frameWidth;
            _frameHeight = frameHeight;
            _frameCount = frameCount;
            _frameTime = frameTime;
            _isLooping = isLooping;
            _currentFrame = 0;
            _elapsedTime = 0f;
        }

        public void Load(ContentManager content)
        {
            _spriteSheet = content.Load<Texture2D>(TextureName);
        }

        public void Update(GameTime gameTime)
        {
            _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_elapsedTime >= _frameTime)
            {
                _currentFrame++;
                _elapsedTime = 0f;

                if (_currentFrame >= _frameCount)
                {
                    if (_isLooping)
                    {
                        _currentFrame = 0;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float scale = 1.0f)
        {
            int row = _currentFrame / (_spriteSheet.Width / _frameWidth);
            int column = _currentFrame % (_spriteSheet.Width / _frameWidth);

            Rectangle sourceRectangle = new Rectangle(column * _frameWidth, row * _frameHeight, _frameWidth, _frameHeight);
            spriteBatch.Draw(_spriteSheet, position, sourceRectangle, color, 0f, new Vector2(_frameWidth / 2, _frameHeight / 2), scale, SpriteEffects.None, 0f);
        }
    }
}

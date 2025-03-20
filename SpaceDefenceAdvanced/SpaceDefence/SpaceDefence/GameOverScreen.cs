using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceDefence
{
    public class GameOverScreen
    {
        private SpriteFont _font;
        private GraphicsDevice _graphicsDevice;

        public GameOverScreen(SpriteFont font, GraphicsDevice graphicsDevice)
        {
            _font = font;
            _graphicsDevice = graphicsDevice;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            string gameOverText = "Game Over";
            string restartText = "Click to return to the Start Screen";

            Vector2 gameOverTextSize = _font.MeasureString(gameOverText);
            Vector2 restartTextSize = _font.MeasureString(restartText);

            Vector2 gameOverTextPosition = new Vector2(
                (_graphicsDevice.Viewport.Width - gameOverTextSize.X) / 2,
                (_graphicsDevice.Viewport.Height - gameOverTextSize.Y) / 2 - 20
            );

            Vector2 restartTextPosition = new Vector2(
                (_graphicsDevice.Viewport.Width - restartTextSize.X) / 2,
                (_graphicsDevice.Viewport.Height - restartTextSize.Y) / 2 + 20
            );

            spriteBatch.DrawString(_font, gameOverText, gameOverTextPosition, Color.Red);
            spriteBatch.DrawString(_font, restartText, restartTextPosition, Color.White);
            spriteBatch.End();
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceDefence
{
    public class PauseScreen
    {
        private SpriteFont _font;
        private GraphicsDevice _graphicsDevice;

        public PauseScreen(SpriteFont font, GraphicsDevice graphicsDevice)
        {
            _font = font;
            _graphicsDevice = graphicsDevice;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            string pauseText = "Game Paused";
            string continueText = "Continue";
            string quitText = "Quit Game";

            Vector2 pauseTextSize = _font.MeasureString(pauseText);
            Vector2 continueTextSize = _font.MeasureString(continueText);
            Vector2 quitTextSize = _font.MeasureString(quitText);

            Vector2 pauseTextPosition = new Vector2(
                (_graphicsDevice.Viewport.Width - pauseTextSize.X) / 2,
                (_graphicsDevice.Viewport.Height - pauseTextSize.Y) / 2 - 100
            );

            Vector2 continueTextPosition = new Vector2(
                (_graphicsDevice.Viewport.Width - continueTextSize.X) / 2,
                (_graphicsDevice.Viewport.Height - continueTextSize.Y) / 2 - 20
            );

            Vector2 quitTextPosition = new Vector2(
                (_graphicsDevice.Viewport.Width - quitTextSize.X) / 2,
                (_graphicsDevice.Viewport.Height - quitTextSize.Y) / 2 + 20
            );

            Rectangle continueTextArea = new Rectangle(continueTextPosition.ToPoint(), continueTextSize.ToPoint());
            Rectangle quitTextArea = new Rectangle(quitTextPosition.ToPoint(), quitTextSize.ToPoint());

            Color continueTextColor = IsMouseOver(continueTextArea) ? Color.Yellow : Color.White;
            Color quitTextColor = IsMouseOver(quitTextArea) ? Color.Yellow : Color.White;

            spriteBatch.DrawString(_font, pauseText, pauseTextPosition, Color.White);
            spriteBatch.DrawString(_font, continueText, continueTextPosition, continueTextColor);
            spriteBatch.DrawString(_font, quitText, quitTextPosition, quitTextColor);
            spriteBatch.End();
        }

        private bool IsMouseOver(Rectangle area)
        {
            MouseState mouseState = Mouse.GetState();
            return area.Contains(mouseState.X, mouseState.Y);
        }
    }
}

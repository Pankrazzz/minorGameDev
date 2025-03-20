using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceDefence
{
    public class StartScreen
    {
        private SpriteFont _font;
        private GraphicsDevice _graphicsDevice;
        private Texture2D _backgroundTexture;

        public StartScreen(SpriteFont font, GraphicsDevice graphicsDevice, Texture2D backgroundTexture)
        {
            _font = font;
            _graphicsDevice = graphicsDevice;
            _backgroundTexture = backgroundTexture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Draw the background image
            spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height), Color.White);

            string titleText = "Space Defence Advance DX Game of the Year Edition";
            string startText = "Start Game";
            string quitText = "Quit Game";

            Vector2 titleTextSize = _font.MeasureString(titleText);
            Vector2 startTextSize = _font.MeasureString(startText);
            Vector2 quitTextSize = _font.MeasureString(quitText);

            Vector2 titleTextPosition = new Vector2(
                (_graphicsDevice.Viewport.Width - titleTextSize.X) / 2,
                (_graphicsDevice.Viewport.Height - titleTextSize.Y) / 2 - 100
            );

            Vector2 startTextPosition = new Vector2(
                (_graphicsDevice.Viewport.Width - startTextSize.X) / 2,
                (_graphicsDevice.Viewport.Height - startTextSize.Y) / 2 - 20
            );

            Vector2 quitTextPosition = new Vector2(
                (_graphicsDevice.Viewport.Width - quitTextSize.X) / 2,
                (_graphicsDevice.Viewport.Height - quitTextSize.Y) / 2 + 20
            );

            Rectangle startTextArea = new Rectangle(startTextPosition.ToPoint(), startTextSize.ToPoint());
            Rectangle quitTextArea = new Rectangle(quitTextPosition.ToPoint(), quitTextSize.ToPoint());

            Color startTextColor = IsMouseOver(startTextArea) ? Color.Yellow : Color.White;
            Color quitTextColor = IsMouseOver(quitTextArea) ? Color.Yellow : Color.White;

            spriteBatch.DrawString(_font, titleText, titleTextPosition, Color.White);
            spriteBatch.DrawString(_font, startText, startTextPosition, startTextColor);
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

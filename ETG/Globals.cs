using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Animation2;

public static class Globals
{
    public static  float TotalSeconds { get; set; }
    public static ContentManager Content { get; set; }
    public static SpriteBatch SpriteBatch { get; set; }
    public static SpriteFont Font;
    public static GraphicsDevice GraphicsDevice { get; set; }
    public static GameTime GameTime;
    public static int ScreenWidth, ScreenHeight;

    public static void Update(GameTime gameTime)
    {
        TotalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        GameTime = gameTime;
    }
}
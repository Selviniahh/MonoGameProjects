using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AStar.Models;

public class Sprite
{
    protected readonly Texture2D _texture;
    public Vector2 Position { get; protected set; }
    public Vector2 Origin { get; protected set; }
    public Color Color { get; set; }
    public Rectangle Rectangle => new Rectangle((int)Position.X,(int)Position.Y,_texture.Width,_texture.Height);

    public Sprite(Texture2D texture, Vector2 position)
    {
        _texture = texture;
        Position = position;
        Origin = Vector2.Zero;
        Color = Color.White;
    }

    public virtual void Draw()
    {
        Globals.SpriteBatch.Draw(_texture,Position,null,Color,0f,Origin,1f,SpriteEffects.None,0f);
    }
}
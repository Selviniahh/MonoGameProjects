using System.Collections.Generic;
using Animation2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ETG;

public class Animation
{
    public Texture2D Texture;
    public Texture2D LastDrawnTexture;
    public Vector2 Origin { get; set; }
    private float _eachFrameSpeed;
    private float _animTimeLeft;
    // private int _frameX, _frameY;
    public List<Rectangle> _frames = new List<Rectangle>();
    private int _currentFrame = 0;
    private readonly int _frameX;
    private readonly int _frameY;
    private bool _active = true;

    public Animation(Texture2D texture, float eachFrameSpeed, int frameX, int frameY, int row = 1) //100 x 100
    {
        Texture = texture;
        _eachFrameSpeed = eachFrameSpeed;
        _frameX = frameX;
        _frameY = frameY;
        _animTimeLeft = _eachFrameSpeed;
        int frameXSize = Texture.Width / frameX; //800 / 8 = 100
        int frameYSize =Texture.Height / frameY; //800 / 8 = 100
        
        for (int i = 0; i < frameX; i++)
        {
            int frameWidth = i * frameXSize; //32 64 96 128 
            int frameHeight = (row - 1) * frameYSize ;// 32 32 32 32
            
            _frames.Add(new Rectangle(frameWidth,frameHeight,frameXSize,frameYSize));
        }
    }
    
    public void Update()
    {
        // LastDrawnTexture = Texture;
        if (!_active) return;
        
        _animTimeLeft -= Globals.TotalSeconds;

        if (_animTimeLeft <= 0)
        {
            _currentFrame++;
            _currentFrame = _currentFrame >= _frameX ? 0 : _currentFrame;
            _animTimeLeft = _eachFrameSpeed;
        }

    }

    public void Draw(Vector2 position, float layerDepth)
    {
        Globals.SpriteBatch.Draw(Texture,position,_frames[_currentFrame],Color.White,0f,Origin,5f,SpriteEffects.None,layerDepth);
    }

    public void Draw(Texture2D texture, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        Globals.SpriteBatch.Draw(Texture,position,_frames[_currentFrame],color,rotation,origin,scale,effects,layerDepth);
    }

    public void Start()
    {
        _active = true;
    }

    public void Stop()
    {
        _active = false;
    }

    public void Restart()
    {
        _currentFrame = 0;
        _animTimeLeft = _eachFrameSpeed;
    }

    public Texture2D GetCurrentFrameAsTexture()
    {
        Rectangle sourceRectangle = _frames[_currentFrame];
        
        Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
        Texture.GetData(0,sourceRectangle,data,0,data.Length);
        Texture2D frameTexture = new Texture2D(Globals.GraphicsDevice,sourceRectangle.Width,sourceRectangle.Height);
        frameTexture.SetData(data);
        return frameTexture;
    }

    public bool IsAnimationFinished()
    {
        return _currentFrame == _frameX - 1; //return if current frame is the last frame
    }
    
    public void RestartAnimation()
    {
        _currentFrame = 0;
        _animTimeLeft = _eachFrameSpeed;
    }


    
}
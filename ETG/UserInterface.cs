using System;
using System.Collections.Generic;
using System.Diagnostics;
using Animation2;
using ETG.Guns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ETG;

public class UserInterface
{
    private Texture2D _frame;
    private Texture2D _ammoBar;
    private Texture2D _ammoDisplay;
    public Texture2D Gun;
    private int frameOffsetY = 35;
    private int frameOffsetX = 10;
    private int GunOffsetX = 25;
    private int GunOffsetY = 25;
    private Vector2 _gunPosition;
    private Vector2 _framePosition;
    private Vector2 _ammoBarPosition;
    private List<Vector2> _ammoList = new List<Vector2>();
    private int _lastAmmoCount = 8;
    private bool isreloaded = true;
    private bool _RemoveLast = true;

    public UserInterface()
    {
        _frame = Globals.Content.Load<Texture2D>("UI/Frame");
        Gun = Globals.Content.Load<Texture2D>("Guns/knav3_reload_001");
        _ammoBar = Globals.Content.Load<Texture2D>("UI/AmmoBarUI");
        _ammoDisplay = Globals.Content.Load<Texture2D>("UI/AmmoDisplay");
        
        _framePosition = new Vector2((float)Globals.ScreenWidth/2 + (float)_frame.Width /2 + frameOffsetX,(float)Globals.ScreenHeight/2 + (float)_frame.Height/2 + frameOffsetY);
        _gunPosition = new Vector2(_framePosition.X + (float)_frame.Width/2 - GunOffsetX,_framePosition.Y + (float)_frame.Height/2 -GunOffsetY);
        _ammoBarPosition = new Vector2(_framePosition.X + 10 + _frame.Width,_framePosition.Y - 30);

        for (int i = 1; i < GunBase.AmmoCount + 1; i++)
        {
            var offset = 17;
            var X = 5;
            var Y = i * offset;
            _ammoList.Add(new Vector2(X + _framePosition.X + _frame.Width + 10,Y + _framePosition.Y - 28));
        }
    }

    public void Update()
    {
        if (GunBase.AmmoCount < _lastAmmoCount || GunBase.AmmoCount == 0)
        {
            try
            {
                _ammoList.RemoveAt(0);
            }
            catch 
            {
            }
            _lastAmmoCount = GunBase.AmmoCount;
        }
        
        if (RogueSpecial.ReloadFinished)
        {
            _ammoList.Clear(); // Clear the list.

            // Reinitialize the list.
            for (int i = 1; i < GunBase.AmmoCount + 1; i++)
            {
                var offset = 17;
                var X = 5;
                var Y = i * offset;
                _ammoList.Add(new Vector2(X + _framePosition.X + _frame.Width + 10,Y + _framePosition.Y - 28));
            }

            _lastAmmoCount = 8;
            RogueSpecial.ReloadFinished = false; // Reset the reload flag.
            _RemoveLast = false;
        }
        
    }
    
    public void Draw()
    {

        Globals.SpriteBatch.Draw(_frame,_framePosition,Color.White);
        Globals.SpriteBatch.Draw(Gun,_gunPosition,null,Color.White,0f,Vector2.Zero,4f,SpriteEffects.None,0.1f);
        Globals.SpriteBatch.Draw(_ammoBar,_ammoBarPosition,null,Color.White,0f,Vector2.Zero,1f,SpriteEffects.None,0.1f);

        for (int i = 0; i < _ammoList.Count; i++)
        {
            Globals.SpriteBatch.Draw(_ammoDisplay,_ammoList[i],Color.White);
            
        }
    }
    
}
using System;
using System.Diagnostics;
using Animation2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ETG;

public class InputManager
{
    public static float angleTest, degreeTest;
    private static Vector2 _direction;
    public static Vector2 Direction => _direction;
    public static bool Moving => _direction != Vector2.Zero;
    public static Vector2 MouseWorldPosition => new Vector2(CurrentMouse.X, CurrentMouse.Y);

    public static Vector2 HeroPosition;
    public static Vector2 HeroOrigin;
    
    private static Texture2D _redPixel;

    // public static Vector2 MouseDirection;

    public static MouseState CurrentMouse;
    public static MouseState PreviousMouse;
    
    public static KeyboardState CurrentKeyboard;
    public static KeyboardState PreviousKeyboard;
    public static void Update()
    {
        PreviousMouse = CurrentMouse;
        CurrentMouse = Mouse.GetState();

        PreviousKeyboard = CurrentKeyboard;
        CurrentKeyboard = Keyboard.GetState();
        
        HeroOrigin += HeroPosition;
        _direction = Vector2.Zero;
        var keyboardState = Keyboard.GetState();

        if (keyboardState.GetPressedKeyCount() > 0)
        {
            if (keyboardState.IsKeyDown(Keys.A)) _direction.X--;
            if (keyboardState.IsKeyDown(Keys.D)) _direction.X++;
            if (keyboardState.IsKeyDown(Keys.W)) _direction.Y--;
            if (keyboardState.IsKeyDown(Keys.S)) _direction.Y++;
        }
    }

    public static float GetMouseAngleRelativeToHero()
    {
        Vector2 diff = MouseWorldPosition - HeroPosition;
        float angle = (float)Math.Atan2(diff.Y, diff.X);
        return angle;
    }

    public static void Draw()
    {
        
       

        Globals.SpriteBatch.DrawString(Globals.Font,"Direction: " + _direction,new Vector2(0,0),Color.White);

        Globals.SpriteBatch.DrawString(Globals.Font,"Mouse GunPosition: " + MouseWorldPosition,new Vector2(0,15),Color.White);
        Globals.SpriteBatch.DrawString(Globals.Font,"HeroOrigin: " + HeroOrigin,new Vector2(0,30),Color.White);
        Globals.SpriteBatch.DrawString(Globals.Font,"HeroPosition: " + HeroPosition,new Vector2(0,45),Color.White);
        
        Vector2 diff = MouseWorldPosition - HeroPosition;
        float angle = (float)Math.Atan2(diff.Y, diff.X); //this will be returned to hero. All the calculation results are this variable
        
        Globals.SpriteBatch.DrawString(Globals.Font,"Relative Mouse pos: " + diff,new Vector2(0,60),Color.White);
        
        float angleInDegrees = MathHelper.ToDegrees(angle);
        Globals.SpriteBatch.DrawString(Globals.Font,"Moving: " + Moving,new Vector2(0,75),Color.White);


        

    }
}


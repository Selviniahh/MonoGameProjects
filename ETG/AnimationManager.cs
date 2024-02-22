using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ETG;

public class AnimationManager
{
    public readonly Dictionary<object, Animation> AnimationDict = new Dictionary<object, Animation>();
    public readonly Dictionary<object, Vector2> GunOrigin = new Dictionary<object, Vector2>();
    public object LastKey;
    public Texture2D LastTexture;
    

    //Add animation to dictionary to store all inside Dictionary
    public void AddAnimation<T>(T key, Animation animation)
    {
        AnimationDict.Add(key, animation);
            LastKey = key;
    }
    
    public void Update<T>(T key, Vector2 position)
    {
        LastTexture = AnimationDict[key].Texture;
        if (AnimationDict.ContainsKey(key))
        {
            AnimationDict[key].Update();
            LastKey = key;
        }
        else
        {
            AnimationDict[LastKey].Restart();
        }
    }

    public void Draw(Vector2 position, float layerDepth)
    {
        AnimationDict[LastKey].Draw(position, layerDepth);
    }
    public void Draw(Texture2D texture, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        AnimationDict[LastKey].Draw(texture, position, color, rotation, origin, scale, effects, layerDepth);
    }

    public void SetOrigin<T>(T key, Vector2 origin)
    {
        if (AnimationDict.ContainsKey(key))
        {
            AnimationDict[key].Origin = origin;
            
        }
    }
    
    //Get Rectangle as Texture but 
    public Texture2D GetCurrentFrameAsTexture()
    {
        return AnimationDict[LastKey].GetCurrentFrameAsTexture();
    }

    public bool IsAnimationFinished()
    {
        return AnimationDict[LastKey].IsAnimationFinished();
    }
}
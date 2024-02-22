using System;
using System.Collections.Generic;
using System.Linq;
using ETG.Enemies;
using Microsoft.Xna.Framework;

namespace ETG.GameLogics;

public class SpawnRandomBulletMan
{
    public List<BulletMan> BulletMen;
    private Random _random;
    private float _timer = 0f; 
    public void Initialize()
    {
        _random = new Random();
        BulletMen = new List<BulletMan>()
        {
            new BulletMan(Hero.Position - new Vector2(150,150))
        };
    }
    
    public void LoadContent()
    {
        foreach (var bulletMan in BulletMen)
        {
            bulletMan.LoadContent();
        }
    }

    public void Update()
    {
        //Create a new list because iterating list while list is changing will result error eventually
        List<BulletMan> newBulletMen = new List<BulletMan>();
        
        //Iterate from end to front
        for (int i = BulletMen.Count-1; i >=0; i--)
        {
            if (BulletMen.All(BulletMen => BulletMen.StopEverything))
            {
                //Nothing

            }
            else
            {
                BulletMen[i].Update(); 
            }
        }
        
        //If all the bullet men are stopped add one. 
        if (BulletMen.All(BulletMen => BulletMen.StopEverything))
        {
            newBulletMen.Add(new BulletMan(new Vector2(_random.Next(0,600),_random.Next(0,600))));
        }

        BulletMen.AddRange(newBulletMen);

        
        
    }

    public void Draw()
    {
        foreach (var bulletMan in BulletMen)
        {
            bulletMan.Draw();
        }
    }
}
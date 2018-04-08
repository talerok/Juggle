using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;

public class Trajectory : IArea
{
    private IFunc2D trajectroy;
    private IFunc2D speed;
    public Trajectory(IFunc2D tr, IFunc2D sp)
    {
        trajectroy = tr;
        speed = sp;
    }

    public Vector2 Get(Vector2 pos, float dt)
    {
        var npos = pos;
        npos.x += speed.Get(pos.x) * dt;
        npos.y = trajectroy.Get(npos.x);
        return npos;    
    }
}

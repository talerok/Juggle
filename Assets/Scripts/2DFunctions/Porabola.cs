using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;

namespace Functions{

    public class Parabola : IFunc2D
    {
        private float a;
        private float b;
        private float c;

        public Parabola(Vector2 f, Vector2 s, Vector2 t)
        {

            a = (t.y - (t.x * (s.y - f.y) + s.x * f.y - f.x * s.y) / (s.x - f.x)) / (t.x * (t.x - f.x - s.x) + f.x * s.x);
            b = (s.y - f.y) / (s.x - f.x) - a * (f.x + s.x);
            c = (s.x * f.y - f.x * s.y) / (s.x - f.x) + a * f.x * s.x;
        }

        public float Get(float x)
        {
            return a * x * x + b * x + c;
        }
    }
}
using Interfaces;

namespace Functions
{
    public class Const : IFunc2D
    {

        float param;

        public Const(float pr)
        {
            param = pr;
        }

        public float Get(float x)
        {
            return param;
        }
    }
}
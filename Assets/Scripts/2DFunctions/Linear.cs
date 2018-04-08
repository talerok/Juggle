using Interfaces;
namespace Functions
{
    public class Linear : IFunc2D
    {

        float param;

        public Linear(float pr)
        {
            param = pr;
        }

        public float Get(float x)
        {
            return param * x;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameInfrastructure
{
    public static class MyMath
    {
        public static float Cos(float theta)
        {
            return (float)Math.Cos(theta);
        }

        public static float Sin(float theta)
        {
            return (float)Math.Sin(theta);
        }

        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }

        private static float RadianToDegree(float angle)
        {
            return angle * (180.0f / (float)Math.PI);
        }

        private static float DegreeToRadian(float angle)
        {
            return (float)Math.PI * angle / 180.0f;     
        }
    }
}

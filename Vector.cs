using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TensorShow
{
    [Serializable]
    class Vector : Matrix
    {
        #region Constructors

        public Vector(int rows, bool isParam = false) : base(rows, 1, isParam) { }

        public Vector(int rows, double initialize) : base(rows, 1, initialize) { }

        public Vector(int rows, double[] initialize) : base(rows, 1, initialize) { }

        #endregion

        #region Operations

        public static Vector OneHot(int y, int n)
        {
            Vector v = new Vector(n, 0);
            v[y, 0].Value = 1;
            return v;
        }

        public static int ArgMax(Matrix a)
        {
            if (a.Rows == 0) throw new IncompatibleSizeException();

            double max = a.elements[0].Value;
            int arg = 0;

            for (int i = 1; i < a.Rows; i++)
            {
                if (a.elements[i].Value > max)
                {
                    arg = i;
                    max = a.elements[i].Value;
                }
            }
            return arg;
        }

        public static Variable ReduceSquared(Matrix a)
        {
            return Matrix.Sum(a, x => x.Power(2));
        }

        #endregion
    }
}

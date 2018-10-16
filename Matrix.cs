using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TensorShow
{
    [Serializable]
    class Matrix
    {
        #region Fields and properties

        public Variable[] elements;

        public int Rows { get; set; }

        public int Cols { get; set; }

        #endregion

        #region Other

        private void Generate(Func<int, Variable> f)
        {
            elements = (from i in Enumerable.Range(0, Rows * Cols) select f(i)).ToArray();
        }

        public Variable this[int i, int j]
        {
            get
            {
                return elements[i * Cols + j];
            }
            set
            {
                elements[i * Cols + j] = value;
            }
        }

        #endregion

        #region Constructors

        public Matrix(int rows, int cols) : this(rows, cols, false) {}

        public Matrix(int rows, int cols, bool isParam = false)
        {
            Rows = rows;
            Cols = cols;

            if (isParam) Generate(i => new Parameter());
            else Generate(i => new Variable());
        }

        public Matrix(int rows, int cols, double[] initialize, bool isParam = false)
        {
            Rows = rows;
            Cols = cols;

            if (isParam) Generate(i => new Parameter(initialize[i]));
            else Generate(i => new Variable(initialize[i]));
        }

        public Matrix(int rows, int cols, double initialize, bool isParam = false)
        {
            Rows = rows;
            Cols = cols;

            if (isParam) Generate(i => new Parameter(initialize));
            else Generate(i => new Variable(initialize));
        }

        #endregion
        
        #region Operators

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (!(a.Cols == b.Rows))
                throw new IncompatibleSizeException();

            Matrix output = new Matrix(a.Rows, b.Cols);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < b.Cols; j ++)
                {
                    Variable sum = new Variable();
                    for (int k = 0; k < a.Cols; k++)
                    {
                        sum += a[i, k] * b[k, j];
                    }
                    output[i, j] = sum;
                }
            }
            return output;
        }

        public static Matrix operator -(Matrix a)
        {
            return Map(a, x => -x);
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            return Map(a, b, (x, y) => x - y);
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            return Map(a, b, (x, y) => x + y);
        }

        public static Matrix operator +(Matrix a, Variable b)
        {
            return Map(a, x => x + b);
        }

        public static Matrix operator -(Matrix a, Variable b)
        {
            return Map(a, x => x - b);
        }

        #endregion

        #region Manipulators

        public static Matrix Map(Matrix a, Matrix b, Func<Variable, Variable, Variable> f)
        {
            if (!(a.Rows == b.Rows && a.Cols == b.Cols))
                throw new IncompatibleSizeException();

            Matrix output = new Matrix(a.Rows, a.Cols);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Cols; j++)
                {
                    output[i, j] = f(a[i, j], b[i, j]);
                }
            }
            return output;
        }

        public static Matrix Map(Matrix a, Func<Variable, Variable> f)
        {
            Matrix output = new Matrix(a.Rows, a.Cols);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Cols; j++)
                {
                    output[i, j] = f(a[i, j]);
                }
            }
            return output;
        }

        public static Variable Sum(Matrix a)
        {
            return Sum(a, x => x);
        }

        public static Variable Sum(Matrix a, Func<Variable, Variable> f)
        {
            Variable sum = new Variable(0);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Cols; j++)
                {
                    sum += f(a[i, j]);
                }
            }
            return sum;
        }

        #endregion

        #region Helpers

        public void Save(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Create))
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, this);
            }
        }

        public static Matrix Load(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new BinaryFormatter();
                return (Matrix)binaryFormatter.Deserialize(stream);
            }
        }

        #endregion

        protected class IncompatibleSizeException : Exception
        {
            public IncompatibleSizeException() : 
                base("The operands have incompatible sizes for specified operation. ") { }
        }
    }
}

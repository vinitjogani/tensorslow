using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TensorShow
{
    [Serializable]
    class Variable
    {
        #region Fields and properties

        private static Random randy = new Random();

        private string Name { get; set; }

        public double Value { get; set; }

        public double Gradient { get; set; }

        EventHandler<BackpropogationEventArgs> Changed;

        #endregion

        #region Constructors

        public Variable()
        {
            Value = randy.NextDouble();
        }

        public Variable(double initial)
        {
            Value = initial;
        }

        #endregion

        #region Gradient updates

        public void Backward(double gradient)
        {
            Changed?.Invoke(this, new BackpropogationEventArgs(gradient, this.Value));
        }

        void Backward(object sender, BackpropogationEventArgs e, Func<double, double, double> transform)
        {
            this.Gradient = transform(e.Gradient, e.Value);
            Backward(this.Gradient);
        }

        #endregion

        #region Operators

        public static Variable operator +(Variable a, Variable b)
        {
            var output = new Variable(a.Value + b.Value);

            Func<double, double, double> t = (g, v) => g;
            output.Changed += (o, e) => a.Backward(o, e, t);
            output.Changed += (o, e) => b.Backward(o, e, t);

            return output;
        }

        public static Variable operator +(Variable a, double b)
        {
            var output = new Variable(a.Value + b);

            Func<double, double, double> t = (g, v) => g;
            output.Changed += (o, e) => a.Backward(o, e, t);

            return output;
        }

        public static Variable operator -(Variable a)
        {
            var output = new Variable(-a.Value);

            Func<double, double, double> t = (g, v) => -g;
            output.Changed += (o, e) => a.Backward(o, e, t);

            return output;
        }

        public static Variable operator -(Variable a, Variable b)
        {
            return a + -b;
        }

        public static Variable operator *(Variable a, Variable b)
        {
            var output = new Variable(a.Value * b.Value);

            Func<double, double, double> t1 = (g, v) => g / b.Value;
            Func<double, double, double> t2 = (g, v) => g / a.Value;
            output.Changed += (o, e) => a.Backward(o, e, t1);
            output.Changed += (o, e) => b.Backward(o, e, t2);

            return output;
        }

        public static Variable operator *(Variable a, double b)
        {
            var output = new Variable(a.Value * b);

            Func<double, double, double> t = (g, v) => g / b;
            output.Changed += (o, e) => a.Backward(o, e, t);

            return output;
        }

        public static Variable operator /(Variable a, Variable b)
        {

            var output = new Variable(a.Value / b.Value);

            Func<double, double, double> t1 = (g, v) => g * b.Value;
            Func<double, double, double> t2 = (g, v) => g * (-a.Value) / (v * v);
            output.Changed += (o, e) => a.Backward(o, e, t1);
            output.Changed += (o, e) => b.Backward(o, e, t2);

            return output;
        }

        public static Variable operator /(double a, Variable b)
        {
            return b.Power(-1) * a;
        }

        #endregion

        #region Manipulators

        public Variable Power(double x)
        {
            var output = new Variable(Math.Pow(this.Value, x));

            Func<double, double, double> t = (g, v) => g * (1 / (x * Math.Pow(this.Value, x - 1)));
            output.Changed += (o, e) => this.Backward(o, e, t);

            return output;
        }

        public Variable Exponent(double x)
        {
            var output = new Variable(Math.Pow(x, this.Value));

            Func<double, double, double> t = (g, v) => g * (1 / (v * Math.Log(x)));
            output.Changed += (o, e) => this.Backward(o, e, t);

            return output;
        }

        public Variable Sigmoid()
        {
            return 1 / ((-this).Exponent(Math.E) + 1);
        }

        #endregion

        class BackpropogationEventArgs : EventArgs
        {
            public BackpropogationEventArgs(double gradient, double value)
            {
                this.Gradient = gradient;
                this.Value = value;
            }

            public double Gradient { get; }
            public double Value { get; }
        }
    }
}

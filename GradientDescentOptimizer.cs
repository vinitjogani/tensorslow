using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TensorShow
{
    class GradientDescentOptimizer : Optimizer
    {
        public GradientDescentOptimizer( double alpha )
        {
            LearningRate = alpha;
        }

        public double LearningRate { get; set; }

        public void Step(Parameter p)
        {
            if (p.Gradient != 0)
                p.Value -= LearningRate * (1 / p.Gradient);
        }
    }
}

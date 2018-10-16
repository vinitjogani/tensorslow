using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TensorShow
{
    class TensorSlow
    {
        public static List<Parameter> parameters = new List<Parameter>();
        
        public static void Minimize(Optimizer optim, Variable var)
        {
            var.Backward(1);
            foreach (var param in parameters)
            {
                optim.Step(param);
            }
        }
    }
}

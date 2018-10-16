using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TensorShow
{
    [Serializable]
    class Parameter : Variable
    {
        public Parameter() : base()
        {
            TensorSlow.parameters.Add(this);
        }

        public Parameter(double initialize) : base(initialize)
        {
            TensorSlow.parameters.Add(this);
        }
    }
}

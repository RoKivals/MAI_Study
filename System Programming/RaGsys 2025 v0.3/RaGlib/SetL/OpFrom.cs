using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.SetL
{
    public class OpFrom : OpNode
    {
        private OpVariable _variable;
        private OpNode _array;

        public OpFrom(OpVariable variable, OpNode array)
        {
            _variable = variable;
            _array = array;
        }

        public override DataNode GetResult(Dictionary<string, Variable> variables)
        {
            return new DataVariableSet(_variable.Name, _array.GetResult(variables));
        }
    }
}

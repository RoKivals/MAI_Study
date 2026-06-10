using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.SetL
{
    public class OpVariable : OpNode
    {
        public string Name { get; }

        public OpVariable(string name)
        {
            Name = name;
        }

        public override DataNode GetResult(Dictionary<string, Variable> variables)
        {
            return variables[Name].Data;
        }
    }
}

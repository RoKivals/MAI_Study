using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.SetL
{
    public class OpCondition: OpNode
    {
        private OpFrom _variableSet;
        private OpNode _condition;

        public OpCondition(OpFrom variableSet, OpNode condition)
        {
            _variableSet = variableSet;
            _condition = condition;
        }

        public override DataNode GetResult(Dictionary<string, Variable> variables)
        {
            var variableSet = _variableSet.GetResult(variables) as DataVariableSet;
            var result = new DataChains("");
            foreach(var variable in variableSet.Elements)
            {
                var newVariables = variables.Clone();
                newVariables[variableSet.Name.Trim()] = new Variable(variableSet.Name.Trim(), variable);
                if((_condition.GetResult(newVariables) as DataBool).Result)
                {
                    result.Chains.Add(variable as DataChain);
                }
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.SetL
{
    public class Variable
    {
        public string Name { get; set; }
        public DataNode Data { get; set; }

        public Variable(string name, DataNode data)
        {
            Name = name;
            Data = data;
        }

        public static Variable Parse(SETL setl, string line)
        {
            var result = new Variable(string.Empty, null);
//sas            var rgx = Parser.Matches(Parser.VariableRegex, line).First().Groups;
//            result.Name = rgx["name"].Value;
//            result.Data = DataNode.Parse(setl, rgx["data"].Value);
            return result;
        } 
    }
}

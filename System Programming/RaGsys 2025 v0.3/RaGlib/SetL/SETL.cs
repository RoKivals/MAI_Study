using RaGlib.SETL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.SetL
{
    public class SETL
    {
        public Dictionary<string, Variable> Variables { get; }
        public Dictionary<string, Function> Functions { get; }

        public SETL(string text)
        {
            var aa = new int[] { 1, 2, 3};
            Variables = new Dictionary<string, Variable>();
            Functions = new Dictionary<string, Function>();
            var lines = Utils.Split(text, new string[] { "\r\n", "\n", "\r" }).Select(l => l.Trim());
            foreach(var line in lines.Where(l => l.Length > 0 && l[0] != '#'))
            {
                Console.Write($"{line}\t");
                var variable = Variable.Parse(this, line);
                Console.WriteLine($"({variable.Data.GetType()})");
                Variables[variable.Name] = variable;
            }
            Console.WriteLine();
        }
    }
}

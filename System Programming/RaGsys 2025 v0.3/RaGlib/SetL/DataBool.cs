using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.SetL
{
    public class DataBool: DataNode
    {
        public bool Result { get; }

        public DataBool(bool result)
        {
            Result = result;
        }

        public override string ToString()
        {
            return Result ? "TRUE" : "FALSE";
        }
    }
}

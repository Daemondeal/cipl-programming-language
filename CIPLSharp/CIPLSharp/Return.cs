using System;
using System.Transactions;

namespace CIPLSharp
{
    public class Return : Exception
    {
        public object Value;
        
        public Return(object value)
        {
            Value = value;
        }
    }
}
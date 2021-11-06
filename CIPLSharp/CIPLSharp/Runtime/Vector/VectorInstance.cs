using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIPLSharp.Runtime
{
    public class VectorInstance : CiplInstance
    {
        private readonly List<object> internalList = new();
        
        public VectorInstance(CiplClass ciplClass) : base(ciplClass)
        {
            
        }

        public object Get(int index)
        {
            if (index < 0 || index > internalList.Count)
                throw new RuntimeError($"Vector index out of bounds (index: {0}, length: {internalList.Count})");
            return internalList[index];
        }

        public void Push(object item)
        {
            internalList.Add(item);
        }

        public void Set(int index, object item)
        {
            internalList[index] = item;
        }

        public int Length()
        {
            return internalList.Count;
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", internalList.Select(Interpreter.Stringify))}]";
        }
    }
}
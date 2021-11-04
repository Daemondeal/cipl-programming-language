using System;
using System.Collections.Generic;
using System.Linq;

namespace CIPLSharp
{
    public class Environment
    {
        public readonly Environment Enclosing;
        
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        public Environment()
        {
            Enclosing = null;
        }

        public Environment(Environment enclosing)
        {
            Enclosing = enclosing;
        }
        
        public void Define(string name, object value)
        {
            values[name] = value;
        }

        public object GetAt(int distance, string name)
        {
            var ancestor = Ancestor(distance);
            if (!ancestor.values.ContainsKey(name))
                throw new Exception("Could not find resolved variable. This could be a resolver bug");
            
            return ancestor.values[name];
        }

        public void AssignAt(int distance, Token name, object value)
        {
            Ancestor(distance).values[name.Lexeme] = value;
        }

        private Environment Ancestor(int distance)
        {
            var env = this;
            for (var i = 0; i < distance; i++)
                env = env.Enclosing;

            return env;
        }

        public object Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
                return values[name.Lexeme];

            if (Enclosing != null)
                return Enclosing.Get(name);

            throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
        }

        public void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
                return;
            }

            if (Enclosing != null)
            {
                Enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
        }
    }
}
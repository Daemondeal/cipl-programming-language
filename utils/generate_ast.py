import sys

def main():
    if len(sys.argv) != 2:
        print("Usage: generate_ast <output_dir>")
        exit(64)
    
    out_dir = sys.argv[1]

    create_file(out_dir, "Expr", [
        ("Logical", "Expr left, Token operatorToken, Expr right"),
        ("Binary", "Expr left, Token operatorToken, Expr right"),
        ("Call", "Expr callee, Token paren, List<Expr> arguments"),
        ("Unary", "Token operatorToken, Expr right"),
        ("Literal", "object value"),
        ("Grouping", "Expr expression"),
        ("Variable", "Token name"),
        ("Assign", "Token name, Expr value")
    ])

    create_file(out_dir, "Statement", [
        ("ExpressionStatement", "Expr expression"),
        ("Procedure", "Token name, List<Token> parameters, List<Statement> body"),
        ("Return", "Token keyword, Expr value"),
        ("If", "Expr condition, Statement thenBranch, Statement elseBranch"),
        ("Var", "Token name, Expr initializer"),
        ("While", "Expr condition, Statement body"),
        ("Break", "Token breakToken"),
        ("Pass", "Token passToken"),
        ("Block", "List<Statement> statements")
    ])

def capitalize(string):
    return string[0].upper() + string[1:]

def create_file(out_dir, base_name, types):
    file = open(f"{out_dir}/{base_name}.cs", 'w')

    def out(content, indent=0):
        file.write(" " * (indent * 4))
        file.write(content)
        file.write("\n")

    def define_ast():
        out("using System.Collections.Generic;")
        out("")
        out("namespace CIPLSharp")
        out("{")

        out(f"public abstract class {base_name}", 1)
        out("{", 1)

        define_visitor()

        out("")
        out("public abstract T Accept<T>(IVisitor<T> visitor);", 2)
        out("")

        # AST classes
        for class_name, fields in types:
            define_type(class_name, fields)

        out("}", 1)
        out("}")

    def define_visitor():
        out("public interface IVisitor<T>", 2)
        out("{", 2)
        for type_name, _ in types:
            out(f"T Visit{type_name}{base_name}({type_name} {base_name.lower()});", 3)
        out("}", 2)

    def define_type(class_name, fields):
        out(f"public class {class_name} : {base_name}", 2)
        out("{", 2)
        
        # Fields
        if fields != "":
            for field in fields.split(", "):
                type, name = field.split(" ")
                out(f"public readonly {type} {capitalize(name)};", 3)
        
        out("")

        # Constructor
        out(f"public {class_name}({fields})", 3)
        out("{", 3)
        if fields != "":
            for field in fields.split(", "):
                name = field.split(" ")[1].strip()
                out(f"{capitalize(name)} = {name};", 4)
        out("}", 3)
        out("")

        # Accept
        out("public override T Accept<T>(IVisitor<T> visitor)", 3)
        out("{", 3)
        out(f"return visitor.Visit{class_name}{base_name}(this);", 4)
        out("}", 3)

        out("}", 2)

    define_ast()
    file.close()
        

if __name__ == "__main__":
    main()
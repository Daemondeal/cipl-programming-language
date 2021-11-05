#!/usr/bin/python

import subprocess
import sys
import os

from termcolor import colored

INTERPRETER = "../CIPLSharp/CIPLSharp/bin/Debug/net5.0/CIPLSharp"
ISOLATION_FOLDER_NAME = "isolated"

# Run a program and return decoded stdout and stderr
def run(program, *args):
    p = subprocess.Popen([program] + list(args), stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    out, err = p.communicate()
    return out.decode('utf-8'), err.decode('utf-8')

def interpret_file(file):
    return run(INTERPRETER, file)

def generate_tokens(file):
    return run(INTERPRETER, file, "--tokens", "--stdout")[0]

def generate_ast(file):
    return run(INTERPRETER, file, "--ast", "--stdout")[0]

def record_all_tests():
    # Iterate over all files with .cipl extension recursively
    for file in subprocess.check_output(["find", ".", "-name", "*.cipl"]).decode('utf-8').splitlines():
        print("Generating test outputs for " + file + "...")
        record_file(file)

# Delete recursively all files with extension .out, .tokens and .ast
def clean_files():
    if input("Deleting all test outputs. Are you sure? [y/N]\n").lower() != "y":
        return

    for file in subprocess.check_output(["find", ".", "-name", "*.out"]).decode('utf-8').splitlines():
        subprocess.call(["rm", file])
    for file in subprocess.check_output(["find", ".", "-name", "*.tokens"]).decode('utf-8').splitlines():
        subprocess.call(["rm", file])
    for file in subprocess.check_output(["find", ".", "-name", "*.ast"]).decode('utf-8').splitlines():
        subprocess.call(["rm", file])

    # Delete ISOLATION_FOLDER_NAME if it exists
    if os.path.exists(ISOLATION_FOLDER_NAME):
        subprocess.call(["rm", "-r", ISOLATION_FOLDER_NAME])

def record_file(file):
    # Get the tokens
    tokens = generate_tokens(file)

    # Get the AST
    ast = generate_ast(file)

    # Get the output
    stdout, stderr = interpret_file(file)

    # Write the tokens to a file
    with open(file + ".tokens", "w") as f:
        f.write(tokens)

    # Write the AST to a file
    with open(file + ".ast", "w") as f:
        f.write(ast)

    # Write the output to a file
    with open(file + ".out", "w") as f:
        f.write("::stdout::\n" + stdout)
        f.write("\n::stderr::\n" + stderr)


def test_file(file):
    ast = generate_ast(file)
    tokens = generate_tokens(file)
    stdout, stderr = interpret_file(file)
    stdout = stdout.strip()
    stderr = stderr.strip()

    # Read ast from .ast file
    with open(file + ".ast", "r") as f:
        previous_ast = f.read()

    # Read tokens from .tokens file
    with open(file + ".tokens", "r") as f:
        previous_tokens = f.read()

    # Read output from .out file
    with open(file + ".out", "r") as f:
        output_raw = f.read()

        previous_stdout = output_raw.split("::stdout::\n")[1].split("::stderr::\n")[0].strip()
        previous_stderr = output_raw.split("::stderr::\n")[1].strip()

    # Compare ASTs
    if ast != previous_ast:
        print("ASTs do not match!")
        print("Expected:")
        print(previous_ast)
        print("Actual:")
        print(ast)
        return False

    # Compare tokens
    if tokens != previous_tokens:
        print("Tokens do not match!")
        print("Expected:")
        print(previous_tokens)
        print("Actual:")
        print(tokens)
        return False

    # Compare stdout
    if stdout != previous_stdout:
        print("Stdout does not match!")
        print("Expected:")
        print(previous_stdout)
        print("Actual:")
        print(stdout)
        return False

    # Compare stderr
    if stderr != previous_stderr:
        print("Stderr does not match!")
        print("Expected:")
        print(previous_stderr)
        print("Actual:")
        print(stderr)
        return False

    return True

def isolate_file(file):
    # Create folder "testing" if it doesn't exist
    if not os.path.exists(ISOLATION_FOLDER_NAME):
        os.makedirs(ISOLATION_FOLDER_NAME)
    # If folder "testing" already existed, delete every file inside of it
    else:
        for f in os.listdir(ISOLATION_FOLDER_NAME):
            os.remove(ISOLATION_FOLDER_NAME + "/" + f)


    # Copy file, file.tokens, file.ast and file.out to "testing" folder
    subprocess.call(["cp", file, "testing"])
    subprocess.call(["cp", file + ".tokens", "testing"])
    subprocess.call(["cp", file + ".ast", "testing"])
    subprocess.call(["cp", file + ".out", "testing"])
    


def perform_all_tests():
    tests_passed, tests_performed = 0, 0
    
    # Iterate over all file with .cipl extension
    for file in subprocess.check_output(["find", ".", "-name", "*.cipl"]).decode('utf-8').splitlines():
        print("Testing " + file + "...")
        result = test_file(file)
        if result:
            print(colored("Test passed!", "green"))
            tests_passed += 1
        else:
            print(colored("Test failed!", "red"))
            tests_performed += 1

        tests_performed += 1
        print()

    print(f"Tests passed: {tests_passed}/{tests_performed} ({round((tests_passed * 100)/tests_performed, 2)}%)")

def main():
    # Get command line arguments
    args = [arg for arg in sys.argv[1:] if arg[0] != '-']

    # Get command line parameters
    params = {arg[1:]: True for arg in sys.argv[1:] if arg[0] == '-'}

    possible_commands = ["record", "test", "clean", "record-file", "record-folder", "test-file", "test-folder", "isolate"]

    if len(args) == 0 or args[0] not in possible_commands:
        print("Usage: test.py " + "|".join(possible_commands))
        return

    if args[0] == "record":
        record_all_tests()
    elif args[0] == "clean":
        clean_files()
    elif args[0] == "test":
        perform_all_tests()
    elif args[0] == "record-file":
        if len(args) != 2:
            print("Usage: test.py record-file <file>")
            return
        record_file(args[1])
    elif args[0] == "record-folder":
        if len(args) != 2:
            print("Usage: test.py record-folder <folder>")
            return
        for file in subprocess.check_output(["find", args[1], "-name", "*.cipl"]).decode('utf-8').splitlines():
            record_file(file)
    elif args[0] == "test-file":
        if len(args) != 2:
            print("Usage: test.py test-file <file>")
            return
        test_file(args[1])
    elif args[0] == "test-folder":
        if len(args) != 2:
            print("Usage: test.py test-folder <folder>")
            return
        for file in subprocess.check_output(["find", args[1], "-name", "*.cipl"]).decode('utf-8').splitlines():
            test_file(file)
    elif args[0] == "isolate":
        if len(args) != 2:
            print("Usage: test.py isolate <file>")
            return
        isolate_file(args[1])


if __name__ == "__main__":
    main()
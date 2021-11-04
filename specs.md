# CIPL - C# Interpreted Programming Language / C Interpreted Programming Language / C Is a Programming Language

A multi-paradigm language inspired by Rust and Python, and created to learn about compilers and interpreters

## Comments

CIPL uses Python/Ruby style comments:
```python
# This is a comment
```
## Separators

Lines are separated by newlines(CIPL doesn't use semicolons). Each newline is considered a separator, unless the next line starts with a dot `.`

## Block

Each block is separated by indentation after a colon (`:`).

```python
if a:
    if b:
        # Block b
    # Block a
# Global scope
```

## Dynamic Typing

You don't need to specify types: CIPL is a dynamically typed language.

## Memory Managmente

CIPL implements a Tracing Garbage Collector to clean memory after it's not needed anymore.

## Data Types

These are all the data types that CIPL uses:

1. Booleans value, either `true` or `false`
```rust
true
false
```
2. Numbers: every number is internally a double precision floating point number. They are written using number literals:
```python
1234 # An integer
12.34 # A decimal number

# Internally, both of those are of the same type
```
3. Strings: they are defined by enclosing them in double quotes `"`:
```python
"Hello World!"
"" # This is an empty string
"123"
```
4. Null: represents no value.
```C#
null
```

# Expressions

## Arithmetic Expressions

CIPL supports the four basic arithmetic expressions, and also the negation unary expression

```python
1 + 1
2 * 2
3 - 1
4 / 2

-4
```

## Concatenating strings

CIPL doesn't allow using `+` to concatenate strings, you use `~`

```python
"Hello" ~ " " ~ "World"
```

There is no implicit conversion, you have to manually convert other types to string if you want to append them to other strings.

## Comparison and equality

```python
2 < 4
2 <= 2
4 > 3
4 >= 4
```

Values of different types are alwyas regarded as different

```python

4 == 4 # True
4 != 5 # True
"4" == "4" # True
4 == "4" # False
```

## Logical Operators

The `not` operator returns `false` if the expression is falsy and `true` if the expression is truthy:
```python
not true # false
not false # true
```

And expressions return `true` if both expressions are `true`, and `false` if both expressions are false.

```python
a and b
a or b
```

If the first expression in an `and` is false, the second one is not executed. Same thing happens if the first expression in an `or` is false.

### Truthyness
TODO

# Statements

## Expression Statements

If you put an expression in a single line without anything, it will be evaluated as if it was a statement

```python
"Hello " ~ "World" # Does nothing, but it evaluates the expression
```

## Declaring variables

Variable names must be alphanumeric, and can include underscores. A variable cannot start with a number.
Variables are declared with the `let` keyword:

```rust
let variable_name = 123
let initialized_variable
```

If you don't initialize a variable, its default value will be `null`

## Pass
`pass` does nothing. It's useful to create empty functions or loops

```python
if a == true:
    pass
```

# Control Flow

## If statements
An if statements executes one of two statements based on some condition

```python
if condition:
    print "yes"
else:
    print "no"
```

## Loops

There are two fundamental loops in CIPL: `while` loops and `repeat` loops.

`while` loops executes the body repeatedly as long as the condition expression evaluates to `true`
```python
let i = 0
while i < 10:
    print i
    i = i + 1
```

`repeat` loops instead repeat the body a specified number of times, and save the current loop index in a variable:

```python
repeat i 10:
    print i
```

Equivalent with `for`

```python
for i in 0..10:
    print i
```

You can get out of loops with the `break` keyword:
```python
let a = 0
while true:
    a = a + 1
    if a > 2:
        break
```


## Temporary implementations

For testing implementations, CIPL has the `print` statement and the `clock` expression.
The first simply prints out whatever is next:
```python
print "Hello World!"
```

The seconds returns the time elapsed from program start, in milliseconds:
```python
let start = clock

# Some instructions

print end - start  # Prints time taken to do those instructions
```

# Keywords

Complete list of keywords used

```
true, false, null, not, and, or, let, pass, if, else, return, while, repeat, print, clock, proc, class, super, this, break
```

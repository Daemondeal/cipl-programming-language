import time

def fib(n):
    if n <= 1:
        return n

    return fib(n - 2) + fib(n - 1)

start = time.time_ns()

for n in range(20):
    print(fib(n))

print("Time taken: " + str(float(time.time_ns() - start) / 1000000) + "ms")

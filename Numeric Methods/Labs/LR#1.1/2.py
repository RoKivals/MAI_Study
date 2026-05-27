import numpy as np

def tridiagonal_solve(a, b, c, d) -> np.ndarray:
    n = len(d)
    p = np.ndarray(n, dtype=float)
    q = np.ndarray(n, dtype=float)
    x = np.ndarray(n, dtype=float)

    p[0] = -c[0] / b[0]
    q[0] = d[0] / b[0]

    for i in range(1, n):
        p[i] = -c[i] / (b[i] + a[i]*p[i-1])
        q[i] = (d[i] - a[i]*q[i-1]) / (b[i] + a[i]*p[i-1])

    x[-1] = q[-1]
    for i in range(n-2, -1, -1):
        x[i] = p[i] * x[i+1] + q[i]
    return x


def main():
    a = np.array(list(map(int, input().split())), dtype=float)
    b = np.array(list(map(int, input().split())), dtype=float)
    c = np.array(list(map(int, input().split())), dtype=float)
    d = np.array(list(map(int, input().split())), dtype=float)
    x = tridiagonal_solve(a, b, c, d)
    print(f"x: {x}")
    print("Проверка:")
    n = len(d)
    for i in range(n):
        x1 = x[i-1] if i > 0 else 0
        x2 = x[i]
        x3 = x[i+1] if i < n - 1 else 0
        print(f"{a[i]*x1 + b[i]*x2 + c[i]*x3} = {d[i]}")


if __name__ == "__main__":
    main()
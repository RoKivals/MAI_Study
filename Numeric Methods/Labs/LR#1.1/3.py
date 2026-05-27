import numpy as np

INF = 10e9

# Ќорма матрицы, использу€ максимальную сумму по строкам
def mat_norm(m: np.ndarray) -> int:
    res = -INF
    for row in m:
        res = max(res, sum(map(abs, row)))
    return res

# Ќорма вектора через максимальное значение по модулю
def vec_norm(v: np.ndarray) -> int:
    res = -INF
    for a in v:
        res = max(res, abs(a))
    return res


def jacobi_cast(a: np.ndarray, b: np.ndarray) -> tuple[np.ndarray, np.ndarray]:
    n = len(b)
    beta = np.ndarray((n,), dtype=float)
    alpha = np.ndarray((n, n), dtype=float)
    for i in range(n):
        beta[i] = b[i] / a[i, i]
        for j in range(n):
            alpha[i, j] = -a[i, j] / a[i, i] if i != j else 0
    return alpha, beta


def zeidel_iteration(x: np.ndarray, alpha: np.ndarray, beta: np.ndarray) -> np.ndarray:
    n = len(x)
    x_k = beta.copy()
    for i in range(n):
        for j in range(i):
            x_k[i] += x_k[j] * alpha[i][j]
        for j in range(i, n):
            x_k[i] += x[j] * alpha[i][j]
    return x_k


def simple_iterations_solve(a: np.ndarray, b: np.ndarray, eps: float) -> tuple[np.ndarray, int]:
    alpha, beta = jacobi_cast(a, b)
    eps_k = 1
    matrix_coef = mat_norm(alpha)/(1-mat_norm(alpha))
    x = beta.copy()
    iter_count = 0
    while eps < eps_k:
        x_k = beta + np.dot(alpha, x)
        eps_k = matrix_coef * vec_norm(x_k - x)
        x = x_k
        iter_count += 1

    return x, iter_count


def zeidel_solve(a: np.ndarray, b: np.ndarray, eps: float) -> tuple[np.ndarray, int]:
    n = len(a)
    alpha, beta = jacobi_cast(a, b)
    eps_k = 1
    c = np.zeros((n, n), dtype=float)
    for i in range(n):
        for j in range(i, n):
            c[i][j] = alpha[i][j]

    matrix_coef = mat_norm(a)/(1-mat_norm(c))
    x = beta.copy()
    iter_count = 0
    while eps < eps_k:
        x_k = zeidel_iteration(x, alpha, beta)
        eps_k = matrix_coef * vec_norm(x_k - x)
        x = x_k
        iter_count += 1

    return x, iter_count


def main():
    n, eps = map(float, input().split())
    a = np.array([list(map(int, input().split())) for _ in range(int(n))])
    b = np.array(list(map(int, input().split())))

    simple_iter_result, simple_iter_count = simple_iterations_solve(a, b, eps)
    print("simple iterations:")
    print(f"\tx: {simple_iter_result}")
    print(f"\titer count: {simple_iter_count}\n")

    zeidel_result, zeidel_count = zeidel_solve(a, b, eps)
    print("zeidel:")
    print(f"\tx: {zeidel_result}")
    print(f"\titer count: {zeidel_count}")


if __name__ == "__main__":
    main()

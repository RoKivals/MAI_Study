import numpy as np
import math


def sign(a: float, eps: float) -> int:
    if a > eps:
        return 1
    elif a < eps:
        return -1
    return 0


def vec_norm(v: np.ndarray) -> float:
    return math.sqrt(sum(map(lambda x: x**2, v)))


def complex_solve(a11: float, a12: float, a21: float, a22: float, eps: float) -> tuple[complex | None, complex | None]:
    a = 1
    b = -a11 - a22
    c = a11 * a22 - a12 * a21
    d = b ** 2 - 4 * a * c
    if d > eps:
        return None, None
    d_c = complex(0, math.sqrt(-d))
    x1 = (-b + d_c) / (2*a)
    x2 = (-b - d_c) / (2*a)
    return x1, x2


def check_eps_for_float(m: np.ndarray, eps: float) -> bool:
    for j in range(len(m)):
        if math.sqrt(sum(map(lambda x: x**2, m[j+2:, j]))) > eps:
            return True
    return False


def check_eps_for_complex(v_cur: np.ndarray, v_prev: np.ndarray, eps: float) -> bool:
    for i in range(len(v_cur)):
        if abs(v_cur[i] - v_prev[i]) > eps:
            return True
    return False


def calc_eigen(a: np.ndarray, eps: float) -> np.ndarray:
    n = len(a)
    v = np.ndarray(n, dtype=complex)
    i = 0
    while i < n - 1:
        x1, x2 = complex_solve(a[i, i], a[i, i + 1], a[i + 1, i], a[i + 1, i + 1], eps)
        if x1 is not None:
            v[i], v[i+1] = x1, x2
            i += 2
        else:
            v[i] = a[i, i]
            i += 1
    if i == n - 1:
        v[i] = a[i, i]

    return v


def householder(b: np.ndarray, i: int, eps: float) -> np.ndarray:
    n = len(b)
    v = b.copy()
    v[i] += sign(b[i], eps) * vec_norm(b)
    h = np.eye(n) - (2 / np.dot(v.transpose(), v)) * np.outer(v, v.transpose())
    return h


def qr_decompose(a: np.ndarray, eps: float) -> tuple[np.ndarray, np.ndarray]:
    n = len(a)
    q = np.eye(n)
    r = a.copy()
    for i in range(n-1):
        b = np.zeros(n)
        b[i:] = r[i:, i]
        h = householder(b, i, eps)
        q = np.dot(q, h)
        r = np.dot(h, r)
    return q, r


def qr_algorithm(a: np.ndarray, eps: float) -> tuple[np.ndarray, int]:
    n = len(a)
    iter_count = 0
    a_k = a.copy()
    v_prev, v_cur = np.zeros((n, n)), calc_eigen(a_k, eps)
    while check_eps_for_float(a_k, eps) or check_eps_for_complex(v_cur, v_prev, eps):
        iter_count += 1
        q, r = qr_decompose(a_k, eps)
        a_k = np.dot(r, q)
        v_prev, v_cur = v_cur, calc_eigen(a_k, eps)
    return v_cur, iter_count


def main():
    n, eps = map(float, input().split())
    n = int(n)
    a = np.array([list(map(int, input().split())) for _ in range(n)])

    v, iter_count = qr_algorithm(a, eps)
    print("Собственные значения:")
    for i in range(n):
        print(f" l_{i} = {v[i]}")

    print(f"Количество итераций: {iter_count}")


if __name__ == "__main__":
    main()
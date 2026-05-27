import numpy as np
import math


def mat_norm(a: np.ndarray) -> float:
    res = 0
    n = len(a)
    for i in range(n):
        for j in range(i+1, n):
            res += a[i, j]**2
    return math.sqrt(res)


def get_coors_of_max(a: np.ndarray) -> tuple[int, int]:
    n = len(a)
    i_max = 0
    j_max = 1
    for i in range(n):
        for j in range(n):
            if i == j:
                continue
            if abs(a[i, j]) > abs(a[i_max, j_max]):
                i_max, j_max = i, j
    return i_max, j_max


def calc_phi(a: np.ndarray, i: int, j: int) -> float:
    if a[i, i] == a[j, j]:
        return math.pi / 4
    return 0.5 * math.atan2(2 * a[i, j], a[i, i] - a[j, j])


def get_rotation_matrix(n: int, phi: float, i: int, j: int) -> np.ndarray:
    u = np.eye(n)
    u[i, i] = math.cos(phi)
    u[i, j] = -math.sin(phi)
    u[j, i] = math.sin(phi)
    u[j, j] = math.cos(phi)
    return u


def jacobi_rotations(a: np.ndarray, eps: float) -> tuple[np.ndarray, np.ndarray, int]:
    n = len(a)
    a_k = a.copy()
    u = np.eye(n)
    iter_count = 0
    while mat_norm(a_k) > eps:
        iter_count += 1
        i, j = get_coors_of_max(a_k)
        phi = calc_phi(a_k, i, j)
        u_k = get_rotation_matrix(n, phi, i, j)
        u = np.dot(u, u_k)
        a_k = np.dot(np.dot(np.transpose(u_k), a_k), u_k)
    return a_k, u, iter_count


def main():
    n, eps = map(float, input().split())
    n = int(n)
    a = np.array([list(map(int, input().split())) for _ in range(n)])

    a_k, u, iter_count = jacobi_rotations(a, eps)
    print("Собственные значения: ")
    for i in range(n):
        print(f" l_{i+1} = {a_k[i, i]}")
    print("Собственные векторы: ")
    for i in range(n):
        print(f" x_{i+1} = {u[:, i]}")
    print("Количество итераций:", iter_count)

    print("\nПроверка (ax = lx):")
    for i in range(n):
        print(f"{np.dot(a, u[:, i])} = {a_k[i, i] * u[:, i]}")


if __name__ == "__main__":
    main()
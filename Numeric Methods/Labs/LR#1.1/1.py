import numpy as np


def LU_decompose(A: np.ndarray) -> tuple[np.ndarray, np.ndarray, float, list[tuple[int, int]]]:
    n = len(A)
    swaps = []
    L = np.eye(n, dtype=float)
    U = A.copy()
    for k in range(n):
        max_row_index = k
        for i in range(k+1, n):
            if abs(U[i, k]) > abs(U[max_row_index, k]):
                max_row_index = i
        if k != max_row_index:
            U[max_row_index, :], U[k, :] = U[k, :], U[max_row_index, :].copy()
            L[max_row_index, :], L[k, :] = L[k, :], L[max_row_index, :].copy()
            L[:, max_row_index], L[:, k] = L[:, k], L[:, max_row_index].copy()
            swaps.append((k, max_row_index))

        for i in range(k+1, n):
            mu = U[i, k] / U[k, k]
            L[i, k] = mu
            U[i, :] -= mu * U[k, :]

    det = (-1)**len(swaps)
    for i in range(n):
        det *= U[i, i]
    return L, U, det, swaps


def solve(L: np.ndarray, U: np.ndarray, b: np.ndarray, swaps: list[tuple[int, int]]) -> np.ndarray:
    n = len(b)
    z = np.ndarray(n, dtype=float)
    x = np.ndarray(n, dtype=float)

    for s1, s2 in swaps:
        b[s1], b[s2] = b[s2], b[s1]

    for i in range(n):
        s = b[i]
        for j in range(i):
            s -= z[j] * L[i, j]
        z[i] = s

    for i in range(n-1, -1, -1):
        if U[i][i] == 0:
            continue
        s = z[i]
        for j in range(n-1, i, -1):
            s -= x[j] * U[i][j]
        x[i] = s / U[i][i]
    return x


def invert(L: np.ndarray, U: np.ndarray, swaps: list[tuple[int, int]]) -> np.ndarray:
    n = len(L)
    res = np.ndarray((n, n), dtype=float)
    for i in range(n):
        b = np.zeros(n)
        b[i] = 1.
        x = solve(L, U, b, swaps)
        for j in range(n):
            res[j, i] = x[j]
    return res


def main():
    n = int(input())
    A = np.array([list(map(int, input().split())) for _ in range(n)], dtype=float)
    b = np.array(list(map(int, input().split())))
    L, U, det, swaps = LU_decompose(A)
    x = solve(L, U, b, swaps)
    invert_matrix = invert(L, U, swaps)
    print(f"L:\n{L}")
    print(f"U:\n{U}")
    print(f"L*U:\n{np.dot(L, U)}")
    print(f"x: {x}")
    print(f"det: {det}")
    print(f"invert_matrix:\n{invert_matrix}")


if __name__ == "__main__":
    main()
import numpy as np

from lab1 import lab1_1 as lab1
from utils import polynom_to_string, draw_polynom


def solve_a(x, y, r):
    n = len(x)
    lhs = np.ndarray((n, r+1))
    for i in range(n):
        for j in range(r+1):
            lhs[i, j] = x[i]**j
    lhs_tr = lhs.transpose()
    L, U, _, swaps = lab1.LU_decompose(np.dot(lhs_tr, lhs))
    rhs = np.dot(lhs_tr, y)
    return lab1.solve(L, U, rhs, swaps)


def F(x, a):
    res = 0
    for i in range(len(a)):
        res += a[i] * x[i]**i
    return res


def mse(x, y, a):
    res = 0
    for i in range(len(x)):
        res += (F(x, a) - y[i]) ** 2
    return res


def main():
    x = list(map(float, input().split()))
    y = list(map(float, input().split()))

    a1 = solve_a(x, y, 1)
    mse1 = mse(x, y, a1)
    print("Приближающий многочлен 1-ой степени:", polynom_to_string(a1))
    print("Сумма квадратов ошибок:", mse1)

    a2 = solve_a(x, y, 2)
    mse2 = mse(x, y, a2)
    print("Приближающий многочлен 2-ой степени:", polynom_to_string(a2))
    print("Сумма квадратов ошибок:", mse2)

    draw_polynom(x, y, a1)
    draw_polynom(x, y, a2)


if __name__ == "__main__":
    main()
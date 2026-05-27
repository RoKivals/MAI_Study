import math
from functools import lru_cache

import numpy as np
from numpy.polynomial import polynomial as poly

from utils import polynom_to_string, draw_polynom


def inter_lagrange_polynom(x, y):
    n = len(x)
    res = np.array([0])
    for i in range(n):
        l_i = np.array([1])
        for j in range(n):
            if i == j:
                continue
            l_i = poly.polymul(l_i, [-x[j], 1]) / (x[i] - x[j])
        res = poly.polyadd(res, y[i] * l_i)
    return res


def inter_newton_polynom(x, y):
    @lru_cache
    def f_diff(l, r):
        if l + 1 == r:
            return (y[l] - y[r]) / (x[l] - x[r])
        return (f_diff(l, r-1) - f_diff(l+1, r)) / (x[l] - x[r])

    n = len(x)
    j = 1
    res = np.array(y[0])
    l_i = np.array([-x[0], 1])
    for i in range(1, n):
        res = poly.polyadd(res, f_diff(0, j)*l_i)
        l_i = poly.polymul(l_i, [-x[i], 1])
        j += 1
    return res


def main():
    f = lambda x_: math.atan(x_)
    x_s = -0.5
    x = list(map(float, input().split()))
    y = list(map(f, x))

    lagrange_poly = inter_lagrange_polynom(x, y)
    lagrange_str = polynom_to_string(lagrange_poly)
    lagrange_error_rate = poly.polyval(x_s, lagrange_poly) - f(x_s)
    print(f"»нтерпол€ционный многочлен Ћагранжа: {lagrange_str}")
    print(f"«начение погрешности интерпол€ции: {lagrange_error_rate}")

    newton_poly = inter_newton_polynom(x, y)
    newton_str = polynom_to_string(newton_poly)
    newton_error_rate = poly.polyval(x_s, newton_poly) - f(x_s)
    print(f"»нтерпол€ционный многочлен Ќьютона: {newton_str}")
    print(f"«начение погрешности интерпол€ции: {newton_error_rate}")

    draw_polynom(x, y, lagrange_poly)


if __name__ == "__main__":
    main()
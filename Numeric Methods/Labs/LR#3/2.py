import bisect

import numpy as np
from matplotlib import pyplot as plt
from numpy.polynomial import polynomial as poly

from lab1 import lab1_2 as lab1


def build_spline(x, y):
    n = len(x)
    a = [0]*n
    b = [0]*n
    c = [0]*n
    d = [0]*n

    h = [0]*n
    eq_a = [0]*(n-2)
    eq_b = [0]*(n-2)
    eq_c = [0]*(n-2)
    eq_d = [0]*(n-2)
    for i in range(1, n):
        h[i] = x[i] - x[i-1]
    for i in range(2, n):
        eq_a[i-2] = h[i-1]
        eq_b[i-2] = 2 * (h[i-1] + h[i])
        eq_c[i-2] = h[i]
        eq_d[i-2] = 3 * ((y[i] - y[i-1]) / h[i] - (y[i-1] - y[i-2]) / h[i-1])
    eq_a[0] = 0
    eq_c[-1] = 0
    c_solved = lab1.tridiagonal_solve(eq_a, eq_b, eq_c, eq_d)

    for i in range(2, n):
        c[i] = c_solved[i-2]
    for i in range(1, n):
        a[i] = y[i-1]
    for i in range(1, n-1):
        b[i] = (y[i] - y[i-1]) / h[i] - h[i] * (c[i+1] + 2*c[i]) / 3
        d[i] = (c[i+1] - c[i]) / (3*h[i])
    c[1] = 0
    b[n-1] = (y[n-1] - y[n-2]) / h[n-1] - (2 / 3) * h[n-1] * c[n-1]
    return a, b, c, d


def draw_polynom(x, y, p):
    fig, ax = plt.subplots(1, 1)
    for i in range(1, len(x)):
        plot_x = np.linspace(x[i - 1], x[i], 100)
        plot_y = sum([p[i-1][j]*(plot_x-x[i-1])**j for j in range(len(p[i-1]))])
        ax.plot(plot_x, plot_y, "-r")
    ax.scatter(x, y)
    plt.show()


def main():
    x_s = float(input())
    x = list(map(float, input().split()))
    y = list(map(float, input().split()))

    a, b, c, d = build_spline(x, y)
    splines = [[a[i], b[i], c[i], d[i]] for i in range(1, len(a))]
    index = bisect.bisect_left(x, x_s)
    p = [a[index], b[index], c[index], d[index]]

    print("Сплайны:\n| i |    a    |    b    |    c    |    d    |")
    for i, spl in enumerate(splines):
        print("| {} | {:7.4f} | {:7.4f} | {:7.4f} | {:7.4f} |".format(i, *spl))

    print(f"\nЗначение функции f({x_s}) =", poly.polyval(x_s - x[index-1], p))
    draw_polynom(x, y, splines)


if __name__ == "__main__":
    main()
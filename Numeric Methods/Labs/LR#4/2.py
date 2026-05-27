import math
import matplotlib.pyplot as plt

from lab4.lab4_1 import runge_kutta_method, runge_romberg, max_abs_error
from lab1.lab1_2 import tridiagonal_solve


def shooting_method(f, g, a, b, h, alpha, beta, delta, gamma, y0, y1, eta0, eta1, eps):
    def get_z0(eta):
        return (y0 - alpha * eta) / beta

    while True:
        _, y_s0, z_s0 = runge_kutta_method(f, g, a, b, h, eta0, get_z0(eta0))
        _, y_s1, z_s1 = runge_kutta_method(f, g, a, b, h, eta1, get_z0(eta1))

        phi0 = delta * y_s0[-1] + gamma * z_s0[-1] - y1
        phi1 = delta * y_s1[-1] + gamma * z_s1[-1] - y1

        eta2 = eta1 - (eta1 - eta0) / (phi1 - phi0) * phi1
        x_s2, y_s2, z_s2 = runge_kutta_method(f, g, a, b, h, eta2, get_z0(eta2))
        phi2 = delta * y_s2[-1] + gamma * z_s2[-1] - y1
        if phi2 < eps:
            return x_s2, y_s2, z_s2

        eta0, eta1 = eta1, eta2


def finite_difference_method(f, p, q, l, r, h, alpha, beta, delta, gamma, y0_, y1_):
    n = int((r - l) / h)
    xk = [l + h * i for i in range(n+1)]
    a, b, c, d = [0]*(n+1), [0]*(n+1), [0]*(n+1), [0]*(n+1)
    """
    Подстановка:
     y'0 = (y1 - y0) / h
     y'N = (yN - y(N-1))) / h
    в
     alpha * y0 + beta * y'0 = y0_
     delta * yN + gamma * y'N = y1_
    упрощение уравнения и выбор коэффициентов:
     b[0]*y0 + c[0]*y1 = d[0]
     a[-1]*y(N-1) + b[-1]yN = d[-1]
    """
    b[0] = h * alpha - beta
    c[0] = beta
    d[0] = h * y0_
    a[-1] = -gamma
    b[-1] = h * delta + gamma
    d[-1] = h * y1_
    for i in range(1, n):
        a[i] = 1 - p(xk[i]) * h / 2
        b[i] = -2 + h**2 * q(xk[i])
        c[i] = 1 + p(xk[i]) * h / 2
        d[i] = h**2 * f(xk[i])

    yk = tridiagonal_solve(a, b, c, d)
    return xk, yk


def draw_plots(real_x, real_y, shooting_x, shooting_y, finite_difference_x, finite_difference_y):
    plt.plot(real_x, real_y, label="real")
    plt.plot(shooting_x, shooting_y, label="shooting")
    plt.plot(finite_difference_x, finite_difference_y, label="finite difference")
    plt.legend()
    plt.show()


def main():
    f = lambda x, y, z: z
    fx = lambda x: 0
    g = lambda x, y, z: ((2 * x + 1) * z - (x + 1) * y) / x
    p = lambda x: -(2 * x + 1) / x
    q = lambda x: (x + 1) / x
    real_f = lambda x: math.exp(x) * x ** 2
    """
    Краевые условия 3 рода
    alpha * y(a) + beta * y'(a) = y0
    delta * y(b) + gamma * y'(b) = y1
    """
    a, b = 1, 2
    alpha, beta, y0 = 0, 1, 3*math.e
    delta, gamma, y1 = -2, 1, 0
    h = float(input())
    eps = float(input())
    eta0, eta1 = 1, 0.8

    real_x = [a+i*h for i in range(int((b-a)/h)+1)]
    real_y = list(map(real_f, real_x))

    shooting_x1, shooting_y1, _ = shooting_method(f, g, a, b, h, alpha, beta, delta, gamma, y0, y1, eta0, eta1, eps)
    shooting_x2, shooting_y2, _ = shooting_method(f, g, a, b, h/2, alpha, beta, delta, gamma, y0, y1, eta0, eta1, eps)

    finite_diff_x1, finite_diff_y1 = finite_difference_method(fx, p, q, a, b, h, alpha, beta, delta, gamma, y0, y1)
    finite_diff_x2, finite_diff_y2 = finite_difference_method(fx, p, q, a, b, h/2, alpha, beta, delta, gamma, y0, y1)

    shooting_er_rr = runge_romberg(shooting_y1, shooting_y2, 2, 4)
    finite_diff_er_rr = runge_romberg(finite_diff_y1, finite_diff_y2, 2, 4)

    shooting_er_ma = max_abs_error(shooting_y1, real_y)
    finite_diff_er_ma = max_abs_error(finite_diff_y1, real_y)

    print(f"""
Погрешность по Рунге-Ромбергу:
    Метода стрельбы: {shooting_er_rr}
    Конечно-разностного метода: {finite_diff_er_rr}
Погрешность путём сравнения с точным решением:
    Метода стрельбы: {shooting_er_ma}
    Конечно-разностного метода: {finite_diff_er_ma}
""")

    draw_plots(real_x, real_y, shooting_x1, shooting_y1, finite_diff_x1, finite_diff_y1)


if __name__ == "__main__":
    main()
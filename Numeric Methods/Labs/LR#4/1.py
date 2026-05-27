import math
import matplotlib.pyplot as plt


def euler_method(f, g, l, r, h, y0, z0):
    n = int((r - l) / h) + 1
    x = [0] * n
    y = [0] * n
    z = [0] * n
    x[0] = l
    y[0] = y0
    z[0] = z0
    for i in range(n-1):
        x[i + 1] = x[i] + h
        y[i + 1] = y[i] + h * f(x[i], y[i], z[i])
        z[i + 1] = z[i] + h * g(x[i], y[i], z[i])

    return x, y, z


def runge_kutta_method(f, g, l, r, h, y0, z0):
    n = int((r - l) / h) + 1
    x = [0] * n
    y = [0] * n
    z = [0] * n
    x[0] = l
    y[0] = y0
    z[0] = z0
    for i in range(n-1):
        K1 = h * f(x[i], y[i], z[i])
        L1 = h * g(x[i], y[i], z[i])
        K2 = h * f(x[i] + h / 2, y[i] + K1 / 2, z[i] + L1 / 2)
        L2 = h * g(x[i] + h / 2, y[i] + K1 / 2, z[i] + L1 / 2)
        K3 = h * f(x[i] + h / 2, y[i] + K2 / 2, z[i] + L2 / 2)
        L3 = h * g(x[i] + h / 2, y[i] + K2 / 2, z[i] + L2 / 2)
        K4 = h * f(x[i] + h, y[i] + K3, z[i] + L3)
        L4 = h * g(x[i] + h, y[i] + K3, z[i] + L3)
        dy = (K1 + 2 * K2 + 2 * K3 + K4) / 6
        dz = (L1 + 2 * L2 + 2 * L3 + L4) / 6
        x[i + 1] = x[i] + h
        y[i + 1] = y[i] + dy
        z[i + 1] = z[i] + dz
    return x, y, z


def adams_method(f, g, l, r, h, y0, z0):
    n = int((r - l) / h) + 1
    x_start, y_start, z_start = runge_kutta_method(f, g, l, l + 4 * h, h, y0, z0)
    x = [0] * n
    y = [0] * n
    z = [0] * n
    x[:4] = x_start
    y[:4] = y_start
    z[:4] = z_start
    for i in range(3, n-1):
        x[i + 1] = x[i] + h

        y[i + 1] = y[i] + h / 24 * (55 * f(x[i], y[i], z[i]) -
                                    59 * f(x[i - 1], y[i - 1], z[i - 1]) +
                                    37 * f(x[i - 2], y[i - 2], z[i - 2]) -
                                    9 * f(x[i - 3], y[i - 3], z[i - 3]))
        z[i + 1] = z[i] + h / 24 * (55 * g(x[i], y[i], z[i]) -
                                    59 * g(x[i - 1], y[i - 1], z[i - 1]) +
                                    37 * g(x[i - 2], y[i - 2], z[i - 2]) -
                                    9 * g(x[i - 3], y[i - 3], z[i - 3]))

    return x, y, z


def runge_romberg(y1, y2, k, p):
    res = 0
    c = 1 / (k ** p + 1)
    for i in range(len(y1)):
        res = max(res, c * abs(y1[i] - y2[i*k]))
    return res


def max_abs_error(y1, y2):
    res = 0
    for i in range(len(y1)):
        res = max(res, abs(y1[i] - y2[i]))
    return res


def draw_plots(real_x, real_y, euler_x, euler_y, runge_kutta_x, runge_kutta_y, adams_x, adams_y):
    plt.plot(real_x, real_y, label="real")
    plt.plot(euler_x, euler_y, label="euler")
    plt.plot(runge_kutta_x, runge_kutta_y, label="runge")
    plt.plot(adams_x, adams_y, label="adams")
    plt.legend()
    plt.show()


def main():
    f = lambda x, y, z: z
    g = lambda x, y, z: (x*(2*x+1)*z - (2*x+1)*y) / (x**2 * (x+1))
    real_f = lambda x: x**2 + x + x*math.log(x)
    y0 = 2
    z0 = 4
    l, r = int(input()), int(input())
    h = 0.1

    real_x = [l+i*h for i in range(int((r-l)/h)+1)]
    real_y = list(map(real_f, real_x))

    euler_x1, euler_y1, _ = euler_method(f, g, l, r, h, y0, z0)
    runge_kutta_x1, runge_kutta_y1, _ = runge_kutta_method(f, g, l, r, h, y0, z0)
    adams_x1, adams_y1, _ = adams_method(f, g, l, r, h, y0, z0)

    euler_x2, euler_y2, _ = euler_method(f, g, l, r, h/2, y0, z0)
    runge_kutta_x2, runge_kutta_y2, _ = runge_kutta_method(f, g, l, r, h/2, y0, z0)
    adams_x2, adams_y2, _ = adams_method(f, g, l, r, h/2, y0, z0)

    euler_er_rr = runge_romberg(euler_y1, euler_y2, 2, 1)
    runge_kutta_er_rr = runge_romberg(runge_kutta_y1, runge_kutta_y2, 2, 4)
    adams_er_rr = runge_romberg(adams_y1, adams_y2, 2, 4)

    euler_er_ma = max_abs_error(euler_y1, real_y)
    runge_kutta_er_ma = max_abs_error(runge_kutta_y1, real_y)
    adams_er_ma = max_abs_error(adams_y1, real_y)

    print(f"""
Погрешность по Рунге-Ромбергу:
    Метода Эйлера: {euler_er_rr}
    Метода Рунге-Кутты: {runge_kutta_er_rr}
    Метода Адамса: {adams_er_rr}
Погрешность путём сравнения с точным решением:
    Метода Эйлера: {euler_er_ma}
    Метода Рунге-Кутты: {runge_kutta_er_ma}
    Метода Адамса: {adams_er_ma}
""")

    draw_plots(real_x, real_y, euler_x1, euler_y1, runge_kutta_x1, runge_kutta_y1, adams_x1, adams_y1)


if __name__ == "__main__":
    main()
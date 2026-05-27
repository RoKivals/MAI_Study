import math


def simple_iterations(phi, phi_d, eps, start_a, start_b):
    q = abs(phi_d(start_a))
    if q > 1 or abs(phi_d(start_b)) > q:
        q = abs(phi_d(start_b))
    iter_count = 0
    coef = q / (1 - q)
    x_k = start_b
    dx = 10e9
    while eps < coef * dx:
        x_k_next = phi(x_k)
        dx = abs(x_k_next - x_k)
        x_k = x_k_next
        iter_count += 1
    return x_k, iter_count


def newton_method(f, f_d, f_d2, eps, x0):
    if f(x0) * f_d2(x0) <= 0:
        raise ValueError("f(x0) * f_d2(x0) must be > 0")

    iter_count = 0
    x_k = x0
    dx = 10e9
    while eps < dx:
        x_k_next = x_k - f(x_k)/f_d(x_k)
        dx = abs(x_k_next - x_k)
        x_k = x_k_next
        iter_count += 1
    return x_k, iter_count


def main():
    eps = float(input())
    f = lambda x: math.sin(x) - 2 * x ** 2 + 0.5
    f_d = lambda x: math.cos(x) - 4*x
    f_d2 = lambda x: -math.sin(x) - 4
    phi = lambda x: math.sqrt((math.sin(x) + 0.5) / 2)
    phi_d = lambda x: math.cos(x) / (2 * math.sqrt(2 * math.sin(x) + 1))
    start_a, start_b = 0, 1

    x, iter_count = simple_iterations(phi, phi_d, eps, start_a, start_b)
    print(f"simple iterations: x={x} iter_count={iter_count}")

    try:
        x, iter_count = newton_method(f, f_d, f_d2, eps, start_a)
    except ValueError:
        x, iter_count = newton_method(f, f_d, f_d2, eps, start_b)

    print(f"newton method: x={x} iter_count={iter_count}")


if __name__ == "__main__":
    main()
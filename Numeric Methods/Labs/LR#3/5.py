def rect_method(f, a, b, h):
    x_prev = a
    x_cur = a + h
    res = 0
    for _ in range(1, int((b-a)//h)+1):
        res += f((x_prev + x_cur) / 2)
        x_prev = x_cur
        x_cur += h
    return res * h


def trapezoid_method(f, a, b, h):
    x_prev = a
    x_cur = a + h
    res = 0
    for _ in range(1, int((b-a)//h)+1):
        res += f(x_prev) + f(x_cur)
        x_prev = x_cur
        x_cur += h
    return res * h / 2


def simpson_method(f, a, b, h):
    h2 = h/2
    x_cur = a + h2
    res = f(a) + f(b)
    for i in range(1, int((b-a)/h2)):
        res += 2*f(x_cur) if i % 2 == 0 else 4*f(x_cur)
        x_cur += h2
    return res * h2 / 3


def runge_romberg(Fh, Fkh, k, p):
    return (Fh - Fkh) / (k**p - 1)


def main():
    x0, xk, h1, h2 = map(float, input().split())
    y = lambda x: x**2 / (x**2 + 16)

    rect1 = rect_method(y, x0, xk, h1)
    trap1 = trapezoid_method(y, x0, xk, h1)
    simp1 = simpson_method(y, x0, xk, h1)

    rect2 = rect_method(y, x0, xk, h2)
    trap2 = trapezoid_method(y, x0, xk, h2)
    simp2 = simpson_method(y, x0, xk, h2)

    rect_er = runge_romberg(rect1, rect2, h2 / h1, 2)
    trap_er = runge_romberg(trap1, trap2, h2 / h1, 2)
    simp_er = runge_romberg(simp1, simp2, h2 / h1, 4)

    print(f"""
Значение метода прямоугольника с шагом {h1}: {rect1}
Значение метода трапеций с шагом {h1}: {trap1}
Значение метода Симпсона с шагом {h1}: {simp1}

Значение метода прямоугольника с шагом {h2}: {rect2}
Значение метода трапеций с шагом {h2}: {trap2}
Значение метода Симпсона с шагом {h2}: {simp2}

Погрешность метода прямоугольника: {rect_er}
Погрешность метода трапеций: {trap_er}
Погрешность метода Симпсона: {simp_er}
"""
          )


if __name__ == "__main__":
    main()
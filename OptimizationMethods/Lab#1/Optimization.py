import matplotlib.pyplot as plt
import numpy as np
import sympy as sp
import math


class RastriginFunc:
    def __init__(self):
        self.x1, self.x2 = sp.symbols('x1, x2')
        self.f = 100 + (self.x1 ** 2 - 10 * sp.cos(2 * 3.14 * self.x1)) ** 2 + \
            (self.x2 ** 2 - 10 * sp.cos(2 * 3.14 * self.x2)) ** 2

    def orig_func(self, x: np.array):
        return self.f.subs({self.x1:x[0], self.x2:x[1]})

    def grad_vector(self, x: np.array):
        vector = [sp.diff(self.f, self.x1),
                           sp.diff(self.f, self.x2)]
        values = {self.x1:x[0], self.x2:x[1]}

        result = [vector[0].subs(values), vector[1].subs(values)]

        return np.array(result, dtype=np.float64)
    
    def hesse_matrix(self, x: np.array):
        h11 = sp.diff(sp.diff(self.f, self.x1), self.x1)
        h12 = sp.diff(sp.diff(self.f, self.x1), self.x2)
        h21 = sp.diff(sp.diff(self.f, self.x2), self.x1)
        h22 = sp.diff(sp.diff(self.f, self.x2), self.x2)
        values = {self.x1:x[0], self.x2:x[1]}

        return np.array([
            [h11.subs(values), h12.subs(values)],
            [h21.subs(values), h22.subs(values)]
        ], dtype=np.float64)

class Func():
    def __init__(self, a, b, n, m):
        self.a = a
        self.b = b
        self.n = n
        self.m = m

    def orig_func(self, x: np.array):
        return self.a * (x[0] - int(self.n/2)) ** 2 + self.b * (x[1] - self.m) ** 2

    def grad_vector(self, x: np.array):
        return np.array([2 * self.a * (x[0] - int(self.n/2)), 2 * self.b * (x[1] - self.m)])

    def hesse_matrix(self, x: np.array):
        return np.array([
            [2 * self.a, 0],
            [0, 2 * self.b]
        ])

class MarquardMethod:
    '''
    initialPoint - начальная точкa (x0)

    '''

    def __init__(self, function: Func, initialPoint, eps1=None, max_iterations=100):

        self.epsilon1 = eps1
        self.function = function
        self.x = initialPoint
        self.y = self.function.orig_func(initialPoint)
        self.iterations = 0
        self.max_iterations = max_iterations

        hasse = self.function.hesse_matrix(self.x)
        self.positive_digit = hasse.max() * 50

    @staticmethod
    def mod_grad(gradient: np.array):
        return math.sqrt(sum([elem ** 2 for elem in gradient]))

    def next_point(self, hesse: np.array, gradient: np.ndarray):
        unit_matrix = np.eye(hesse.shape[0])
        temp_matrix = np.array(
            hesse + self.positive_digit * unit_matrix, dtype=np.float64)
        temp_matrix = np.linalg.inv(temp_matrix)
        difference = temp_matrix @ gradient

        new_x = self.x - difference
        return new_x

    def method(self):
        result = np.array(self.x, dtype=np.float64)
        new_x = self.x
        # True - 3 step, False - 7 step
        step_start = True
        hesse, grad_f = None, None

        while True:

            if step_start:

                grad_f = self.function.grad_vector(self.x)
                if self.mod_grad(grad_f) <= self.epsilon1:
                    # np.append(result, self.x)
                    break

                if self.iterations >= self.max_iterations:
                    # np.append(result, self.x)
                    break

                hesse = self.function.hesse_matrix(self.x)
            # 7
            new_x = self.next_point(hesse, grad_f)

            if self.function.orig_func(new_x) < self.function.orig_func(self.x):
                self.iterations += 1
                self.positive_digit /= 2
                step_start = True
            else:
                self.positive_digit *= 2
                step_start = False

            self.x = new_x
            result = np.vstack([result, self.x])

        return result

def marquard_main():
    print("Метод Марквардта для уравнения:")
    print("f(x, y) = a(x - n/2)^2 + b(y-m)^2")
    a, b, n, m = list(map(int, input("Введите коэффиценты a b n m: ").split()))
    x0 = list(map(int, input("Введите координаты начального приближения (2d): ").split()))
    x0 = np.array(x0)
    eps1 = float(input("Введите точность измерения (eps1): "))

    f = Func(a, b, n, m)
    marquard = MarquardMethod(f, x0, eps1, 10)
    res = marquard.method()

    print('Минимум:')
    print(res[-1])
    print('Итерации:')
    for idx, point in enumerate(res):
        print(f"Iteration #{idx}: {point}")


    final_dot = res[-1]
    # Создаем данные
    x = np.linspace(final_dot[0] - 10, final_dot[0] + 10, 100)
    y = np.linspace(final_dot[1] - 10, final_dot[1] + 10, 100)

    x, y = np.meshgrid(x, y)
    z = a * (x - int(n / 2)) ** 2 + b * (y - m) ** 2

    # Создаем фигуру и оси
    fig = plt.figure()
    ax = fig.add_subplot(111, projection='3d')

    # Построение поверхности
    ax.plot_surface(x, y, z, cmap='viridis', alpha=0.7)
    lst_x = np.array([i[0] for i in res])
    lst_y = np.array([i[1] for i in res])

    lst_z = np.array([f.orig_func(i) for i in res])
    ax.plot(lst_x, lst_y, lst_z, color='red',)

    # Настройка меток осей
    ax.set_xlabel('X ось')
    ax.set_ylabel('Y ось')
    ax.set_zlabel('Z ось')

    # Показ графика
    plt.show()

def rastrigin_main():
    print("Метод Марквардта для уравнения Растригина:")
    r = RastriginFunc()

    x0 = list(map(int, input("Введите координаты начального приближения (2d): ").split()))
    x0 = np.array(x0)
    eps1 = float(input("Введите точность измерения (eps1): "))

    marquard = MarquardMethod(r, x0, eps1, 10)
    res = marquard.method()

    print('Минимум:')
    print(res[-1])
    print('Итерации:')
    for idx, point in enumerate(res):
        print(f"Iteration #{idx}: {point}")


    final_dot = res[-1]
    # Создаем данные
    x = np.linspace(final_dot[0] - 10, final_dot[0] + 10, 100)
    y = np.linspace(final_dot[1] - 10, final_dot[1] + 10, 100)

    x, y = np.meshgrid(x, y)
    z = 100 + (x ** 2 - 10 * np.cos(2 * 3.14 * x)) ** 2 + \
            (y ** 2 - 10 * np.cos(2 * 3.14 * y)) ** 2

    # Создаем фигуру и оси
    fig = plt.figure()
    ax = fig.add_subplot(111, projection='3d')

    # Построение поверхности
    ax.plot_surface(x, y, z, cmap='viridis', alpha=0.7)
    lst_x = np.array([i[0] for i in res])
    lst_y = np.array([i[1] for i in res])

    lst_z = np.array([r.orig_func(i) for i in res])
    ax.plot(lst_x, lst_y, lst_z, color='red',)

    # Настройка меток осей
    ax.set_xlabel('X ось')
    ax.set_ylabel('Y ось')
    ax.set_zlabel('Z ось')

    # Показ графика
    plt.show()

if __name__ == '__main__':
    marquard_main()
    rastrigin_main()
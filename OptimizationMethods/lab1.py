import matplotlib.pyplot as plt
import numpy as np
import math

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

    def __init__(self, function: Func, initialPoint, eps1=None, max_iterations=10, positive_0=10**4):

        self.epsilon1 = eps1
        self.function = function
        self.x = initialPoint
        self.y = self.function.orig_func(initialPoint)
        self.iterations = 0
        self.max_iterations = max_iterations
        self.positive_digit = positive_0

    @staticmethod
    def mod_grad(gradient: np.array):
        return math.sqrt(sum([elem ** 2 for elem in gradient]))
    
    def next_point(self, hesse: np.array, gradient: np.ndarray):
        unit_matrix = np.eye(hesse.shape[0])
        temp_matrix = hesse + self.positive_digit * unit_matrix
        temp_matrix = np.linalg.inv(temp_matrix)
        difference = temp_matrix * gradient

        new_x = self.x - difference * np.transpose(gradient)
        return new_x

    def method(self):
        result = np.array(x0)
        new_x = x0
        # True - 3 step, False - 7 step
        step_start = False
        
        while True:
            if step_start:
                
                grad_f = self.function.grad_vector(self.x)
                if self.mod_grad(grad_f) <= self.epsilon1:
                    np.append(result, self.x)
                    break
                
                if self.iterations >= self.max_iterations:
                    np.append(result, self.x)
                    break
                hesse = self.function.hesse_matrix(self.x)
                
            #7
            new_x = self.next_point(hesse, grad_f)

            if self.function.orig_func(new_x) < self.function.orig_func(self.x):
                self.iterations += 1
                self.positive_digit /= 2
                step_start = True
            else:
                self.positive_digit *= 2
                step_start = False

        return result

a, b, c, m = list(map(int, input("Введите коэффиценты a b c m: ").split()))
x0 = list(map(int, input("Введите координаты начального приближения (2d): ").split()))
x0 = np.array(x0)
eps1 = int(input("Введите точность измерения (eps1)"))

f = Func(1, 1, 10, 5)
m = MarquardMethod(f, x0, 0.001, 0.001)
m.method()

print('Минимум:')
print(gaus_zeidel(Func(a, b, n ,m), x, e1, e2))
print('Итерации:')
for i in range(len(lst_vec)):
    print(f'{i}: {lst_vec[i]}')


#from mpl_toolkits.mplot3d import Axes3D

# Создаем данные
x = np.linspace(-10, 10, 100)
y = np.linspace(-10, 10, 100)
x, y = np.meshgrid(x, y)
z = a * (x - int(n / 2))**2 + b * (y - m)**2

# f(x) = a * (x1 - [n/2]) ^ 2 + b * (x2 - m)^2
# Создаем фигуру и оси
fig = plt.figure()
ax = fig.add_subplot(111, projection='3d')

# Построение поверхности
ax.plot_surface(x, y, z, cmap='viridis', alpha=0.7)
lst_x = np.array([i[0] for i in lst_vec])
lst_y = np.array([i[1] for i in lst_vec])
f = Func(a, b, n ,m)
lst_z = np.array([f.funcMyVar(i) for i in lst_vec])
ax.plot(lst_x, lst_y, lst_z, color='red',)

# Настройка меток осей
ax.set_xlabel('X ось')
ax.set_ylabel('Y ось')
ax.set_zlabel('Z ось')

# Показ графика
plt.show()
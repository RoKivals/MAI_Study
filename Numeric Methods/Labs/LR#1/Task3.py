import math
import copy
from Matrix import QuadMatrix, TripleDiagMatrix, Matrix


def prepare_SLE(A: QuadMatrix, b: list):
    size = len(A)

    result_matrix = QuadMatrix(size, 0)
    result_coef = Matrix(1, size, 0)

    for row in range(size):
        for col in range(size):
            if row == col:
                result_matrix[row, col] = 0
            else:
                result_matrix[row, col] = - A[row, col] / A[row, row]

        result_coef[row] = b[row] / A[row, row]

    return result_matrix, result_coef.transpose()

def stop_condition(X: Matrix, epsilon: float):
    norm = sum(elem ** 2 for row in X for elem in row)
    return math.sqrt(norm) < epsilon


def Iterative_Solution_of_SLE(A: QuadMatrix, b: list, epsilon: float):
    '''
    Метод простой итерации:
    Ax = b
    x = Bx + g

    A -> M - N (Метод Якоби)
    M = D, D - Diagonal matrix

    Условие оставновки: норма матрицы разности двух приближений меньше заданной точности (epsilon).
    '''

    B, g = prepare_SLE(A, b)
    x = copy.copy(g)
    flag = False
    iterations_cnt = 0

    while not flag:
        prev_x = copy.copy(x)
        x = B @ prev_x + g
        iterations_cnt += 1
        flag = stop_condition(prev_x - x, epsilon)
    return x, iterations_cnt
        
def Seidel_Solution_of_SLE(A: QuadMatrix, b: list, epsilon: float):
    '''
    Модификация метода Якоби. 
    При вычислении очередного приближения x, используются все вычисленные ранее приближения.
    Условие остановки: как в метода Якоби
    '''
    def Seidel_new_x(B: QuadMatrix, x: Matrix):
        size = B.size()
        res = copy.copy(x)
        for row in range(size):
            for col in range(size):
                res[row, 0] += B[row, col] * x[col, 0]

        return res

    B, g = prepare_SLE(A, b)
    x = copy.copy(g)
    flag = False
    iterations_cnt = 0

    while not flag:
        prev_x = copy.copy(x)
        x = B @ prev_x + g
        x = Seidel_new_x(B, x)
        iterations_cnt += 1
        flag = stop_condition(prev_x - x, epsilon)
    return x, iterations_cnt

def main():
    # Iterative method
    size = int(input("Введите размер матрицы: "))
    print('Введите значения матрицы:\n')
    data = [[] for _ in range(size)]
    for row in range(size):
        data[row] = list(map(int, input().split()))
    A = TripleDiagMatrix(size, data)
    
    b_coef = list(map(int, input("Введите коэффициенты: ").split()))
    eps = float(input('Введите эпсилон: '))

    print('Метод итерации (Якоби):')
    x, cnt_iterative = Iterative_Solution_of_SLE(A, b_coef, eps)
    print('Solution:', x)
    print(f'Количество итераций: {cnt_iterative}')

    print('Метод итерации (Сейдел):')
    x_seidel, cnt_seidel = Seidel_Solution_of_SLE(A, b_coef, eps)
    print('Solution:', x_seidel)
    print(f'Количество итераций: {cnt_seidel}')

if __name__ == '__main__':
    main()

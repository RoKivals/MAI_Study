from Matrix import TripleDiagMatrix


def Solution_of_SLE(matrix: TripleDiagMatrix, b: list):
    '''
        Ax = b, A - trdiagonal matrix, b - vector
        Ai * xi-1 + Bi * xi + Ci * xi+1 = bi
        Ai - diag left to main diagonal
        Bi - main diagonal
        Ci - diag right to main diagonal
    '''
    size = len(matrix)
    alpha, beta = [0 for _ in range(size)], [0 for _ in range(size)]

    # Вычисление прогоночных коэффициентов
    alpha[0] = - matrix[0, 1] / matrix[0, 0]
    beta[0] = b[0] / matrix[0, 0]

    for i in range(1, size - 1):
        alpha[i] = - matrix[i, i + 1] / \
            (matrix[i, i] + matrix[i, i - 1] * alpha[i - 1])
        
        beta[i] = (b[i] - matrix[i, i - 1] * beta[i - 1]) / \
            (matrix[i, i] + matrix[i, i - 1] * alpha[i - 1])

    alpha[size - 1] = 0
    beta[size - 1] = (b[-1] - matrix[size - 1, -2] * beta[size - 2]) \
        / (matrix[size - 1, -2] * alpha[size - 2] + matrix[size - 1, -1])

    # Вычисление решений (обратная прогонка)
    x = [0 for _ in range(size)]
    x[-1] = beta[-1]

    for i in range(size - 1, 0, -1):
        x[i - 1] = alpha[i] * x[i] + beta[i]

    return x


def main():
    size = int(input("Введите размер матрицы: "))
    print('Введите значения матрицы (без нулей вне диагонали): \n')
    data = [[] for _ in range(size)]
    for row in range(size):
        data[row] = list(map(int, input().split()))

    A = TripleDiagMatrix(size, data)
    b_coef = list(map(int, input("Введите коэффициенты: ")))

    x = Solution_of_SLE(A, b_coef)

    print('Solution:', x)

if __name__ == '__main__':
    main()

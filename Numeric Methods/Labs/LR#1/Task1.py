from Matrix import QuadMatrix, TriangleMatrix, Matrix


def LU_decomposition(A: QuadMatrix):
    size = len(A)
    L = TriangleMatrix(size, 1)
    U = TriangleMatrix(size, 1)

    for row in range(size):
        for col in range(size):
            if row <= col:
                summa = 0
                for k in range(row):
                    summa += L[row, col] * U[k, col]
                U[row, col] = A[row, col] - summa

            if row > col:
                summa = 0
                for k in range(col):
                    summa += L[row, k] * U[k, col]
                L[row, col] = (A[row, col] - summa) / U[col, col]
    return L, U


def Solution_of_SLE(A: QuadMatrix, b: list):
    '''
        Ax = b
        A = L * U
        L * U * x = b
    '''

    def substitution(matrix: TriangleMatrix, coef: list, Reversed: bool = True):
        range_ = range(0, matrix.size)

        if Reversed == False:
            range_ = reversed(range_)

        result = [0 for _ in range(matrix.size)]
        result[0] = coef[0]

        for i in range(1, matrix.size):
            summa = 0
            for j in range(i):
                summa += matrix[i, j] * coef[j]
            result[i] = coef[i] - summa

        return result

    L, U = LU_decomposition(A)
    # Step 1: Ly = b
    y = substitution(L, b)

    # Step 2: Ux = y
    x = substitution(U, y, False)

    return x


def det(A: QuadMatrix) -> int:
    L, U = LU_decomposition(A)
    return L.determination() * U.determination()


def inverse_matrix(A: QuadMatrix):
    L, U = LU_decomposition(A)

    size = len(L)

    unit_matrix = QuadMatrix(size)
    unit_matrix.change_diag(1)

    result = QuadMatrix(size)
    for row in unit_matrix:
        inverse_row = Solution_of_SLE(L, U, row)
        result.add_col(inverse_row)
    return result


def main():
    size = int(input("Введите размер матрицы: "))
    print('Введите значения матрицы:\n')
    A = QuadMatrix(size)
    data = [[] for _ in range(size)]
    for row in range(size):
        data[row] = list(map(int, input().split()))

    A.fill_data(data)

    b_coef = list(
        map(int, input("Введите значения свободных коэффицентов: ").split()))

    print("LU - разложение:")
    L, U = LU_decomposition(A)
    print(f'{L=}')
    print(f'{U=}')

    print("System solution")
    x = Solution_of_SLE(L, U, b_coef)
    print('x:', x)

    print("det A =", det(L, U))

    print("A^(-1)")
    print(inverse_matrix(A))


if __name__ == '__main__':
    main()

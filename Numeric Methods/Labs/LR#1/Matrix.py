import copy
import math
from typing import List



class Matrix:
    def __init__(self, rows: int, cols: int, default_value=0) -> None:
        self.data = [[default_value for _ in range(cols)] for _ in range(rows)]
        self.rows = rows
        self.cols = cols

    def __repr__(self) -> str:
        res = ""
        res = res + f"Matrix ({self.rows}, {self.cols})\n"
        for row in range(self.rows):
            res = res + '\n'
            res = res + " ".join(str(self.data[row][col])
                                 for col in range(self.cols))
        return res

    def __iter__(self):
        return self.data.__iter__()

    def __getitem__(self, pos):
        if len(pos) == 2:
            row, col = pos
            return self.data[row][col]

        if len(pos) == 1:
            row = pos[0]
            return self.data[row]

        raise TypeError(f"Wrong position: {pos}")

    def __setitem__(self, pos, value):
        try:
            row, col = pos
        except TypeError:
            row, col = 0, pos

        self.data[row][col] = value

    def _size(self):
        return (self.rows, self.cols)

    def __add__(self, other):
        if self._size() != other._size():
            raise ValueError(
                f"Matrices have different sizes: {self.size}!= {other.size}")

        result = Matrix(self.rows, self.cols)
        for row in range(self.rows):
            for col in range(self.cols):
                result[row, col] = self[row, col] + other[row, col]
        return result

    def __sub__(self, other):
        if self._size() != other._size():
            raise ValueError(
                f"Matrices have different sizes: {self.size}!= {other.size}")

        result = Matrix(self.rows, self.cols)
        for row in range(self.rows):
            for col in range(self.cols):
                result[row, col] = self[row, col] - other[row, col]

        return result

    def __matmul__(self, other):
        if self.cols != other.rows:
            raise ValueError(
                f"Matrices have different sizes: {self.cols}!= {other.rows}")

        result = Matrix(self.rows, other.cols)
        for row in range(result.rows):
            for col in range(result.cols):
                elem = 0
                for k in range(self.cols):
                    elem += self[row, k] * other[k, col]
                result[row, col] = elem
        return result

    def _is_vector(self):
        return self.rows == 1

    def fill_data(self, data: list):
        self.rows = len(data)
        self.cols = len(data[0])
        self.data = copy.deepcopy(data)

    def add_row(self, values: list):
        self.rows += 1
        self.data.append(values)

    def add_col(self, values: list):
        self.cols += 1
        for row, elem in enumerate(values):
            self.data[row].append(elem)

    def transpose(self):
        result = Matrix(self.cols, self.rows)
        for row in range(self.cols):
            for col in range(self.rows):
                result[row, col] = self[col, row]

        return result

    def norm(self):
        row_max = max([sum(row) for row in self])
        col_max = max([sum(col) for col in self.transpose()])

        square_norm = math.sqrt(sum(elem ** 2 for row in self for elem in row))

        return max(square_norm, row_max, col_max)


class QuadMatrix(Matrix):
    def __init__(self, size: int, default_value=0) -> None:
        super().__init__(size, size, default_value)
        self.size = size

    def __len__(self) -> int:
        return self.size

    @property
    def _size(self):
        return (self.rows, self.cols)

    def change_diag(self, value):
        for order in range(self.size):
            self[order, order] = value

    def add_col(self, values: list):
        pass

    def add_row(self, values: list):
        pass


class TriangleMatrix(QuadMatrix):
    def __init__(self, size: int, diag_value, default_value=0, ) -> None:
        super().__init__(size, default_value)
        for row in range(self.size):
            for col in range(self.size):
                if row == col:
                    self[row, col] = diag_value

    def determination(self) -> int:
        result = 1
        for idx in range(self.size):
            result *= self[idx, idx]
        return result


class DiagMatrix(QuadMatrix):
    pass


class TripleDiagMatrix(QuadMatrix):
    def __init__(self, size: int, diag_values: list) -> None:
        super().__init__(size, 0)
        self[0, 0], self[0, 1] = diag_values[0][0], diag_values[0][1]

        for diag_order in range(1, self.size - 1):
            self[diag_order, diag_order - 1], self[diag_order, diag_order], self[diag_order, diag_order + 1] = \
                diag_values[diag_order][0], diag_values[diag_order][1], diag_values[diag_order][2]

        self[self.size - 1, self.size - 2], self[self.size - 1, self.size - 1] = \
            diag_values[-1][0], diag_values[-1][1]

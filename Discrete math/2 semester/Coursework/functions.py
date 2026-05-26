# Построение n-ого списка кода Грея
def BuildGrey(n: int, depth=1) -> list:
    GreyCode = [[0 for _ in range(n)] for __ in range(2 ** n)]
    GreyCode[0][n - 1] = 0
    GreyCode[1][n - 1] = 1
    if n <= 0:
        return []
    elif n == 1:
        return GreyCode
    else:
        CodesCount = 2
        for i in range(1, n):
            t = CodesCount - 1
            CodesCount *= 2
            for k in range(CodesCount // 2, CodesCount):
                GreyCode[k] = GreyCode[t].copy()
                GreyCode[t][n - 1 - i] = 1
                GreyCode[k][n - 1 - i] = 0
                t -= 1
    return GreyCode


# Преобразование двоичного числа в код Грея
# Позволяет подавать на вход
def BinToGrey(b: bytes) -> bytes:
    return b ^ (b >> 1)


def GreyToBin(grey: bytes) -> bytes:
    pass

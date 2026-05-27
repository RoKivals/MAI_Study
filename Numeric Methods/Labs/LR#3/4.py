def find_index(x, x_s):
    for i in range(len(x)):
        if x[i] <= x_s <= x[i+1]:
            return i


def d1(x, y, x_s, i):
    dydx = (y[i+1] - y[i]) / (x[i+1] - x[i])
    return dydx + ((y[i+2] - y[i+1])/(x[i+2] - x[i+1]) - dydx) * (2*x_s - x[i] - x[i+1]) / (x[i+2] - x[i])


def d2(x, y, i):
    return 2 * ((y[i+2] - y[i+1])/(x[i+2] - x[i+1]) - (y[i+1] - y[i])/(x[i+1] - x[i])) / (x[i+2] - x[i])


def main():
    x_s = float(input())
    x = list(map(float, input().split()))
    y = list(map(float, input().split()))
    index = find_index(x, x_s)

    print("Первая производная:", d1(x, y, x_s, index))
    print("Вторая производная:", d2(x, y, index))


if __name__ == "__main__":
    main()
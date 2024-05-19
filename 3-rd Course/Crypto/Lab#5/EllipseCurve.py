from math import sqrt
import random
import time


def is_prime(number):
    for i in range(2, int(sqrt(number)) + 1):
        if number % i == 0:
            return False
    return True


def get_next_prime_number(start):
    while not is_prime(start):
        start += 1
    return start


class EllipticCurve:
    class Point:
        def __init__(self, x, y):
            self.x = x
            self.y = y

        def __eq__(self, point) -> bool:
            return self.x == point.x and self.y == point.y
        
        def __repr__(self):
            return f"Point: x={self.x}, y={self.y}"

        def isCenter(self):
            return self.x == 0 and self.y == 0

    def __init__(self, a, b, p):
        self.a = a
        self.b = b
        self.p = p
        self.ap = a % p
        self.bp = b % p
        self.random = random.Random(2904)

    def set_p(self, value):
        if is_prime(value):
            self.p = value

    def is_elliptic_curve(self, point: Point):
        return (point.y**2) % self.p == (point.x**3 + self.ap * point.x + self.bp) % self.p

    def __repr__(self):
        return f"y^2 = x^3 + {self.ap}*x + {self.bp} % {self.p}"

    # Реализуем деление по модулю p
    # Расширенный алгоритм Евклида, находит НОД для коэф a, b, а также x, y
    # в уравнении ax + by = gcd
    @staticmethod
    def extended_gcd(a, b):
        prev_gcd, gcd = a, b
        prev_x, x = 1, 0
        prev_y, y = 0, 1

        while gcd:
            quotient = prev_gcd // gcd
            prev_gcd, gcd = gcd, prev_gcd - quotient * gcd
            prev_x, x = x, prev_x - quotient * x
            prev_y, y = y, prev_y - quotient * y

        return prev_gcd, prev_x, prev_y

    def inverse_of(self, n):
        gcd, x, _ = self.extended_gcd(n, self.p)

        return x % self.p if gcd == 1 else -1

    def add_points(self, point_1: Point, point_2: Point):
        if point_1.isCenter():
            return point_2

        if point_2.isCenter():
            return point_1

        if point_1.x == point_2.x and point_1.y != point_2.y:
            return self.Point(0, 0)

        if point_1 == point_2:
            s = (3 * point_1.x ** 2 + self.ap) * \
                self.inverse_of(2 * point_1.y) % self.p
        else:
            s = ((point_1.y - point_2.y) *
                 self.inverse_of(point_1.x - point_2.x)) % self.p

        x = (s**2 - 2 * point_1.x) % self.p
        y = (point_1.y + s * (x - point_1.x)) % self.p

        return self.Point(x, -y % self.p)

    def order_point(self, point):
        i = 0
        check = self.add_points(point, point)
        while not check.isCenter():
            check = self.add_points(check, point)
            i += 1

        return i

    def step(self):
        self.ap = self.a % self.p
        self.bp = self.b % self.p
        print(self)

        points = []
        start_time = time.time()

        for x in range(self.p):
            for y in range(self.p):
                curr_point = self.Point(x, y)
                if self.is_elliptic_curve(curr_point):
                    points.append(curr_point)

        print("Curve order:", len(points))

        random_point = random.choice(points)
        print(f"Point {random_point} order:", self.order_point(random_point))

        elapsed_time = time.time() - start_time
        print("Time:", elapsed_time, "s\n")

        return elapsed_time
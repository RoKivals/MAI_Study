from math import sqrt
import random
import time


class EllipticCurve:
    def __init__(self, A, B, P):
        self.A = A
        self.B = B
        self.p = P
        self.ap = A % P
        self.bp = B % P
        self.random = random.Random(2904)
    
    def is_elliptic_curve(self, x, y):
        return (y**2) % self.p == (x**3 + self.ap * x + self.bp) % self.p
    
    def __str__(self):
        return f"y^2 = x^3 + {self.ap}*x + {self.bp} % {self.p}"
    
    @staticmethod
    def extended_euclidean_algorithm(a, b):
        s, old_s = 0, 1
        t, old_t = 1, 0
        r, old_r = b, a
        
        while r != 0:
            quotient = old_r // r
            old_r, r = r, old_r - quotient * r
            old_s, s = s, old_s - quotient * s
            old_t, t = t, old_t - quotient * t
        
        return old_r, old_s, old_t
    
    def inverse_of(self, n):
        gcd, x, _ = self.extended_euclidean_algorithm(n, self.p)

        if gcd != 1:
            return -1
        else:
            return x % self.p
    
    def add_points(self, p1, p2):
        if p1 == (0, 0):
            return p2
        
        if p2 == (0, 0):
            return p1
        
        if p1[0] == p2[0] and p1[1] != p2[1]:
            return (0, 0)
        
        if p1 == p2:
            s = (3 * p1[0]**2 + self.ap) * self.inverse_of(2 * p1[1]) % self.p
        else:
            s = ((p1[1] - p2[1]) * self.inverse_of(p1[0] - p2[0])) % self.p
        
        x = (s**2 - 2 * p1[0]) % self.p
        y = (p1[1] + s * (x - p1[0])) % self.p
        
        return (x, -y % self.p)
    
    def order_point(self, point):
        i = 0
        check = self.add_points(point, point)
        while check != (0, 0):
            check = self.add_points(check, point)
            i += 1

        return i
    
    def step(self):
        self.ap = self.A % self.p
        self.bp = self.B % self.p
        print(self)
        
        points = []
        start_time = time.time()
        
        for x in range(self.p):
            for y in range(self.p):
                if self.is_elliptic_curve(x, y):
                    points.append((x, y))
        
        print("Curve order:", len(points))
        
        random_point = random.choice(points)
        print(f"Point {random_point} order:", self.order_point(random_point))
        
        elapsed_time = time.time() - start_time
        print("Time:", elapsed_time, "s\n")

        return elapsed_time
    
    def is_prime(self, number):
        for i in range(2, sqrt(number)):
            if number % i == 0:
                return False
        return True
    
    def get_next_prime_number(self, start):
        while not self.is_prime(start):
            start += 1

        return start
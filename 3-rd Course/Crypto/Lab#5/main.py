import EllipseCurve as EC


def main():
    a = int(input("a: "))
    b = int(input("b: "))
    p = int(input("p: "))
    time = int(input("time: "))
    print()

    ec = EC.EllipticCurve(a, b, p)

    time_passed = 0
    iter = 1

    while time_passed < time:
        print("Iter", iter)
        ec.set_p(EC.get_next_prime_number(ec.p + iter * 3000))
        time_passed = ec.step()
        iter += 1

    print("Ans:", ec)

main()

# 2s => 8.49s | 1 iter
# 4s => 8.40s | 1 iter
# 8s => 8.61s | 1 iter
# 32s => 8.29s | 1 iter | 60.86s | 2 iter
# 64s => 8.22s | 1 iter | 57.68 | 2 iter | 218.48s | 3 iter
# 128s => 8.28s | 1 iter | 59.55s | 2 iter | 216.15s | 3 iter
# 256s => 8.21s | 1 iter | 57.80s | 2 iter | 215.64s | 3iter | 587.70s | 4 iter
# y^2 = x^3 + 3878*x + 22887 % 30689
# Curve order: 30605
# Point (6284, 17343) order: 186148
# Time: 587.7021234035492 seconds
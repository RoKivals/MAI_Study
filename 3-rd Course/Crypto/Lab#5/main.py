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
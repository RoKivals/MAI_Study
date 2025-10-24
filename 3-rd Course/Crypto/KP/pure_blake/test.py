from pure_blake import Hasher
import matplotlib.pyplot as plt


def differential_analysis(message1, message2, round):
    hasher1 = Hasher(round)
    hasher1.update(message1)
    hash1 = hasher1.finalize()
    hasher2 = Hasher(round)
    hasher2.update(message2)
    hash2 = hasher2.finalize()
    diff_bits = sum(bin(b1 ^ b2).count('1') for b1, b2 in zip(hash1, hash2))
    return diff_bits


def main():
    message1 = b"Hello, world!"
    message2 = b"Hello, world?"
#    message1 = b"Hello, world!"[::-1]
#    message2 = b"Hello, world?"[::-1]

    rounds = range(1, 8)
    differences = [differential_analysis(message1, message2, round) for round in rounds]
    print(differences)

    plt.plot(rounds, differences)
    plt.xlabel('Количество раундов')
    plt.ylabel('Количество отличающихся бит')
    plt.title('Дифференциальный криптоанализ BLAKE3')
    plt.show()
main()


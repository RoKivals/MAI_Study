#! C:/Users/slava/AppData/Local/Programs/Python/Python310/python

import sys
import random
import string


def get_random_string(length: int):
    random_list = [random.choice(string.ascii_letters) for _ in range(length)]
    return "".join(random_list)


def generate_kv():
    key = random.randint(0, 65535)
    length = random.randint(0, 1000000) % 2048
    value = get_random_string(length)
    return key, value


def main():
    if len(sys.argv) != 3:
        print(f"Usage: {sys.argv[0]} <test count=10>")
        sys.exit(1)

    test_count = int(sys.argv[2])
    test_dir = sys.argv[1]
    for num in range(test_count):
        lines_count = random.randint(0, 100)
        input_array = []
        for _ in range(lines_count):
            key, value = generate_kv()
            input_array.append((key, value))
        #answer = sorted(input_array, key=lambda x: x[0])
        test_path_base = f"{test_dir}/{num:02d}"

        with open(f"{test_path_base}.t", "w") as ifd:
            for key, value in input_array:
                ifd.write(f"{key} {value}\n")

        # with open(f"{test_path_base}.a", "w") as ifd:
        #     for key, value in answer:
        #         ifd.write(f"{key} {value}\n")


if __name__ == '__main__':
    main()

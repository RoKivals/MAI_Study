import os
import functions


def main():
    while True:
        print("Данная программа может:")
        print("1. Вывести код Грея n-ого размера")
        print("2. Перевести любое число в код Грея")
        choice = int(input("Укажите, какой пункт меню вы хотите выбрать: "))
        if choice == 1:
            GreySize = int(input("Введите размер кода Грея: "))
            GreyCode = functions.BuildGrey(GreySize)
            for row in GreyCode:
                print(' '.join(list(map(str, row))))
        elif choice == 2:
            GreyNum = int(
                input("Введите число, которое вы ходите закодировать кодом Грея. Вводить в 10-ой системе счиления. \n"))
            print(f"Для числа {GreyNum}, код Грея выглядит так: {functions.BinToGrey(GreyNum):0b}")
        else:
            os.system('cls||clear')
            print("Ошибка, такой пункт выбрать невозможно!")
            break


if __name__ == "__main__":
    main()

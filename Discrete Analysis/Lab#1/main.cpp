#include "CountingSort.hpp"

using namespace std;

// Пара - ключ, значение
template<class T, class Y>
struct Pair {
    T key{};
    Y value{};
};

// Оператор ввода для пары
template<class T, class Y>
std::istream &operator>>(std::istream &in, Pair<T, Y> &p) {
    in >> p.key >> p.value;
    return in;
}

// Оператор вывода для пары
template<class T, class Y>
std::ostream &operator<<(std::ostream &out, Pair<T, Y> &p) {
    out << p.key << " " << p.value;
    return out;
}


int main() {
    Vector<Pair<size_t, string>> data;
    Pair<size_t, string> temp;
    while (cin >> temp) {
        data.PushBack(temp);
    }
    CountingSort<Pair<size_t, string>>(data);
    for (size_t i(0); i < data.Size(); ++i) {
        cout << data[i];
    }
}

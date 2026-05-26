#include <iostream>
#include <iomanip>
#include <cmath>

int main() {
    float a, b, c;
    std::cin >> a >> b >> c;

    float disc = b * b - 4 * a * c;

    if (a == 0 && c == 0 && b == 0) 
        std::cout << "any" << std::endl;
    else if (a == 0 && b == 0 && c != 0) 
        std::cout << "incorrect" << std::endl;
    else if (a == 0) 
        std::cout << -c / b << std::endl;
    else if (disc == 0) 
        std::cout << std::fixed << std::setprecision(6) << -b/(2 * a) << std::endl;
    else if (disc > 0) 
        std::cout << std::fixed << std::setprecision(6) << (-b + sqrt(disc)) / (2 * a) << " " << (-b - sqrt(disc)) / (2 * a) << '\n';
    else 
        std::cout << "imaginary" << std::endl;

    return 0;
}
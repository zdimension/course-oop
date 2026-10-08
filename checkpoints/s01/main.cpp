#include "resistor.hpp"

#include <iostream>
#include <memory>

int main() {
    Resistor automatic("R1", 1000.0);
    std::cout << automatic.description() << ": "
              << automatic.voltage(2e-3) << " V\n";

    auto dynamic = std::make_unique<Resistor>("R2", 2200.0);
    std::cout << dynamic->description() << '\n';
}

#include "resistor.hpp"

#include <cassert>
#include <cmath>

int main() {
    const Resistor resistor("R1", 1000.0);
    assert(resistor.name() == "R1");
    assert(std::abs(resistor.resistance_ohm() - 1000.0) < 1e-12);
    assert(std::abs(resistor.voltage(2e-3) - 2.0) < 1e-12);
}

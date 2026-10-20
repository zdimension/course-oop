#include "capacitor.hpp"
#include "inductor.hpp"
#include "resistor.hpp"

#include <cassert>
#include <cmath>
#include <complex>

int main() {
    const Resistor resistor("R1", 1000.0);
    assert(resistor.name() == "R1");
    assert(resistor.description() == "R1 (1000 ohm)");
    assert(std::abs(resistor.voltage(2e-3) - 2.0) < 1e-12);
    assert(std::abs(resistor.impedance(10.0) - std::complex<double>(1000.0, 0.0)) < 1e-12);
    assert(std::abs(resistor.impedance(10000.0) - std::complex<double>(1000.0, 0.0)) < 1e-12);

    const Capacitor capacitor("C1", 100e-9);
    assert(capacitor.description() == "C1 (1e-07 F)");
    assert(std::abs(capacitor.impedance(1000.0) - std::complex<double>(0.0, -1591.5494309189535)) < 1e-9);

    const Inductor inductor("L1", 10e-3);
    assert(inductor.description() == "L1 (0.01 H)");
    assert(std::abs(inductor.impedance(1000.0) - std::complex<double>(0.0, 62.831853071795862)) < 1e-9);

    const Component& component = capacitor;
    assert(component.name() == "C1");
    assert(component.impedance(1000.0) == capacitor.impedance(1000.0));
}

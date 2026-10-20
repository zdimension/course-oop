#include "capacitor.hpp"
#include "inductor.hpp"
#include "resistor.hpp"

#include <cstddef>
#include <iostream>

void display_at(const Component& component, double frequency_hz) {
    std::cout << component.description() << ": "
              << component.impedance(frequency_hz) << " ohm\n";
}

void display_all(const Component* components[], std::size_t count, double frequency_hz) {
    for (std::size_t i = 0; i < count; ++i) {
        display_at(*components[i], frequency_hz);
    }
}

int main() {
    Resistor resistor("R1", 1000.0);
    Capacitor capacitor("C1", 100e-9);
    Inductor inductor("L1", 10e-3);

    const Component* components[] = {&resistor, &capacitor, &inductor};

    for (const Component* component : components) {
        display_at(*component, 1000.0);
    }

    const double frequencies[] = {10.0, 100.0, 1000.0, 10000.0};
    for (double frequency_hz : frequencies) {
        std::cout << "\nf = " << frequency_hz << " Hz\n";
        display_all(components, 3, frequency_hz);
    }
}

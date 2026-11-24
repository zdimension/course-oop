#include "capacitor.hpp"
#include "filter_kind.hpp"
#include "inductor.hpp"
#include "parallel.hpp"
#include "resistor.hpp"
#include "series.hpp"

#include <iostream>
#include <memory>
#include <stdexcept>
#include <utility>
#include <vector>

void display_at(const Component& component, double frequency_hz) {
    std::cout << component.description() << ": "
              << component.impedance(frequency_hz) << " ohm\n";
}

void display_all(const std::vector<const Component*>& components, double frequency_hz) {
    for (const Component* component : components) {
        display_at(*component, frequency_hz);
    }
}

Series make_divider(double r1_ohm, double r2_ohm) {
    Series divider("Divider");
    divider.add(std::make_unique<Resistor>("R1", r1_ohm));
    divider.add(std::make_unique<Resistor>("R2", r2_ohm));
    return divider;
}

int main() {
    Resistor resistor("R1", 1000.0);
    Capacitor capacitor("C1", 100e-9);
    Inductor inductor("L1", 10e-3);
    std::vector<const Component*> components = {&resistor, &capacitor, &inductor};
    display_all(components, 1000.0);
    display_at(make_divider(1000.0, 2000.0), 1000.0);

    std::cout << '\n';
    try {
        Resistor negative("R2", -1000.0);
        display_at(negative, 1000.0);
    } catch (const std::invalid_argument& error) {
        std::cout << "R2 refused: " << error.what() << '\n';
    }

    try {
        std::cout << capacitor.impedance(0.0) << '\n';
    } catch (const std::invalid_argument& error) {
        std::cout << "C1 at 0 Hz: " << error.what() << '\n';
    }

    Series empty("S1");
    try {
        std::cout << empty.impedance(1000.0) << '\n';
    } catch (const std::logic_error& error) {
        std::cout << "S1: " << error.what() << '\n';
    }

    std::cout << '\n'
              << to_string(RcFilterKind::LowPass) << ", "
              << to_string(RcFilterKind::HighPass) << '\n';
}

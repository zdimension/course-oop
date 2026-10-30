#include "capacitor.hpp"
#include "inductor.hpp"
#include "parallel.hpp"
#include "resistor.hpp"
#include "series.hpp"

#include <iostream>
#include <memory>
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
    Resistor other("R2", 2200.0);

    std::vector<const Component*> components = {&resistor, &capacitor, &inductor};
    components.push_back(&other);
    display_all(components, 1000.0);

    auto p1 = std::make_unique<Parallel>("P1");
    p1->add(std::make_unique<Resistor>("R1", 100.0));
    p1->add(std::make_unique<Resistor>("R2", 1000.0));
    p1->add(std::make_unique<Resistor>("R3", 700.0));

    auto p2 = std::make_unique<Parallel>("P2");
    p2->add(std::make_unique<Resistor>("R4", 800.0));
    p2->add(std::make_unique<Resistor>("R5", 1200.0));

    Series circuit("Circuit");
    circuit.add(std::move(p1));
    circuit.add(std::move(p2));
    circuit.add(std::make_unique<Resistor>("R6", 1100.0));

    std::cout << '\n';
    display_at(circuit, 1000.0);
    display_at(make_divider(1000.0, 2000.0), 1000.0);
}

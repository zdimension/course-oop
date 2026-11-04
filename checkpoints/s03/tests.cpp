#include "capacitor.hpp"
#include "parallel.hpp"
#include "resistor.hpp"
#include "series.hpp"

#include <cassert>
#include <cmath>
#include <complex>
#include <memory>
#include <string>
#include <utility>

int destroyed = 0;

class Probe : public Component {
public:
    Probe(std::string name) : Component(std::move(name)) {}
    ~Probe() { ++destroyed; }

    std::complex<double> impedance(double /* frequency_hz */) const override { return {1.0, 0.0}; }
    std::string description() const override { return name(); }
};

bool near(std::complex<double> actual, std::complex<double> expected) {
    return std::abs(actual - expected) < 1e-9;
}

Series make_divider(double r1_ohm, double r2_ohm) {
    Series divider("Divider");
    divider.add(std::make_unique<Resistor>("R1", r1_ohm));
    divider.add(std::make_unique<Resistor>("R2", r2_ohm));
    return divider;
}

int main() {
    Series series("S1");
    series.add(std::make_unique<Resistor>("R1", 1000.0));
    series.add(std::make_unique<Resistor>("R2", 2000.0));
    assert(series.description() == "S1 = Series(R1 (1000 ohm), R2 (2000 ohm))");
    assert(near(series.impedance(1000.0), {3000.0, 0.0}));
    series.add(std::make_unique<Capacitor>("C1", 100e-9));
    assert(near(series.impedance(1000.0), {3000.0, -1591.5494309189535}));

    Parallel parallel("P");
    parallel.add(std::make_unique<Resistor>("R1", 1000.0));
    parallel.add(std::make_unique<Resistor>("R2", 2000.0));
    assert(near(parallel.impedance(1000.0), {2000.0 / 3.0, 0.0}));

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
    assert(p1 == nullptr && p2 == nullptr);
    assert(circuit.description() ==
           "Circuit = Series(P1 = Parallel(R1 (100 ohm), R2 (1000 ohm), R3 (700 ohm)), "
           "P2 = Parallel(R4 (800 ohm), R5 (1200 ohm)), R6 (1100 ohm))");
    assert(std::abs(circuit.impedance(1000.0) - std::complex<double>(1660.4597701149425, 0.0)) < 1e-6);

    const Series divider = make_divider(1000.0, 2000.0);
    assert(near(divider.impedance(1000.0), {3000.0, 0.0}));

    Series transfer("S2");
    transfer.add(std::make_unique<Resistor>("R1", 1000.0));
    auto r3 = std::make_unique<Resistor>("R3", 3000.0);
    transfer.add(std::move(r3));
    assert(r3 == nullptr);
    assert(near(transfer.impedance(1000.0), {4000.0, 0.0}));

    {
        auto branch = std::make_unique<Parallel>("P");
        branch->add(std::make_unique<Probe>("A"));
        Series owner("S");
        owner.add(std::make_unique<Probe>("B"));
        owner.add(std::move(branch));
        assert(destroyed == 0);
    }
    assert(destroyed == 2);
}

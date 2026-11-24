#include "capacitor.hpp"
#include "filter_kind.hpp"
#include "inductor.hpp"
#include "parallel.hpp"
#include "resistor.hpp"
#include "series.hpp"

#include <cassert>
#include <cmath>
#include <complex>
#include <memory>
#include <stdexcept>
#include <string>
#include <utility>

bool near(std::complex<double> actual, std::complex<double> expected) {
    return std::abs(actual - expected) < 1e-9;
}

bool resistor_rejected(const std::string& name, double resistance_ohm) {
    try {
        Resistor resistor(name, resistance_ohm);
    } catch (const std::invalid_argument&) {
        return true;
    }
    return false;
}

bool capacitor_rejected(double capacitance_farad) {
    try {
        Capacitor capacitor("C1", capacitance_farad);
    } catch (const std::invalid_argument&) {
        return true;
    }
    return false;
}

bool inductor_rejected(double inductance_henry) {
    try {
        Inductor inductor("L1", inductance_henry);
    } catch (const std::invalid_argument&) {
        return true;
    }
    return false;
}

bool frequency_rejected(const Component& component, double frequency_hz) {
    try {
        component.impedance(frequency_hz);
    } catch (const std::invalid_argument&) {
        return true;
    }
    return false;
}

bool empty_rejected(const Component& circuit) {
    try {
        circuit.impedance(1000.0);
    } catch (const std::logic_error&) {
        return true;
    }
    return false;
}

int main() {
    assert(resistor_rejected("R1", -1000.0));
    assert(resistor_rejected("R1", 0.0));
    assert(resistor_rejected("", 1000.0));
    assert(!resistor_rejected("R1", 1000.0));
    assert(capacitor_rejected(0.0));
    assert(capacitor_rejected(-100e-9));
    assert(!capacitor_rejected(100e-9));
    assert(inductor_rejected(0.0));
    assert(inductor_rejected(-10e-3));
    assert(!inductor_rejected(10e-3));

    Resistor resistor("R1", 1000.0);
    Capacitor capacitor("C1", 100e-9);
    Inductor inductor("L1", 10e-3);
    assert(frequency_rejected(resistor, 0.0));
    assert(frequency_rejected(capacitor, 0.0));
    assert(frequency_rejected(inductor, -1000.0));
    assert(!frequency_rejected(capacitor, 1000.0));
    assert(near(resistor.impedance(1000.0), {1000.0, 0.0}));
    assert(near(capacitor.impedance(1000.0), {0.0, -1591.5494309189535}));
    assert(near(inductor.impedance(1000.0), {0.0, 62.831853071795862}));

    bool name_rejected = false;
    try {
        Series series("");
    } catch (const std::invalid_argument&) {
        name_rejected = true;
    }
    assert(name_rejected);

    Series series("S1");
    Parallel parallel("P1");
    assert(empty_rejected(series));
    assert(empty_rejected(parallel));
    assert(series.description() == "S1 = Series()");

    auto r1 = std::make_unique<Resistor>("R1", 1000.0);
    series.add(std::move(r1));
    bool null_rejected = false;
    try {
        series.add(std::move(r1));
    } catch (const std::invalid_argument&) {
        null_rejected = true;
    }
    assert(null_rejected);
    series.add(std::make_unique<Resistor>("R2", 2000.0));
    assert(series.description() == "S1 = Series(R1 (1000 ohm), R2 (2000 ohm))");
    assert(near(series.impedance(1000.0), {3000.0, 0.0}));
    assert(frequency_rejected(series, 0.0));

    null_rejected = false;
    try {
        parallel.add(nullptr);
    } catch (const std::invalid_argument&) {
        null_rejected = true;
    }
    assert(null_rejected);
    parallel.add(std::make_unique<Resistor>("R1", 1000.0));
    parallel.add(std::make_unique<Resistor>("R2", 2000.0));
    assert(near(parallel.impedance(1000.0), {2000.0 / 3.0, 0.0}));

    assert(to_string(RcFilterKind::LowPass) == "low-pass");
    assert(to_string(RcFilterKind::HighPass) == "high-pass");
    bool kind_rejected = false;
    try {
        to_string(static_cast<RcFilterKind>(42));
    } catch (const std::invalid_argument&) {
        kind_rejected = true;
    }
    assert(kind_rejected);
}

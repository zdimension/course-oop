#include "capacitor.hpp"

#include <sstream>
#include <utility>

constexpr double pi = 3.14159265358979323846;

Capacitor::Capacitor(std::string name, double capacitance_farad)
    : Component(std::move(name)), capacitance_farad_(capacitance_farad) {}

double Capacitor::capacitance_farad() const {
    return capacitance_farad_;
}

std::complex<double> Capacitor::impedance(double frequency_hz) const {
    const std::complex<double> j{0.0, 1.0};
    return 1.0 / (j * 2.0 * pi * frequency_hz * capacitance_farad_);
}

std::string Capacitor::description() const {
    std::ostringstream output;
    output << name() << " (" << capacitance_farad_ << " F)";
    return output.str();
}

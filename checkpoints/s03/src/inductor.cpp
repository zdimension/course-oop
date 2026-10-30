#include "inductor.hpp"

#include <sstream>
#include <utility>

constexpr double pi = 3.14159265358979323846;

Inductor::Inductor(std::string name, double inductance_henry)
    : Component(std::move(name)), inductance_henry_(inductance_henry) {}

double Inductor::inductance_henry() const {
    return inductance_henry_;
}

std::complex<double> Inductor::impedance(double frequency_hz) const {
    const std::complex<double> j{0.0, 1.0};
    return j * 2.0 * pi * frequency_hz * inductance_henry_;
}

std::string Inductor::description() const {
    std::ostringstream output;
    output << name() << " (" << inductance_henry_ << " H)";
    return output.str();
}

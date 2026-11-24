#include "resistor.hpp"

#include <sstream>
#include <stdexcept>
#include <utility>

Resistor::Resistor(std::string name, double resistance_ohm)
    : Component(std::move(name)), resistance_ohm_(resistance_ohm) {
    if (resistance_ohm_ <= 0.0) {
        throw std::invalid_argument("resistance must be positive");
    }
}

double Resistor::resistance_ohm() const {
    return resistance_ohm_;
}

double Resistor::voltage(double current_ampere) const {
    return resistance_ohm_ * current_ampere;
}

std::complex<double> Resistor::impedance(double frequency_hz) const {
    if (frequency_hz <= 0.0) {
        throw std::invalid_argument("frequency must be positive");
    }
    return {resistance_ohm_, 0.0};
}

std::string Resistor::description() const {
    std::ostringstream output;
    output << name() << " (" << resistance_ohm_ << " ohm)";
    return output.str();
}

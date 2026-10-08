#include "resistor.hpp"

#include <sstream>
#include <utility>

Resistor::Resistor(std::string name, double resistance_ohm)
    : name_(std::move(name)), resistance_ohm_(resistance_ohm) {}

const std::string& Resistor::name() const {
    return name_;
}

double Resistor::resistance_ohm() const {
    return resistance_ohm_;
}

double Resistor::voltage(double current_ampere) const {
    return resistance_ohm_ * current_ampere;
}

std::string Resistor::description() const {
    std::ostringstream output;
    output << name_ << " (" << resistance_ohm_ << " ohm)";
    return output.str();
}

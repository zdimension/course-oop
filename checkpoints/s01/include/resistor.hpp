#pragma once

#include <string>

class Resistor {
public:
    Resistor(std::string name, double resistance_ohm);

    const std::string& name() const;
    double resistance_ohm() const;
    double voltage(double current_ampere) const;
    std::string description() const;

private:
    std::string name_;
    double resistance_ohm_;
};

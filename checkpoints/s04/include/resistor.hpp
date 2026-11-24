#pragma once

#include "component.hpp"

#include <complex>
#include <string>

class Resistor : public Component {
public:
    Resistor(std::string name, double resistance_ohm);

    double resistance_ohm() const;
    double voltage(double current_ampere) const;
    std::complex<double> impedance(double frequency_hz) const override;
    std::string description() const override;

private:
    double resistance_ohm_;
};

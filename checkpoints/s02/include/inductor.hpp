#pragma once

#include "component.hpp"

#include <complex>
#include <string>

class Inductor : public Component {
public:
    Inductor(std::string name, double inductance_henry);

    double inductance_henry() const;
    std::complex<double> impedance(double frequency_hz) const override;
    std::string description() const override;

private:
    double inductance_henry_;
};

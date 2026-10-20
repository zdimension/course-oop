#pragma once

#include "component.hpp"

#include <complex>
#include <string>

class Capacitor : public Component {
public:
    Capacitor(std::string name, double capacitance_farad);

    double capacitance_farad() const;
    std::complex<double> impedance(double frequency_hz) const override;
    std::string description() const override;

private:
    double capacitance_farad_;
};

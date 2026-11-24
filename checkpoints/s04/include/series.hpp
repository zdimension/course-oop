#pragma once

#include "component.hpp"

#include <complex>
#include <memory>
#include <string>
#include <vector>

class Series : public Component {
public:
    Series(std::string name);

    void add(std::unique_ptr<Component> component);
    std::complex<double> impedance(double frequency_hz) const override;
    std::string description() const override;

private:
    std::vector<std::unique_ptr<Component>> components_;
};

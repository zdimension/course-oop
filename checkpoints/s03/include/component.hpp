#pragma once

#include <complex>
#include <string>

class Component {
public:
    virtual ~Component() = default;

    const std::string& name() const;
    virtual std::complex<double> impedance(double frequency_hz) const = 0;
    virtual std::string description() const = 0;

protected:
    Component(std::string name);

private:
    std::string name_;
};

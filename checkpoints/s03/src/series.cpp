#include "series.hpp"

#include <cstddef>
#include <utility>

Series::Series(std::string name) : Component(std::move(name)) {}

void Series::add(std::unique_ptr<Component> component) {
    components_.push_back(std::move(component));
}

std::complex<double> Series::impedance(double frequency_hz) const {
    std::complex<double> total{0.0, 0.0};
    for (const auto& component : components_) {
        total += component->impedance(frequency_hz);
    }
    return total;
}

std::string Series::description() const {
    std::string result = name() + " = Series(";
    for (std::size_t i = 0; i < components_.size(); ++i) {
        if (i > 0) {
            result += ", ";
        }
        result += components_[i]->description();
    }
    return result + ")";
}

#include "series.hpp"

#include <cstddef>
#include <stdexcept>
#include <utility>

Series::Series(std::string name) : Component(std::move(name)) {}

void Series::add(std::unique_ptr<Component> component) {
    if (component == nullptr) {
        throw std::invalid_argument("component must not be null");
    }
    components_.push_back(std::move(component));
}

std::complex<double> Series::impedance(double frequency_hz) const {
    if (components_.empty()) {
        throw std::logic_error("series is empty");
    }
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

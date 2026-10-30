#include "parallel.hpp"

#include <cstddef>
#include <utility>

Parallel::Parallel(std::string name) : Component(std::move(name)) {}

void Parallel::add(std::unique_ptr<Component> component) {
    components_.push_back(std::move(component));
}

std::complex<double> Parallel::impedance(double frequency_hz) const {
    std::complex<double> inverse{0.0, 0.0};
    for (const auto& component : components_) {
        inverse += 1.0 / component->impedance(frequency_hz);
    }
    return 1.0 / inverse;
}

std::string Parallel::description() const {
    std::string result = name() + " = Parallel(";
    for (std::size_t i = 0; i < components_.size(); ++i) {
        if (i > 0) {
            result += ", ";
        }
        result += components_[i]->description();
    }
    return result + ")";
}

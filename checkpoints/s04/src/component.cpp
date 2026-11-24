#include "component.hpp"

#include <stdexcept>
#include <utility>

Component::Component(std::string name) : name_(std::move(name)) {
    if (name_.empty()) {
        throw std::invalid_argument("name must not be empty");
    }
}

const std::string& Component::name() const {
    return name_;
}

#include "component.hpp"

#include <utility>

Component::Component(std::string name) : name_(std::move(name)) {}

const std::string& Component::name() const {
    return name_;
}

#include "filter_kind.hpp"

#include <stdexcept>

std::string to_string(RcFilterKind kind) {
    switch (kind) {
    case RcFilterKind::LowPass:
        return "low-pass";
    case RcFilterKind::HighPass:
        return "high-pass";
    }
    throw std::invalid_argument("unknown filter kind");
}

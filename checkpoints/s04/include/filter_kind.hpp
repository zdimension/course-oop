#pragma once

#include <string>

enum class RcFilterKind { LowPass, HighPass };

std::string to_string(RcFilterKind kind);

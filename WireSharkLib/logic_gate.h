//
// Created by Administrator on 2023/12/15.
//

#ifndef LOGIC_GATES_H
#define LOGIC_GATES_H

#include <array>
#include <random>

#include "quick_queue.h"
#include <tuple>

struct gate_info {
    int x, y;
    int *last_active;
};

struct logic_gate {
    int lamp_on, last_active;
    bool active;
    gate_info info;
};

inline int update_round;

inline quick_queue<gate_info> gates_next, gates_current;

template <int lamp_total>
void all_on_gate_checker(logic_gate *gate) {
    auto cur = gate->lamp_on == lamp_total;
    if (cur ^ gate->active) {
        gate->active = cur;
        if (update_round != gate->last_active)
            gates_next.push(gate->info);
    }
}

inline void any_on_gate_checker(logic_gate *gate) {
    auto cur = gate->lamp_on != 0;
    if (cur ^ gate->active) {
        gate->active = cur;
        if (update_round != gate->last_active)
            gates_next.push(gate->info);
    }
}

template <int lamp_total>
void any_off_gate_checker(logic_gate *gate) {
    auto cur = gate->lamp_on != lamp_total;
    if (cur ^ gate->active) {
        gate->active = cur;
        if (update_round != gate->last_active)
            gates_next.push(gate->info);
    }
}

inline void all_off_gate_checker(logic_gate *gate) {
    auto cur = gate->lamp_on == 0;
    if (cur ^ gate->active) {
        gate->active = cur;
        if (update_round != gate->last_active)
            gates_next.push(gate->info);
    }
}

inline void one_on_gate_checker(logic_gate *gate) {
    auto cur = gate->lamp_on == 1;
    if (cur ^ gate->active) {
        gate->active = cur;
        if (update_round != gate->last_active)
            gates_next.push(gate->info);
    }
}

inline void not_one_on_gate_checker(logic_gate *gate) {
    auto cur = gate->lamp_on != 1;
    if (cur ^ gate->active) {
        gate->active = cur;
        if (update_round != gate->last_active)
            gates_next.push(gate->info);
    }
}

static auto generator = std::default_random_engine();

template <int lamp_total>
void error_gate_checker(logic_gate *gate) {
    if (generator() % lamp_total < gate->lamp_on) {
        gates_next.push(gate->info);
    }
}

struct one_error_gate {
    std::array<bool *, 4> states;
    bool originalState;
    gate_info info;
    int last_active = 0;
};

template <int n>
void one_error_gate_checker(one_error_gate *gate) {
    auto state = gate->originalState;
    if constexpr (n > 0) state ^= *gate->states[0];
    if constexpr (n > 1) state ^= *gate->states[1];
    if constexpr (n > 2) state ^= *gate->states[2];
    if constexpr (n > 3) state ^= *gate->states[3];
    if (state) gates_next.push(gate->info);
}

#endif //LOGIC_GATES_H

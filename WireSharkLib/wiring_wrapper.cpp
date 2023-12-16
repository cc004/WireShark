//
// Created by Administrator on 2023/12/15.
//

#include "wiring_wrapper.h"
#include <cstring>
#include <iostream>

#include "common.h"
#include "logic_gate.h"
#include "pixel_box.h"
#include "interop.h"

void wiring_wrapper::BigTripWire(int l, int t, int w, int h) {
    for (int i = 0; i < w; ++i)
        for (int j = 0; j < h; ++j)
            TripWireSingle(l + i, t + j);
}

void wiring_wrapper::TripWireSingle(const int l, const int t) {
    if (inputConnectedCompoents[l][t]) {
        // std::cout << "invoke input connected components @" << l << ", " << t << std::endl;
        inputConnectedCompoents[l][t]();
    }

    PixelBoxPass();
    LogicGatePass();
}

void wiring_wrapper::PixelBoxPass() {
    while (box_to_update.tail) {
        --box_to_update.tail;
        const auto box = box_to_update.cache[box_to_update.tail];
        if (box->state == 3) {
            *box->TileFrameX = 18 - *box->TileFrameX;
        }
        box->state = 0;
    }
}

void wiring_wrapper::LogicGatePass() {
    if (gates_current.tail) return;
    ++update_round;

    while (lamps_to_check.tail) {
        while (lamps_to_check.tail) {
            --lamps_to_check.tail;
            lamps_to_check.cache[lamps_to_check.tail]();
        }

        while (gates_next.tail) {
            {
                auto t = gates_next;
                gates_next = gates_current;
                gates_current = t;
            }

            for (int i = 0; i < gates_current.tail; ++i) {
                auto& [x, y, t] = gates_current.cache[i];
                if (!t) {

                } else if (*t != update_round) {
                    *t = update_round;
                    TripWireSingle(x, y);
                }
            }

            gates_current.tail = 0;
        }
    }
}

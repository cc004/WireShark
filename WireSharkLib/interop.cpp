//
// Created by Administrator on 2023/12/15.
//
#include "interop.h"
#include "wiring_wrapper.h"

#include <iostream>

void BigTripWire(int l, int t, int w, int h) {
    wiring_wrapper::BigTripWire(l, t, w, h);
}

void HitTile(int x, int y) {
    // std::cout << "[debug] wire triggered: " << x << ", " << y << std::endl;
    triggers.push(x);
    triggers.push(y);
}

void Teleport(const Vector2& from, const Vector2& to) {
    triggers.push(from.first);
    triggers.push(from.second);
    triggers.push(to.first);
    triggers.push(to.second);
}

void RetrieveUpdates(UpdateInfo* info) {
    info->teleports = teleports.cache;
    info->teleport_len = teleports.tail / 4;
    info->triggers = triggers.cache;
    info->trigger_len = triggers.tail / 2;

    teleports.tail = 0;
    triggers.tail = 0;
}

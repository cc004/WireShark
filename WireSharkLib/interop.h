//
// Created by Administrator on 2023/12/15.
//

#ifndef INTEROP_H
#define INTEROP_H

#include "pixel_box.h"
#include "common.h"

#include "quick_queue.h"

inline quick_queue<float> teleports;
inline quick_queue<int> triggers;

extern "C" {
    struct UpdateInfo {
        float* teleports;
        int teleport_len;
        int* triggers;
        int trigger_len;
    };

    void RetrieveUpdates(UpdateInfo* info);

    // these methods are implemented by codegen
    pixel_box* GetPixelBoxPointer();
    int GetPixelBoxCount();
    void BigTripWire(int l, int t, int w, int h);
}

void HitTile(int x, int y);

void Teleport(const Vector2 &from, const Vector2 &to);

#endif //INTEROP_H

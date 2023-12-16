//
// Created by Administrator on 2023/12/15.
//

#ifndef COMMON_H
#define COMMON_H

#include <tuple>
#include "quick_queue.h"

constexpr int maxTilesX = 8400, maxTilesY = 2400;

using Vector2 = std::pair<float, float>;

inline Vector2* teleport;
inline Vector2 teleport_t[8];

using Connection = void (*) ();

extern Connection inputConnectedCompoents[maxTilesX][maxTilesY];

struct bind {
    void (*func)(void *ptr);

    void *ptr;

    void operator()() const
    {
        func(ptr);
    }

    template <typename T, typename T2>
    bind (void (*func)(T *ptr), T2* ptr) : func(reinterpret_cast<void (*)(void*)>(func)), ptr(ptr) {}

    bind() : func(nullptr), ptr(nullptr) {}
};

inline quick_queue<bind> lamps_to_check;

#endif //COMMON_H

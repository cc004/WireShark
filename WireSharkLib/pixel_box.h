//
// Created by Administrator on 2023/12/15.
//

#ifndef PIXEL_BOX_H
#define PIXEL_BOX_H

#include "quick_queue.h"

struct pixel_box {
    int state;
    int x, y;
    short *TileFrameX;
};

inline quick_queue<pixel_box*> box_to_update;

#endif //PIXEL_BOX_H

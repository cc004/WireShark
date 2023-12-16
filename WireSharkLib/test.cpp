//
// Created by Administrator on 2023/12/16.
//


#include "interop.h"

int main() {
    /*
    freopen("test.txt", "w", stdout);
    for (int i = 0; i < maxTilesX; ++i) {
        for (int j = 0; j < maxTilesY; ++j) {
            int t = 0;
            for (int k = 0; k < 4; ++k)
                if (inputConnectedCompoents[i][j][k])
                    t |= 1 << k;
            std::cout << (t ? (char)('a' + t) : ' ');
        }
        std::cout << std::endl;
    }
    //BigTripWire(2702, 2081, 1, 1);
    for (int i = 0; i < maxTilesX; ++i)
        for (int j = 0; j < maxTilesY; ++j)
            if (inputConnectedCompoents[i][j])
                std::cout << i << ", " << j << std::endl;
    std::string s;
    std::cout << "counting" << std::endl;
    BigTripWire(349, 752, 1, 1);
    std::cout << "reseting" << std::endl;
    BigTripWire(342 , 752, 1, 1);
    std::cout << "counting" << std::endl;
    BigTripWire(349, 752, 1, 1);*/

    BigTripWire(354, 752, 1, 1);
}

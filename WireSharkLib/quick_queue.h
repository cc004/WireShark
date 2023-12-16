#ifndef QUEUE_H
#define QUEUE_H

template <typename T>
class quick_queue
{
public:
    T* cache;
    int tail = 0;
    quick_queue() { cache = new T[1024 * 1024 * 10]; }

    template <typename... Args>
    void push(Args... args) { new(cache + (tail++)) T(args...); }
};

#endif
#pragma once

#include "common.h"

template<typename T>
T *reallocate_array(T *pointer, size_t old_size, size_t new_size) {
    if (new_size == 0) {
        delete[] pointer;
        return nullptr;
    }

    if (old_size == 0) {
        return new T[new_size];
    }

    T *new_pointer = new T[new_size];
    size_t copy_size = old_size < new_size ? old_size : new_size;

    for (size_t i = 0; i < copy_size; i++) {
        new_pointer[i] = pointer[i];
    }

    delete[] pointer;
    return new_pointer;
}

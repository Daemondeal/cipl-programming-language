#ifndef cipl_memory_h
#define cipl_memory_h

#include "common.h"


// Minimum elements in all dynamic arrays
#define MIN_ARRAY_SIZE 8

// Grow factor off an array after reallocation
#define GROW_FACTOR 2

#define GROW_CAPACITY(capacity) \
    ((capacity) < MIN_ARRAY_SIZE ? MIN_ARRAY_SIZE : (capacity * GROW_FACTOR))

// Grows an array, and returns the new pointer to memory
#define GROW_ARRAY(type, pointer, old_count, new_count) \
    (type*)reallocate(pointer, sizeof(type) * (old_count), \
    sizeof(type) * (new_count))

// Frees an array
#define FREE_ARRAY(type, pointer, capacity) \
    reallocate(pointer, sizeof(type) * (capacity), 0)

void *reallocate(void *pointer, usize old_size, usize new_size);

#endif
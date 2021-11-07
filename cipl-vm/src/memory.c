#include "memory.h"

#include <stdio.h>
#include <stdlib.h>

void *reallocate(void *pointer, usize old_size, usize new_size) {
    if (old_size == new_size)
        return pointer;

    if (new_size == 0) {
        free(pointer);
        return NULL;
    }

    // realloc works on NULL pointers
    void* result = realloc(pointer, new_size);

    if (result == NULL) {
        fprintf(stderr, "Can't allocate any more memroy.");
        exit(-1);
    }

    return result;
}
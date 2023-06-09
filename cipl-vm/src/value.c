#include "value.h"
#include "memory.h"

#include <stdio.h>

void printValue(Value value) {
    printf("%g", value);
}

void initValueArray(ValueArray *array) {
    array->count = 0;
    array->capacity = 0;
    array->values = NULL;
}

void pushValueArray(ValueArray *array, Value value) {
    if (array->capacity < array->count + 1) {
        usize old_capacity = array->capacity;
        array->capacity = GROW_CAPACITY(old_capacity);
        array->values = GROW_ARRAY(Value, array->values, old_capacity, array->capacity);
    }

    array->values[array->count] = value;
    array->count++;
}

void freeValueArray(ValueArray *array) {
    FREE_ARRAY(Value, array->values, array->capacity);
    initValueArray(array);
}
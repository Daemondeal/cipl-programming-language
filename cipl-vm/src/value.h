#ifndef cipl_value_h
#define cipl_value_h

#include "common.h"

typedef f64 Value;

typedef struct {
    usize capacity;
    usize count;
    Value *values;
} ValueArray;

void printValue(Value value);

void initValueArray(ValueArray *array);
void pushValueArray(ValueArray *array, Value value);
void freeValueArray(ValueArray *array);

#endif
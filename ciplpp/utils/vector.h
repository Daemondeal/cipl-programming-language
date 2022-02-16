#pragma once

#include "common.h"
#include "memory.h"

#include <cstdio>

template <typename T>
class Vector {
private:
    const size_t STARTING_CAPACITY = 8;
    const size_t GROWTH_FACTOR = 2;

    typedef T* iterator;

    T *m_buffer;
    size_t m_capacity;
    size_t m_count;

    void reserve(size_t new_capacity) {
        m_buffer = reallocate_array(m_buffer, m_capacity, new_capacity);
        m_capacity = new_capacity;
    }

public:
    Vector() : m_buffer(nullptr),
        m_capacity(0), m_count(0) {}

    ~Vector() {
        reallocate_array(m_buffer, m_capacity, 0);
    }

    iterator begin() {
        return m_buffer;
    }

    iterator end() {
        return m_buffer + m_count;
    }

    T &operator[](size_t index) {
        return m_buffer[index];
    }

    void push(T item) {
        if (m_count >= m_capacity)
            reserve(m_capacity < STARTING_CAPACITY ? STARTING_CAPACITY : (m_capacity * GROWTH_FACTOR));

        m_buffer[m_count] = item;
        m_count++;
    }

    T pop() {
        m_count--;
        return m_buffer[m_count];
    }

    T& first() {
       return m_buffer[0];
    }

    T& last() {
        return m_buffer[m_count - 1];
    }

    size_t count() {
        return m_count;
    }
};

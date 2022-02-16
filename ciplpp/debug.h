#pragma once

#include "utils/common.h"
#include "chunk.h"

void disassembleChunk(Chunk &chunk, const char *name);
size_t disassembleInstruction(Chunk &chunk, size_t offset);

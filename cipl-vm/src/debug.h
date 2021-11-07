#ifndef cipl_debug_h
#define cipl_debug_h

#include "common.h"
#include "chunk.h"

void disassembleChunk(Chunk *chunk, const char *name);
usize disassembleInstruction(Chunk *chunk, usize offset);


#endif
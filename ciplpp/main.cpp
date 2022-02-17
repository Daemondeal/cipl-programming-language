#include <cstdio>
#include <cstdlib>
#include <cstring>

#include "utils/common.h"
#include "utils/vector.h"

#include "chunk.h"
#include "debug.h"
#include "vm.h"



static void repl(VM &vm) {
    char line[1024];
    for (;;) {
        printf("> ");

        if (!fgets(line, sizeof(line), stdin)) {
            printf("\n");
            break;
        }

        vm.interpret(line);
    }
}

static char *read_file(const char *path) {
    FILE *fp = fopen(path, "r");
    if (fp == nullptr) {
        fprintf(stderr, "Unable to open file %s.\n", path);
        exit(74);
    }

    fseek(fp, 0L, SEEK_END);
    size_t file_size = ftell(fp);
    rewind(fp);

    char *buffer = (char*)malloc(file_size + 1);
    if (buffer == nullptr) {
        fprintf(stderr, "Not enough memory to read %s\n", path);
        exit(74);
    }

    size_t bytes_read = fread(buffer, sizeof(char), file_size, fp);
    if (bytes_read < file_size) {
        fprintf(stderr, "Could not read file %s\n", path);
        exit(74);
    }

    buffer[bytes_read] = '\0';

    fclose(fp);
    return buffer;
}

static void run_file(VM &vm, const char *path) {
    char *source = read_file(path);
    InterpretResult result = vm.interpret(source);
    free(source);

    if (result == INTERPRET_COMPILE_ERROR) exit(65);
    if (result == INTERPRET_RUNTIME_ERROR) exit(70);
}

int main(int argc, const char *argv[]) {
    VM vm;

    if (argc == 1)
        repl(vm);
    else if (argc == 2)
        run_file(vm, argv[1]);
    else {
        fprintf(stderr, "Usage: ciplpp [path]\n");
        exit(64);
    }


    return 0;
}

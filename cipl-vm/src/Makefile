CC=gcc
CFLAGS=-Wall
OUTNAME=cipl-vm

SOURCES=main.c chunk.c memory.c debug.c value.c

SUBDIRS=compiler vm
SUBDIRSLIB=compiler/compiler.o vm/vm.o
SUBDIRSCLEAN=$(addsuffix clean, ${SUBDIRS}) .clean

MAKE=make
RM=rm

.PHONY: all ${SUBDIRS}

all: ${SOURCES} ${SUBDIRS}
	${CC} ${CFLAGS} -o ${OUTDIR}/${OUTNAME} ${SOURCES} ${SUBDIRSLIB}

# Compiling Subdirs
.PHONY: subdirs ${SUBDIRS}
subdirs: ${SUBDIRS}

${SUBDIRS}:
	$(MAKE) -C $@


.PHONY: test
test:
	echo ${PWD}

# Cleaning
.PHONY: clean
clean: ${SUBDIRSCLEAN}

clean_curdir:
	rm -rfv *.o *~ ${OUTNAME} .depend .*.cmd *.ko *.mod.c

%clean: %
	${MAKE} -C $< -f ${PWD}/Makefile clean_curdir
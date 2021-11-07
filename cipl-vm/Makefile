
.PHONY: all clean
all:
	make -C ${PWD}/src/ all OUTDIR=${PWD}/

clean:
	rm cipl-vm
	cd ./src && ${MAKE} clean && cd ..
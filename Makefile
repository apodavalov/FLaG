all: pdf

./Output/sampledz6.4.tex: Samples/sampledz6.4.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sampledz6.4.xml Output/sampledz6.4.tex

./Output/sampledz6.tex: Samples/sampledz6.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sampledz6.xml Output/sampledz6.tex

./Output/sample3.6.tex: Samples/sample3.6.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample3.6.xml Output/sample3.6.tex
	[ -f ./Samples/sample3.6.tex.patch ] && patch -p1 -d ./Output < ./Samples/sample3.6.tex.patch

./Output/sample2.2.tex: Samples/sample2.2.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample2.2.xml Output/sample2.2.tex
#	[ -f ./Samples/sample2.2.tex.patch ] && patch -p1 -d ./Output < ./Samples/sample2.2.tex.patch

./Output/sample2.3.tex: Samples/sample2.3.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample2.3.xml Output/sample2.3.tex
#	[ -f ./Samples/sample2.3.tex.patch ] && patch -p1 -d ./Output < ./Samples/sample2.3.tex.patch

./Output/sample2.7.tex: Samples/sample2.7.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample2.7.xml Output/sample2.7.tex
	[ -f ./Samples/sample2.7.tex.patch ] && patch -p1 -d ./Output < ./Samples/sample2.7.tex.patch

./Output/sample6.3.tex: Samples/sample6.3.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample6.3.xml Output/sample6.3.tex
	[ -f ./Samples/sample6.3.tex.patch ] && patch -p1 -d ./Output < ./Samples/sample6.3.tex.patch

./Output/sample4.4.tex: Samples/sample4.4.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample4.4.xml Output/sample4.4.tex
	[ -f ./Samples/sample4.4.tex.patch ] && patch -p1 -d ./Output < ./Samples/sample4.4.tex.patch

./Output/sample5.4.tex: Samples/sample5.4.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample5.4.xml Output/sample5.4.tex
	[ -f ./Samples/sample5.4.tex.patch ] && patch -p1 -d ./Output < ./Samples/sample5.4.tex.patch

./Output/sample1.4.tex: Samples/sample1.4.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample1.4.xml Output/sample1.4.tex
	[ -f ./Samples/sample1.4.tex.patch ] && patch -p1 -d ./Output < ./Samples/sample1.4.tex.patch

./Output/sample6.2.tex: Samples/sample6.2.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample6.2.xml Output/sample6.2.tex

./Output/sample7.3.tex: Samples/sample7.3.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample7.3.xml Output/sample7.3.tex
	[ -f ./Samples/sample7.3.tex.patch ] && patch -p1 -d ./Output < ./Samples/sample7.3.tex.patch
	
./Output/sample0.0.tex: Samples/sample0.0.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample0.0.xml Output/sample0.0.tex

./Output/sample1.2.tex: Samples/sample1.2.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample1.2.xml Output/sample1.2.tex

./Output/sample1.7.tex: Samples/sample1.7.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample1.7.xml Output/sample1.7.tex

./Output/sample1.8.tex: Samples/sample1.8.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample1.8.xml Output/sample1.8.tex

./Output/sample4.3.tex: Samples/sample4.3.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample4.3.xml Output/sample4.3.tex

./Output/sample4.2.tex: Samples/sample4.2.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample4.2.xml Output/sample4.2.tex

./Output/sample4.1.tex: Samples/sample4.1.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample4.1.xml Output/sample4.1.tex

tex: ./Output/sample2.3.tex ./Output/sample4.2.tex ./Output/sample1.8.tex ./Output/sample2.2.tex ./Output/sampledz6.4.tex ./Output/sampledz6.tex ./Output/sample3.6.tex ./Output/sample2.7.tex ./Output/sample6.3.tex ./Output/sample4.4.tex ./Output/sample5.4.tex ./Output/sample6.2.tex ./Output/sample4.1.tex ./Output/sample4.3.tex ./Output/sample7.3.tex ./Output/sample1.2.tex ./Output/sample1.7.tex ./Output/sample0.0.tex ./Output/sample1.4.tex

./Output/sampledz6.4.pdf: ./Output/sampledz6.4.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sampledz6.4.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sampledz6.4.tex

./Output/sampledz6.pdf: ./Output/sampledz6.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sampledz6.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sampledz6.tex

./Output/sample3.6.pdf: ./Output/sample3.6.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample3.6.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample3.6.tex

./Output/sample2.3.pdf: ./Output/sample2.3.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample2.3.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample2.3.tex

./Output/sample2.2.pdf: ./Output/sample2.2.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample2.2.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample2.2.tex

./Output/sample2.7.pdf: ./Output/sample2.7.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample2.7.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample2.7.tex

./Output/sample6.3.pdf: ./Output/sample6.3.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample6.3.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample6.3.tex

./Output/sample4.4.pdf: ./Output/sample4.4.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample4.4.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample4.4.tex

./Output/sample5.4.pdf: ./Output/sample5.4.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample5.4.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample5.4.tex

./Output/sample6.2.pdf: ./Output/sample6.2.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample6.2.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample6.2.tex

./Output/sample1.4.pdf: ./Output/sample1.4.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample1.4.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample1.4.tex

./Output/sample0.0.pdf: ./Output/sample0.0.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample0.0.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample0.0.tex

./Output/sample4.2.pdf: ./Output/sample4.2.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample4.2.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample4.2.tex

./Output/sample4.1.pdf: ./Output/sample4.1.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample4.1.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample4.1.tex

./Output/sample4.3.pdf: ./Output/sample4.3.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample4.3.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample4.3.tex

./Output/sample7.3.pdf: ./Output/sample7.3.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample7.3.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample7.3.tex

./Output/sample1.7.pdf: ./Output/sample1.7.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample1.7.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample1.7.tex

./Output/sample1.8.pdf: ./Output/sample1.8.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample1.8.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample1.8.tex

./Output/sample1.2.pdf: ./Output/sample1.2.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample1.2.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample1.2.tex

pdf: ./Output/sample2.3.pdf ./Output/sample4.2.pdf ./Output/sample1.8.pdf ./Output/sample2.2.pdf ./Output/sampledz6.4.pdf ./Output/sampledz6.pdf ./Output/sample3.6.pdf ./Output/sample2.7.pdf ./Output/sample6.3.pdf ./Output/sample4.4.pdf ./Output/sample5.4.pdf ./Output/sample6.2.pdf ./Output/sample4.1.pdf ./Output/sample4.3.pdf ./Output/sample7.3.pdf ./Output/sample1.2.pdf ./Output/sample1.7.pdf ./Output/sample0.0.pdf ./Output/sample1.4.pdf

clean:
	rm -rf ./Output/*

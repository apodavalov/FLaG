all: clean pdf

./Output/sample5.4.tex: Samples/sample5.4.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample5.4.xml Output/sample5.4.tex

./Output/sample1.4.tex: Samples/sample1.4.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample1.4.xml Output/sample1.4.tex

./Output/sample6.2.tex: Samples/sample6.2.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample6.2.xml Output/sample6.2.tex

./Output/sample7.3.tex: Samples/sample7.3.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample7.3.xml Output/sample7.3.tex

./Output/sample0.0.tex: Samples/sample0.0.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample0.0.xml Output/sample0.0.tex

./Output/sample1.2.tex: Samples/sample1.2.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample1.2.xml Output/sample1.2.tex

./Output/sample1.7.tex: Samples/sample1.7.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample1.7.xml Output/sample1.7.tex

./Output/sample4.3.tex: Samples/sample4.3.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample4.3.xml Output/sample4.3.tex

./Output/sample4.1.tex: Samples/sample4.1.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample4.1.xml Output/sample4.1.tex

tex: ./Output/sample5.4.tex ./Output/sample6.2.tex ./Output/sample4.1.tex ./Output/sample4.3.tex ./Output/sample7.3.tex ./Output/sample1.2.tex ./Output/sample1.7.tex ./Output/sample0.0.tex ./Output/sample1.4.tex

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

./Output/sample1.2.pdf: ./Output/sample1.2.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample1.2.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample1.2.tex

pdf: ./Output/sample5.4.pdf ./Output/sample6.2.pdf ./Output/sample4.1.pdf ./Output/sample4.3.pdf ./Output/sample7.3.pdf ./Output/sample1.2.pdf ./Output/sample1.7.pdf ./Output/sample0.0.pdf ./Output/sample1.4.pdf

clean:
	rm -rf ./Output/*

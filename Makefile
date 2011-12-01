all: clean pdf

./Output/sample7.3.tex: Samples/sample7.3.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample7.3.xml Output/sample7.3.tex

./Output/sample1.2.tex: Samples/sample1.2.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample1.2.xml Output/sample1.2.tex

tex: ./Output/sample7.3.tex ./Output/sample1.2.tex

./Output/sample7.3.pdf: ./Output/sample7.3.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample7.3.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample7.3.tex

./Output/sample1.2.pdf: ./Output/sample1.2.tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample1.2.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample1.2.tex

pdf: ./Output/sample7.3.pdf ./Output/sample1.2.pdf

clean:
	rm -rf ./Output/*

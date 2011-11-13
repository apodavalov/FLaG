all: clean pdf

./Output/tex/sample7.3.tex: Samples/sample7.3.xml
	[ -d ./Output ] || mkdir ./Output
	[ -d ./Output/tex ] || mkdir ./Output/tex
	mono Bin/FLaG.exe Samples/sample7.3.xml Output/tex/sample7.3.tex

tex: ./Output/tex/sample7.3.tex

./Output/sample7.3.pdf: tex
	[ -d ./Output/pdf ] || mkdir ./Output/pdf
	pdflatex --output-directory=./Output/pdf/ ./Output/tex/sample7.3.tex

pdf: ./Output/sample7.3.pdf

clean:
	rm -rf ./Output/

all: clean pdf

./Output/sample7.3.tex: Samples/sample7.3.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample7.3.xml Output/sample7.3.tex

tex: ./Output/sample7.3.tex

./Output/sample7.3.pdf: tex
	[ -d ./Output ] || mkdir ./Output
	pdflatex --output-directory=./Output ./Output/sample7.3.tex
# Hack! Make full translation in one time
	pdflatex --output-directory=./Output ./Output/sample7.3.tex

pdf: ./Output/sample7.3.pdf

clean:
	rm -rf ./Output/

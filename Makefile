all: clean pdf

./Output/sample7.3db.xml: Samples/sample7.3.xml
	[ -d ./Output ] || mkdir ./Output
	mono Bin/FLaG.exe Samples/sample7.3.xml Output/sample7.3db.xml

docbook: Output/sample7.3db.xml

./Output/tex/sample7.3.tex: docbook
	[ -d ./Output/tex ] || mkdir ./Output/tex
	dblatex -P latex.encoding=utf8 -P latex.unicode.use=1 -t tex -o ./Output/tex/sample7.3.tex ./Output/sample7.3db.xml

tex: ./Output/tex/sample7.3.tex

./Output/pdf/sample7.3.pdf: docbook
	[ -d ./Output/pdf ] || mkdir ./Output/pdf
	dblatex -P latex.encoding=utf8 -P latex.unicode.use=1 -o ./Output/pdf/sample7.3.pdf ./Output/sample7.3db.xml

pdf: ./Output/pdf/sample7.3.pdf

clean:
	rm -rf ./Output/

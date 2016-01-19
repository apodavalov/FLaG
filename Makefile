LATEXMK=latexmk
FLAG=Sources/FLaG/bin/Release/FLaG.exe
MONO=mono
OUTPUT=Output
SAMPLES=Samples

SOURCES=$(wildcard $(SAMPLES)/*.xml)
TEXS=$(patsubst $(SAMPLES)/%.xml, $(OUTPUT)/%.tex, $(SOURCES))
PDFS=$(patsubst $(OUTPUT)/%.tex, $(OUTPUT)/%.pdf, $(TEXS))

all: $(PDFS)

texs: $(TEXS)

$(OUTPUT):
	mkdir $(OUTPUT)

$(OUTPUT)/%.tex: $(SAMPLES)/%.xml $(OUTPUT)
	$(MONO) $(FLAG) $< $@

$(OUTPUT)/%.pdf: $(OUTPUT)/%.tex
	export buf_size=1000000; $(LATEXMK) -pdf --output-directory=$(OUTPUT) $<

%.tex: $(OUTPUT)/%.tex

%.pdf: $(OUTPUT)/%.pdf

clean:
	rm -rf $(OUTPUT)/*

.PHONY: texs all %.pdf %.tex clean

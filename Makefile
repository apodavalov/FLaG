LATEXMK=latexmk
FLAG=Sources/FLaG/bin/Release/net10.0/FLaG
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
	$(FLAG) $< $@

$(OUTPUT)/%.pdf: $(OUTPUT)/%.tex | $(OUTPUT)/svg-inkscape
	export buf_size=1000000; $(LATEXMK) -pdflua -lualatex="lualatex --shell-escape %O '\PassOptionsToPackage{inkscapepath=$(OUTPUT)/svg-inkscape/}{svg}\input{%S}'" --output-directory=$(OUTPUT) $<

$(OUTPUT)/svg-inkscape:
	mkdir -p $(OUTPUT)/svg-inkscape

%.tex: $(OUTPUT)/%.tex

%.pdf: $(OUTPUT)/%.pdf

clean:
	rm -rf $(OUTPUT)/*

.PHONY: texs all %.pdf %.tex clean

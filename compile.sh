#!/bin/sh

cd Sources
xbuild /property:Configuration=Release /target:clean
xbuild /property:Configuration=Release
cd ..
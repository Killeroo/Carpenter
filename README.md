# Carpenter - Static Photo Webpage Generator

Carpenter is a very basic tool intended to generate static webpages, mainly for showcasing photographs. 

The tool is designed to read a template file, which is then used to generate subsequent pages based on a `SCHEMA` file. The resulting page is a combination of the information contained in the schema and the template html file.

*The tool is not production ready by any means and currently requires the template file to be laid out in a very specific way*

# Usage

Run `carpenter` in whatever directory contains `template.html` (please look at the `template.html` file in the repo to see how this needs to be laid out), carpenter will then loop through each folder in that directory looking for a SCHEMA file and a collection of images.

# Motivation

I wanted to host my photos as simple static html files. Instead of having to constantly tinker with the HTML code of each page manually, I wrote this tool to generate the contents of each page for me.

# Examples

The contents of my [personnal photo site](https://matthewcarney.info/photos/feb-2021/) were all generated using Carpenter:

https://matthewcarney.info/photos/

# License 

```
MIT License

Copyright (c) 2022 Matthew Carney

```

#FoxEngine.TranslationTool
A bundle of tools related to text editing Fox Engine / MGSV Ground Zeroes files.

This bundle contains the following tools:

Name      | Description
--------- | ------------
FfntTool  | Font unpacker/repacker
LangTool  | String table unpacker/repacker
SubpTool  | Subtitle unpacker/repacker


##Requirements
```
Microsoft .NET Framework 4.5
```

##FfntTool
A Fox Engine bitmap font (.ffnt) unpacker and repacker.

###Usage
```
FfntTool file_path [output_path]
```

###Examples

Unpacking a font file. This will create the file called *KanjiFont.ffnt.xml* and a folder called *KanjiFont*. The font bitmaps will be exported as png files to the *KanjiFont* folder. Each layer of the bitmap font is exported as a single black and white png image.
```
FfntTool KanjiFont.ffnt
```

Repacking a font file. This will create the file called *KanjiFont.ffnt*. The bitmap font layers will be read and merged. Only pixels with the color white will be included in the resulting font.
```
FfntTool KanjiFont.ffnt.xml
```

##LangTool
A Fox Engine localizable string table (.lng) unpacker and repacker.

###Usage
```
LangTool file_path [output_path]
```

###Examples

Unpacking an .lng file. This will create the file *gz_menu.lng#eng.xml*.
```
LangTool gz_menu.lng#eng
```

Repacking an .lng file. This will create the file *gz_menu.lng#eng*
```
LangTool gz_menu.lng#eng.xml
```

##SubpTool
A Fox Engine subtitle pack unpacker and repacker.

###Usage
```
SubpTool [options] file_path [output_path]
```

###Options
The language of the file must be specified to correctly decode certain subtitles. Not specifying a language option will default to the ASCII encoding.

Option | Language   | Encoding
------ | ---------- | --------
-eng   | English    | ASCII
-ara   | Arabic     | UTF-8
-jpn   | Japanese   | UTF-8
-por   | Portuguese | UTF-8
-fre   | French     | ISO-8859-1
-ger   | German     | ISO-8859-1
-ita   | Italian    | ISO-8859-1
-spa   | Spanish    | ISO-8859-1
-rus   | Russian    | ISO-8859-5

###Examples

Unpacking a subtitle package.
```
SubpTool common.subp
```

Unpacking an encoded subtitle package.
```
SubpTool -jpn JpnText\common.subp
```

Repacking a subtitle package.
```
SubpTool common.subp.xml
```
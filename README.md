# epubed - EPUB Editor

Allows batch editing of EPUB files.

## Usage

Example:
```powershell
# shows titles of all books written in English language
epubed **/*.epub --where Content.Language=en --get Content.Title 

# shows books with broken content.opf file
epubed **/*.epub --where CanLoadContent=false

# fixes language for some EPUB books
epubed **/*.epub --where lang=UND --set lang=en
```

### Command-Line Keys

| Key | Values | Description
| ------------- | ------------- | ------------- |
| *(no key)* | file name | Specify the files to use. Must be relative to current directory or to the one specified with `--directory`. Globbing is possible, and ** is supported for recursive directory search.
| -d -dir --directory  |full path to directory | Set the root directory used to search for target files.
| -g --get | **PATH** | Adds specified EPUB path (see next section) to the result set.
| -s --set | **PATH**=*value* | Sets specified EPUB attribute in every file in the input set.
| -v --verb | **PATH**::**action** | Performs an action on every file in the input set. Possible actions are specified below.
| -w --where | **PATH**=*value* | If an attribute specified by PATH is not equal to the value, the file is removed from input set.

While processing files, filters are applied first, then getters, then setters, then verbs.

### EPUB Paths

Different attributes of an EPUB file are accessed through hierarchical paths:
| Path | Alias | Description
| ------------- | ------------- | ------------- |
| . | | Root path (full file path)
| CanLoadContent | | true if content.opf could be loaded, false otherwise
| Content | | Root for all metadata from content.opf


### 

## TODO

* Add more paths
* Allow globbing for root paths

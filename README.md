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
| Content.MetaCover | | Path to the cover picture inside the book archive
| Content.Title | | Book title
| Content.Creator | | Author
| Content.Publisher | | Publisher
| Content.Date | | Date of publishing
| Content.Identifier | | ISBN, UUID or another book identifier
| Content.Language | lang | [Language code](https://en.wikipedia.org/wiki/List_of_ISO_639-2_codes) for the book
| Content.Subject | | Book keywords (doesn't work for multiple subjects for now)
| Content.Description || Blurb for the book
| Content.Rights | | &copy; stuff
| Content.Type | | Dublin Core metadata specification. Always must be "Text".
| Content.Source | | An identifier (ISBN, UUID etc.) of book or series this EPUB is part of.
| Content.Relation | | An identifier (ISBN, UUID etc.) of book or series this EPUB relates to.
| Content.Coverage | | Dublin Core stuff
| Content.Contributor | | More authors (broken in the same way as subject)

### Verbs

Some changes cannot be expressed as simple setters, so instead you invoke them as a verb.
Any verb is fully specified by a path and a verb ID, separated by :: sigil.
If verb is applied to the the whole document, just leave out path, but keep the :: sigil.

| Path and Verb | Description
| ------------- | -------------
| ::fix-content-length | Removes 1 byte from content.opf document.

## Implementation Notes

* EPUB loading and manipulation is located in `epubed` namespace. The classes that end in -Model represent corresponding parts of book archive. All such classes are immutable, and receive required input from constructor parameters. They are also IDisposable, and must be released when disposed. Moreover, they must dispose all opened resources if an exception happens.
* Navigation, getters/setters, filters and verbs are located in `epubed.Query` namespace. These classes use `epubed.*` object model to manipulate EPUB data, but provide type-less uniform interface to all attributes. The main type here is `ITraversable`, which represents a node in EPUB data tree. It has a mutable Value property, allows to run verbs and access children.
* `QueryExecutor` contains main algorithm to run user-specified operations on every requested file. It contains multiple extensions points for resource management, error handling etc. SafeConsoleQueryExecutor is used by the app.
* Command line parsing is located in `epubed.CommandLine` namespace. The code should be refactored and possibly replaced by a library.

## TODO

* Add more paths
* Allow globbing for root paths
* Fix Content.Subject
* Refactor `epubed.CommandLine`

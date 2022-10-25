# Kardashev
Kardashev is a C#/.NET class library used to process MARC-21 records from a binary encoded .marc file.  The library provides a `MarcStreamReader` class that can be used to read the contents of large MARC-21 files as a stream while processing them in your consuming application.

![build-and-test](https://github.com/ECULibraries/kardashev/actions/workflows/build-and-test.yml/badge.svg)

## Why the name "Kardashev"
The Kardashev scale, proposed by Nikolai Kardashev in 1964, is a method of measuring a civilization's level of technological advancement based on the amount of energy it is able to use.  Given this, I felt it was the only accurate scale that could define the amount of energy it took to learn the MARC-21 format.  Based on this information and the amount of energy able and needed to use to understand MARC-21, this project would be rated as a Type III civilization; in possesion of energy at the scale of its own galazxy with an energy consumption of ≈4×10<sup>44</sup> marc-21/sec.

## Usage
### Reading Records From a MARC-21 File
To read records form a MARC-21 formatted file, you would use the `Kardashev.IO.MarcStreamReader` class.  This class is basically a wrapper around a `System.IO.FileStream` to read the contents of the MARC-21 file.  It inhertis from `IDisposable` since it wraps a stream, and should ideally be used within a `using` block.  It also inherits from `IEnumerable`, where the `GetEnumerator()` method does the heavy work of reading record by record.

The following example demonstrates how to use this to read a MARC-21 file named "records.marc":

```cs
using Kardashev;
using Kardashev.IO;

namespace MyApplication;

public class Program
{
    public static void Main(string[] args)
    {
        using(MarcStreamReader reader = new("records.marc"))
        {
            // Note that the foreach statement below specifies a nullable Record?
            foreach(Record? marcRecord in reader)
            {
                //  Do something with this record ...
                Console.WriteLine(marcRecord.ToString());
            }
        }
    }
}
```
The `MarcStreamReader` has an alternate constructor that takes a `MarcStreamReaderOptions` record type as well. This exposes two properties you can configure to change the behavior of the `MarcStreamReader`. These properties are as follows:



| Property | Type | Default Value | Description |
| --- | --- | --- | --- |
| `MarcStreamReader.ForceUtf8` | `bool` | `false` | Forces the record to be read as UTF-8 encoded regardless of the encoding scheme defined in the Leader[9] character of the record |
| `MarcStreamReader.SkipOnError` | `bool` | `true` |  When `true`, will skip the current record being processed and immediatly return `null` if an error is encountered that would hinder reading the remainder of the record.  When this is `false`, instead of skipping, an exception is thrown instead. |

### Extracting Data From a `Record` With Patterns
One of the primary reasons Kardashev was created was to assist in extracting data from records so that they could be indexed into a datastore, such as ElasticSearch.  As such, the utility class `Kardashev.PatternExtraction.PatternExtractor` is used for this.  The method of pattern extraction is based on the pattern specification used by [Traject](https://github.com/traject/traject), so if you are familar with that, then this shoudl be easy enough to pick up.

**When using the pattern extraction with thousands and thousands (or more) records, it is recommended to create the pattern extraction instances first and reuse them for each record processed instead of creating them on every record.  This is because the patterns themselves have to be parse, and over the course of thousands or millions of records, this adds up**.

Each `PatternExtractor` instance you create must supplied with a pattern and optionally given `ExtractorOptions`.  If no `ExtractorOptions` are given, then the default options are used. The following table details the avaialble values for the `ExtractorOptions` and the default values used if none are supplied:


| Property | Type | Default Value | Description |
| --- | --- | --- | --- |
| `First` | `bool` | `false` | When `true`, will extract only the first value found from the extraction pattern. This is useful when extracting data from a Varaible Field that can be repeated. |
| `TrimPuncuation` | `bool` | `false` | When `true`, trims common leading and trailing puncuation from the value extracted |
| `Default` | `string?` | `null` | When given, will use the value specified as the default value to return if an no values are found using the pattern. |
| `AllowDuplicates` | `bool` | `false` | When `false`, detects duplicate extracted values and returns only the unique value set |
| `Seperator` | `string?` |`null` | When given, all values extract will first be combined into a string value where each value is seperated by the seperator specified.  If this is not given, then all extracted values will remain as separate entries in the returned array. |
| `AlternateField` | `Kardashev.AlternateField` | `AlternateField.Include` | Specifies if Alternate Script data from linked Variable Data Field 880s should be included, not included, or to only extract the data from the 880 field and not the original field. |

### The Pattern String
The pattern string that must be given to the `PatternExtractor` has a few specifications depending on if you are extracting from a Variable Control Field or a Variable Data Field.  First, at minimum, the pattern must be

1. A minimum 3 characters in length
2. The first 3 characters of the pattern must be numerical, representing the tag of the field to extract from (e.g. `001`, `010`, `250`, etc).

Following the Tag value in the pattern, the remainder of the pattern depends on if it is for A Variable Control Field or Varaible Data Field.

### Varaible Control Field Patterns
The pattern specification for a Variable Control Field consists of a string that contains the Varaible Control Field tag value (`001` - `009`) and an option slice speicfication enclused in [] brackets.  The slice specification can be a single zero-based index value of the character to extract or an inclusive range where the start and end index are seperated by a '-' character.  If no slice is specified, then the entire data string for that field is extracted.

For example, let's say we had the following Varaible Control Field

```
005 19940223151047.0
```

The following table shows examples of valid pattern strings to extract from the above field and what data they would extract based on the pattern specification:

| Pattern      | Result             | Explination |
| ------------ | ------------------ | ----------- |
| `"005"`      | `19940223151047.0` | Only the tag value was given, so the entire data string will be extracted. |
| `"005[5]"`   | `2`                | A single slice value was given, specifying to extract the zero-based 5th chracter from the data string. |
| `"005[0-7]"` | `11940222`         | A range slice value was given, specifying to extract chracters 0 - 7 inclusivly (zero-based) from the data string. |

### Varaible Data Field Patterns
The pattern for a Varaible Data field consists of a string that contains the Varaible Data Field tag value (`010` - `999`), optional indicators enclosed within \|\| pipes, and optiona subfield codes. When supplying optional indicators, they must be enclused with the '\|' character at the start and end, and 2 indicator values must be given.  Indicator values must be alphanumeric (0 -9 or a -z) or a ' ' space, or it can be a '*' astrick to indicate any indicator value will match.

Subfield codes must be alphanumeric (0-9 or a-z).  When supplying subfield codes, only the values from those subfields will be extracted. If a subfield code is repeated (e.g. `250aa`, `250bb`), then multiple instances of that subfield code within the Varaible Data Field will be joined together with a ' ' space character.  When no subfield codes are supplied, then all subfields values are extracted.  If a value was given for the `ExtractorOptions.Seperator`, then all subfield values extracted will be combined into one string seperated by the seperator value given; otherwise, they will be extracted as separate values.

For example, let's say we had the following Varaible Data Field

```
270 1# $ECU Libraries$a1000 E 5th St.$bGreenville$cNC$dU.S.$e27858
```

The following table shows examples of valid pattern strings to extract from the above field and what the data they would extract based on the pattern specification
| Pattern      | Result             | Explination |
| ------------ | ------------------ | ----------- |
| `"270"`      | `["ECU Libraries", "1000 E 5th St.", "Greenville", "NC", "27858"]` | No subfield codes were specified, so all values would be extracted |
| `"270a"`     | `["ECU Libraries", "1000 E 5th St."]` | Only subfield code `a` was specified, and the field has two instances of that subfield, so both were extracted. |
| `"270aa"`    | `["ECU Libraries 1000 E 5th St."]`    | The pattern repeated the `a` specification and the field has two instances. So both were extracted and combined into a single value seperated by a ' ' space |
| `"270\|1*\|"`  | `["ECU Libraries", "1000 E 5th St.", "Greenville", "NC", "27858"]` | The `1*` indicators were specified, and this field does have a `1` indicator as the first indicator. No subfield codes were given, so all were extracted. |
| `"270\|2*\|`   | `[]`               | The `2*` indicators were specified, however this field does not have a `2` as the first indicator, so it does not match and no values would be extracted. |
| `"270\|1*\|b"` | `["Greenville"]`   | The `1*` indicators were specified, and this field does have a `1` indicator as the first indicator.  Subfield code `b` was also specified, so we only extracted that subfield value.

**Note: In the above examples, the extracted values are extracted into individual elements in the array. If a value were given for the `ExtractorOptions.Seperator`, the instead of multiple elements in the array, all elements would be combined into one string, seperated by the `ExtractorOptions.Seperator` value specified.**


### Multiple Patterns In One String
You can supply the `PatternExtractor` with multiple patterns in one string. Each pattern must be seperated by a ':' colon character, and you can mix Varaible Control Field patterns with Varaible Data Field patterns (e.g. `001:002[5]:250|1*|:270abc`).  A common use for this may be when extracting information such as the primary author of a title.  This value can be found in a few different fields.  So you might create a pattern such as `100abcdq:110abcd:111acde`.  This would be split into three seperate patterns `["100abcdq", "110abcd", "111acde"]` and then processed in the order they were given.  This is important incase you indicated `true` for the `ExtractorOptions.First` as that would indicate to extract the first value found.

### Using Pattern Example
The following is an example of using the `PaternExtractor` to extract the display title and the display author values from each record processed

```cs
using Kardashev;
using Kardashev.IO;
using Kardashev.PatternExtraction;

namespace MyApplication;

public class Program
{
    public static void Main(string[] args)
    {
        //  Create the pattern extractors first so they can be reused for
        //  each record instead of creating a new instance per record
        PatterExtractor titleExtractor = new("245abnpfg", new ExtractorOptions(TrimPuncuation: true, First: true));
        PatterExtractor authorExtractor = new("100abcdq:110abcd:111acde", new ExtractorOptions(TrimPuncuation: true, First: true));

        using(MarcStreamReader reader = new("records.marc"))
        {
            // Note that the foreach statement below specifies a nullable Record?
            foreach(Record? marcRecord in reader)
            {
                if(record != null)
                {
                    string[] titleResult = titleExtractor.Extract(marcRecord);
                    string[] authorResult = authorExtractor.Extract(marcRecord);

                    //  Do something with the results like putting them in a
                    //  data store?

                }
            }
        }
    }
}
```

## License
Kardashev is released under the MIT License with the exception of the `/source/Kardashev/Encodings/MARC8.cs` file, which is released under the GNU Lesser General Public License.  For more infomration on these individual licenses, please refer to the following
* Kardashev License - [MIT License Notice](LICENSE)
* MARC8.cs License - [GNU Lesser General Public Notice](MARC8LICENSE)

## Acknowledgement
I wanted to give awknowledgement to two other projects that were great sources of understanding as I went through reading up on the MARC-21 format and putting together the pattern parsing.

First would be the [CSharp_MARC](https://github.com/afrozenpeach/CSharp_MARC/) library by @afrozenpeach. This project was a great resource and provided the MARC8 Encoding port in C# as well.  The actual MARC Editor application itself is really cool and something that I've used a lot in the past when tracking down issues in MARC records tha we ingest.

Second would be [Traject](https://github.com/traject/traject) @traject, initially create by Jonathan Rochkind (John Hopkins Library) and Bill Dueber (University of Michigan).  This is where the bases for the pattern matching for this library comes from.  I was mostly familr with the pattern matching since traject is used under the hood in Project Blacklight. I wanted an already familar system in place when creating this library and did my best to mirror it.

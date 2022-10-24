/* -----------------------------------------------------------------------------
Copyright 2022 Christopher Whitley

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
----------------------------------------------------------------------------- */

namespace Kardashev.PatternExtraction;

public sealed class PatternExtractor
{
    //  Holds the extractors that were built from the given pattern.
    private List<IFieldExtractor> _extractors = new();

    //  Additional options to adhere to when extracting values.
    private ExtractorOptions _options;

    /// <summary>
    ///     Creates a new <see cref="PatternExtraction"/> class instance.
    /// </summary>
    /// <param name="pattern">
    ///     The pattern specification that describes the fields to extract
    ///     the values from.
    /// </param>
    /// <param name="options">
    ///     Additional options to adhere to when extracting values.
    /// </param>
    public PatternExtractor(string pattern, ExtractorOptions? options = default)
    {
        _options = options ?? new();
        ParsePattern(pattern);
    }

    /// <summary>
    ///     Parses the given pattern specification into individual extractor
    ///     instances to be used when extracting.
    /// </summary>
    /// <param name="pattern">
    ///     The pattern to parse.
    /// </param>
    /// <exception cref="InvalidPatternException">
    ///     Thrown if a pattern is invalid.
    /// </exception>
    private void ParsePattern(string pattern)
    {
        //  The specification for a pattern allows it to contain multiple
        //  patterns deliniated by a ':' character.  So we split on that
        string[] patterns = pattern.Split(':', StringSplitOptions.RemoveEmptyEntries);

        //  Go pattern-by-pattern and create the approprate extractor
        for (int i = 0; i < patterns.Length; i++)
        {
            //  For the pattern to be valid, it must at minimum be 3 characters
            //  in length.
            if (patterns[i].Length < 3)
            {
                throw new InvalidPatternException(patterns[i], "A pattern must be at minimum 3 characters in length");
            }

            //  The first 3 characters of the pattern must be numerical to
            //  represent the tag value of the field to extract from.
            if (!int.TryParse(patterns[i][0..3], out int tag))
            {
                throw new InvalidPatternException(patterns[i], "The first 3 characters of a pattern must be numerical to represent the field tag value");
            }

            //  Determine which type of extract to create based on the tag value
            //  < 10 = Control Field Extractor
            //  >= 10 = Data Field Extractor
            _extractors.Add(tag switch
            {
                int value when value < 10 => new ControlFieldExtractor(patterns[i]),
                _ => new DataFieldExtractor(patterns[i])
            });
        }
    }

    public string[] Extract(Record record)
    {
        //  Holds the values as we extract them
        List<string> extracted = new();

        //  Iterate all extractors and extract the values
        for (int i = 0; i < _extractors.Count; i++)
        {
            IFieldExtractor extractor = _extractors[i];

            string[] extractorResult = extractor.Extract(record, _options);

            //  Should we only take the first value found?
            if (extractorResult.Length > 0 && _options.First)
            {
                extracted.Add(extractorResult[0]);
                return extracted.ToArray();
            }
            else
            {
                extracted.AddRange(extractorResult);
            }
        }

        if (extracted.Count == 0 && _options.Default is not null)
        {
            extracted.Add(_options.Default);
        }

        //  Return the extracted results
        return extracted.ToArray();
    }
}

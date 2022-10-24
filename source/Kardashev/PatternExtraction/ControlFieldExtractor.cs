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

using System.Diagnostics.CodeAnalysis;
using Kardashev.Extensions;

namespace Kardashev.PatternExtraction;

/// <summary>
///     An extractor used to extract data from the Control Fields of a
///     <see cref="Record"/>.
/// </summary>
internal class ControlFieldExtractor : IFieldExtractor
{
    /// <summary>
    ///     Gets the Variable Control Field tag that this extractor will match
    ///     when extracting values.
    /// </summary>
    public string Tag { get; private set; }

    /// <summary>
    ///     Gets an optional <see cref="Range"/> value that defines a slice
    ///     of the Varaible Control Field's dat to extract instead of the
    ///     entire data string.
    /// </summary>
    public Range? Range { get; private set; } = default;

    /// <summary>
    ///     Creates a new <see cref="ControlFieldExtractor"/> class instance
    ///     initialized to extract data based on the <paramref name="pattern"/>
    ///     given.
    /// </summary>
    /// <param name="pattern">
    ///     The Data Field pattern that describes what data to extract.
    /// </param>
    public ControlFieldExtractor(string pattern) => ParsePattern(pattern);

    /// <summary>
    ///     Prases the pattern given to be used by this
    ///     <see cref="ControlFieldExtractor"/>
    /// </summary>
    /// <param name="pattern">
    ///     The pattern to parse.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     Thrown if the pattern given is an invalid control field extractor
    ///     pattern.
    /// </exception>
    [MemberNotNull(nameof(Tag))]
    private void ParsePattern(string pattern)
    {
        //  Control field patterns must be a minimum 3 characters that are
        //  numeric and define the tag of the field this will extract from.
        if (pattern.Length < 3)
        {
            throw new ArgumentException(nameof(pattern));
        }

        //  Get the tag
        Tag = pattern[0..3];

        //  Validate that the tag is numeric and that is is within the
        //  acceptable range for control fields (001 - 009)
        if (!int.TryParse(Tag, out int tag))
        {
            throw new ArgumentException(nameof(pattern));
        }
        else if (tag > 9)
        {
            throw new ArgumentException(nameof(pattern));
        }

        //  Remove the tag from the pattern
        pattern = pattern[3..];

        //  If there are still values in the pattern to parse then that means
        //  a slice was given
        if (pattern.Length > 0)
        {
            //  Slices must be between [] brackets. So the first and last
            //  characters remaining in the pattern must be the [] brackets
            if (pattern[0] != '[' || pattern[^1] != ']')
            {
                throw new ArgumentException(nameof(pattern));
            }

            //  Remove the [] brackets from the pattern
            pattern = pattern[1..^1];

            //  The reamining can be either a number that defines a single
            //  character position or it could be a range (two numbers seperated
            //  by a '-' hyphen).
            string[] split = pattern.Split('-', StringSplitOptions.RemoveEmptyEntries);

            int start;
            int end;

            //  Try to parse the start value out
            if (!int.TryParse(split[0], out start))
            {
                throw new ArgumentException(nameof(pattern));
            }

            //  If there's a second value, try to parse it for the end value
            if (split.Length > 1)
            {
                //  Try to parse out the end vlaue
                if (!int.TryParse(split[1], out end))
                {
                    throw new ArgumentException(nameof(pattern));
                }
            }
            else
            {
                //  There was not a second value, so the start and end are the
                //  same.
                end = start;
            }

            //  Regardless of the above, the end value needs to be incremented
            //  by 1 since our pattern is end 'inclusive' but C# Range.End is
            //  'exclusive'
            end++;

            Range = new(start, end);
        }
    }

    /// <summary>
    ///     Extracts data from the <paramref name="record"/> given  based on the
    ///     pattern used to initialize this <see cref="ControlFieldExtractor"/>.
    /// </summary>
    /// <param name="record">
    ///     The <see cref="Record"/> to extarct the data from.
    /// </param>
    /// <param name="options">
    ///     An <see cref="ExtractorOptions"/> value tha defines additional
    ///     optoins to use when extracting the data.
    /// </param>
    /// <returns>
    ///     A new <see cref="Array"/> of <see cref="string"/> elements where
    ///     each element is the data extracted.
    /// </returns>
    public string[] Extract(Record record, ExtractorOptions options)
    {
        //  Used to quickly check if a value is already added incase the options
        //  specify that duplicates aren't allowed.
        HashSet<string> duplicateLookup = new();

        //  Will hold the values that we extract.
        List<string> extracted = new();

        //  Even though most control fields are labeld NR (non-repeatable),
        //  some, such as field 006 are repeatble. So we just enumerate based
        //  on tag instead of pulling a single field
        foreach (ControlField field in record.EnumerateFieldsByTag(Tag))
        {
            string data;

            //  If a range was given in the patter, only extract the charcters
            //  within that range.
            if (Range is not null)
            {
                data = field.Data[Range.Value.Start..Range.Value.End];
            }
            else
            {
                //  No range defined, get the entire data value
                data = field.Data;
            }

            //  Check here for duplcate.  We could check after puncuation is
            //  trimmed, but it would be the same result regardless.  Doing it
            //  here those saves the step of trimming then checking
            if (!options.AllowDuplicates)
            {
                if (duplicateLookup.Contains(data))
                {
                    //  Just move on to the next enumeration
                    continue;
                }

                duplicateLookup.Add(data);
            }

            //  Should punctuation be trimmed?
            if (options.TrimPuncuation)
            {
                data = data.TrimPuncuation();
            }

            extracted.Add(data);

            //  If we were told to extract only the first value found, then
            //  break out early
            if (options.First)
            {
                break;
            }
        }

        return extracted.ToArray();
    }
}

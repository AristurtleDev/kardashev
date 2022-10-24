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

/// <summary>
///     Exposes methods for extracting data from a <see cref="Record"/>.
/// </summary>
internal interface IFieldExtractor
{
    /// <summary>
    ///     Extracts data from the <paramref name="record"/> given.
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
    string[] Extract(Record record, ExtractorOptions options);
}

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
///     Defines values that describe the how to include alternate script
///     fields when extracting data from a <see cref="Record"/> using an
///     <see cref="IFieldExtractor"/>.
/// </summary>
public enum AlternateField
{
    /// <summary>
    ///     Defines that alternate script fiels should be included along with
    ///     the original data fields they are linked too.
    /// </summary>
    Include,

    /// <summary>
    ///     Defines that alternate script fields should not be included.
    /// </summary>
    DontInclude,

    /// <summary>
    ///     Defines that only the alternate script fields should be included
    ///     and not the data fields they are linked too.
    /// </summary>
    Only
}

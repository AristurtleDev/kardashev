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
///     Creates a new <see cref="ExtractorOptions"/> record instance.
/// </summary>
/// <remarks>
///     Exposes configurable options for the a <see cref="MarcExtractor"/>
///     instance.
/// </remarks>
/// <param name="First">
///     <para>
///         Extract only the first value found from the extraction pattern.
///     </para>
///     <para>
///         The default value for this is <see langword="false"/>.
///     </para>
/// </param>
/// <param name="TrimPuncuation">
///     <para>
///         Trim leading and trailing puncuation marks from each value extracted.
///     </para>
///     <para>
///         The default value for this is <see langword="false"/>.
///     </para>
/// </param>
/// <param name="Default">
///     <para>
///         A default value to proided if no values are found.
///     </para>
///     <para>
///         When <see langword="null"/> and no value is found, then nothing
///         will be included for the value.
///     </para>
///     <para>
///         The default value for this is <see langword="null"/>.
///     </para>
/// </param>
/// <param name="AllowDuplicates">
///     <para>
///         Whether the result should include duplicate values.
///     </para>
///     <para>
///         The default value for this is <see langword="false"/>.
///     </para>
/// </param>
/// <param name="Seperator">
///     <para>
///         The seperator to use when combining multiple subfield values
///         togehter.
///     </para>
///     <para>
///         Set to <see langword="null"/> to leave all subfield values as
///         separate values when extracting.
///     </para>
///     <para>
///         The default is a " " space, except for patterns with only one
///         subfield, in which case the default will be <see langword="null"/>.
///     </para>
/// </param>
/// <param name="AlternateField">
///     <para>
///         An <see cref="AlternateField"/> enum value that specifies if
///         alternate data from a linked 880 field should be include, not
///         not included, or only use the alternate field data.
///     </para>
///     <para>
///         When <see cref="AlternateField.Include"/>, the data from the
///         original field plus the data from all liked 880 fields are included.
///     </para>
///     <para>
///         When <see cref="AlternateField.DontInclude"/>, the only the data
///         from the original field is used even when there is a field 880 with
///         linked data.
///     </para>
///     <para>
///         When <see cref="AlternateField.Only"/>, then only the data found in
///         a linked 880 field is included and the original field is not.
///     </para>
/// </param>
public record ExtractorOptions(bool First = false,
                                   bool TrimPuncuation = false,
                                   string? Default = default,
                                   bool AllowDuplicates = false,
                                   string? Seperator = default,
                                   AlternateField AlternateField = AlternateField.Include);

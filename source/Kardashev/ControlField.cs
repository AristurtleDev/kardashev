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

namespace Kardashev;

/// <summary>
///     Represents a Control Field in a MARC-21 formtted record.
/// </summary>
/// <remarks>
///     Control fields contain Tag values &lt; 10 and consist of only the Tag
///     and Data values.
/// </remarks>
public sealed class ControlField : Field
{
    /// <summary>
    ///     Gets or Sets the data value for this <see cref="ControlField"/>.
    /// </summary>
    public string Data { get; set; } = string.Empty;

    /// <summary>
    ///     Gets a value that indicates whether this <see cref="ControlField"/>
    ///     is considered emtpy.
    /// </summary>
    /// <remarks>
    ///     A <see cref="ControlField"/> is considered empty when the
    ///     <see cref="ControlField.Data"/> value is equal to
    ///     <see cref="string.Empty"/> or <see langword="null"/>.
    /// </remarks>
    public override bool IsEmpty => string.IsNullOrEmpty(Data);

    /// <summary>
    ///     Creates a new <see cref="ControlField"/> class instance initialized
    ///     with the values provided.
    /// </summary>
    /// <param name="tag">
    ///     The tag value for this <see cref="ControlField"/>.
    /// </param>
    /// <param name="data">
    ///     The data value for this <see cref="ControlField"/>.
    /// </param>
    public ControlField(string tag, string data) : base(tag) => Data = data;

    /// <summary>
    ///     Returns a new string containing the MARC-21 formatted representation
    ///     of this <see cref="ControlField"/>.
    /// </summary>
    /// <returns>
    ///     A new string containing the MARC-21 formatted representation of this
    ///     <see cref="ControlField"/>.
    /// </returns>
    public override string ToMarc() => $"{Data}{FIELD_TERMINATOR}";

    /// <summary>
    ///     Returns a new string representation of this
    ///     <see cref="ControlField"/>.
    /// </summary>
    /// <returns>
    ///     A new string represetnation of this <see cref="ControlField"/>.
    /// </returns>
    public override string ToString() => $"{base.ToString()} {Data}";

    /// <summary>
    ///     Validates that the <paramref name="tag"/> value given represents a
    ///     valid Tag value for a Variable Control Field.
    /// </summary>
    /// <remarks>
    ///     A Variable Control Field tag must be 3 characters in length that are
    ///     numerical characters representing a value less than 10.
    /// </remarks>
    /// <param name="tag">
    ///     The Tag value to validate.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the <paramref name="tag"/> given is valid;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    protected override bool ValidateTag(string tag) => tag.Length == 3 &&
                                                       int.TryParse(tag, out int value) &&
                                                       value < 10;
}

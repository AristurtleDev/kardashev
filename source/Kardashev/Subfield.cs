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
///     Represents a Subfield element of a Variable Data Field in a MARC-21
///     formatted record.
/// </summary>
/// <remarks>
///     Subfields consist of an indentifying code and data.
/// </remarks>
public sealed class Subfield
{
    /// <summary>
    ///     Gets or Sets the code value of this <see cref="Subfield"/>.
    /// </summary>
    public char Code { get; set; }

    /// <summary>
    ///     Gets or Sets the data value of this <see cref="Subfield"/>.
    /// </summary>
    public string Data { get; set; }

    /// <summary>
    ///     Gets a value that indicates whether this <see cref="Subfield"/>
    ///     is empty.
    /// </summary>
    /// <remarks>
    ///     A <see cref="Subfield"/> is considered empty when the
    ///     <see cref="Subfield.Data"/> value is equal to
    ///     <see cref="string.Empty"/> or is <see langword="null"/>.
    /// </remarks>
    public bool IsEmpty => string.IsNullOrEmpty(Data);

    /// <summary>
    ///     Creates a new <see cref="Subfield"/> class instance initialized
    ///     with the values provided.
    /// </summary>
    /// <param name="code">
    ///     The code value of this <see cref="Subfield"/>.
    /// </param>
    /// <param name="data">
    ///     The data value of this <see cref="Subfield"/>.
    /// </param>
    public Subfield(char code, string data) => (Code, Data) = (code, data);

    /// <summary>
    ///     Returns a new string containing the MARC-21 formatted representation
    ///     of this <see cref="Subfield"/>.
    /// </summary>
    /// <returns>
    ///     A new string containing the MARC-21 formatted representation of this
    ///     <see cref="Subfield"/>.
    /// </returns>
    public string ToMarc() => $"{SUBFIELD_DELIMINATOR}{Code}{Data}";

    /// <summary>
    ///     Returns a new string representation of this <see cref="Subfield"/>.
    /// </summary>
    /// <returns>
    ///     A new string representation of this <see cref="Subfield"/>.
    /// </returns>
    public override string ToString() => $"{Code}| {Data}";

}

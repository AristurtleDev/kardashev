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

namespace Kardashev;

/// <summary>
///     Represents a Field in a MARC-21 formatted record.
/// </summary>
/// <remarks>
///     Fields contain only a Tag value.
/// </remarks>
public abstract class Field
{
    private string _tag;

    /// <summary>
    ///     Gets or Sets the Tag value of this <see cref="Field"/>.
    /// </summary>
    public string Tag
    {
        get => _tag;

        [MemberNotNull(nameof(_tag))]
        set
        {
            if (!ValidateTag(value))
            {
                throw new ArgumentException($"The tag value '{value}' is not valid for this field", nameof(value));
            }

            _tag = value;
        }
    }

    /// <summary>
    ///     Gets a value that indicates whether this <see cref="Field"/> is a
    ///     <see cref="ControlField"/> type.
    /// </summary>
    public bool IsControlField => GetType() == typeof(ControlField);

    /// <summary>
    ///     Gets a value that indicates whether this <see cref="Field"/> is a
    ///     <see cref="DataField"/> type.
    /// </summary>
    public bool IsDataField => GetType() == typeof(DataField);

    /// <summary>
    ///     When overridden in a derivied class, gets a value that indicates
    ///     whether the field is empty.
    /// </summary>
    public abstract bool IsEmpty { get; }

    /// <summary>
    ///     Initializes a new <see cref="Field"/> instance with the
    ///     <paramref name="tag"/> value given.
    /// </summary>
    /// <param name="tag">
    ///     The tag value of this <see cref="Field"/>.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     Thrown if the <paramref name="tag"/> value given is not a valid Tag
    ///     value for the derived type.
    /// </exception>
    public Field(string tag) => Tag = tag;

    /// <summary>
    ///     When overridden in a derived class, returns a new string containing
    ///     the MARC-21 formatted representation of this <see cref="Field"/>.
    /// </summary>
    /// <returns>
    ///     A new string containing the MARC-21 formatted represetnation of this
    ///     <see cref="Field"/>.
    /// </returns>
    public abstract string ToMarc();

    /// <summary>
    ///     <para>
    ///         Returns a new string representation of this <see cref="Field"/>.
    ///     </para>
    ///     <para>
    ///         When overridden in a derviced class, returns a new string
    ///         represetnation of that type.
    ///     </para>
    /// </summary>
    /// <returns>
    ///     A new string represetnation of this <see cref="Field"/>.
    /// </returns>
    public override string ToString() => Tag;

    /// <summary>
    ///     When overridden in a derived class, validates that the
    ///     <paramref name="tag"/> value given represents a valid Tag value
    ///     for that type.
    /// </summary>
    /// <param name="tag">
    ///     The Tag value to validate.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the <paramref name="tag"/> given is valid;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    protected abstract bool ValidateTag(string tag);
}

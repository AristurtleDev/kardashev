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

namespace Kardashev.Serialization;

/// <summary>
///     Represents an exeption that can be thrown during the deserialization of
///     a MARC-21 record.
/// </summary>
public sealed class MarcDeserializationException : Exception
{
    /// <summary>
    ///     Gets the byte array buffer represetentation of the MARC reocrd
    ///     that was used for deserialization when this
    ///     <see cref="MarcDeserializationException"/> was thrown.
    /// </summary>
    /// <remarks>
    ///     This value is only avaialble if the exception was thrown during a
    ///     call to <see cref="MarcDeserializer.Deserialize(byte[], bool)"/>;
    ///     othewise, it will be <see langword="null"/>.
    /// </remarks>
    public byte[]? Buffer { get; set; }

    /// <summary>
    ///     Gets the <see cref="Record"/> that was being built when the
    ///     <see cref="MarcDeserializationException"/> occured.  This record
    ///     will not be a complete record and only what was partially built, but
    ///     will contain any information such as warnings that were added
    ///     up to the point of the error.
    /// </summary>
    public Record Record { get; set; }

    /// <summary>
    ///     Gets the string representation of the MARC record that was being
    ///     deseiralized when this <see cref="MarcDeserializationException"/>
    ///     was thrown.
    /// </summary>
    public string? Marc { get; set; }

    public MarcDeserializationException(string message, Record record, string marc, byte[]? buffer, Exception? innerException)
        : base(message, innerException) => (Record, Marc, Buffer) = (record, marc, buffer);
}

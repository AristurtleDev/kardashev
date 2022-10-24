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

namespace Kardashev.IO;

/// <summary>
///     Represents an except that can occur during the initialization of the
///     stream for a <see cref="MarcStreamReader"/> instance.
/// </summary>
/// <remarks>
///     This exception is an general wrapper for all exceptions that can occur
///     during stream initialization.  When thrown, refer to the inner exception
///     for the exact reason of the error.
/// </remarks>
public sealed class StreamInitializationException : Exception
{
    /// <summary>
    ///     Creates a new <see cref="StreamInitializationException"/> class
    ///     instance.
    /// </summary>
    /// <param name="message">
    ///     A message that details the reason the exception occured.
    /// </param>
    /// <param name="innerEx">
    ///     The inner exception that was the initial error.
    /// </param>
    public StreamInitializationException(string message, Exception innerEx)
        : base(message, innerEx) { }
}

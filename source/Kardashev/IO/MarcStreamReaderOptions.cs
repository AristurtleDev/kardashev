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
///     Creates a new <see cref="MarcExtractorOptions"/> record instance.
/// </summary>
/// <remarks>
///     Exposes configurable options for the a <see cref="MarcStreamReader"/>
///     instance.
/// </remarks>
/// <param name="ForceUtf8">
///     Whether UTF-8 encoding should be forced regardless if the record's
///     Leader[9] values specifies otherwise.
/// </param>
/// <param name="SkipOnError">
///     <para>
///         Whether the record being read should be skipped if an exception occurs
///         while reading the record.
///     </para>
///     <para>
///         If <see langword="false"/>, then the exception that occurs will be
///         thrown.
///     </para>
/// </param>
public sealed record MarcStreamReaderOptions(bool ForceUtf8 = false, bool SkipOnError = true);

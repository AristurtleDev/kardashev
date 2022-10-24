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

//  Global using so these constants are accessiable in all classes without the
//  need of additional using statements
global using static Globals;

/// <summary>
///     Exposes global constants used throughout the
///     <see cref="Kardashev.Marc"/> library.
/// </summary>
public static class Globals
{
    /// <summary>
    ///     The fixed byte length of a record leader (24-bytes).
    /// </summary>
    public const int LEADER_LENGTH = 24;

    /// <summary>
    ///     The fixed byte lenght of a directory entry (12-byte).
    /// </summary>
    public const int DIRECTORY_ENTRY_LENGTH = 12;

    /// <summary>
    ///     The character that represents the end of field terminator.
    /// </summary>
    public const char FIELD_TERMINATOR = '\x1E';

    /// <summary>
    ///     The character that represents the end of record terminator.
    /// </summary>
    public const char RECORD_TERMINATOR = '\x1D';

    /// <summary>
    ///     The character that represents the deliminator used to deliniate
    ///     subfields.
    /// </summary>
    public const char SUBFIELD_DELIMINATOR = '\x1F';

    /// <summary>
    ///     The character that represents an empty indicator for a data field.
    /// </summary>
    public const char EMPTY_INDICATOR = ' ';
}

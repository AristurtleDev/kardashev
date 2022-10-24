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

namespace Kardashev.Tests;

internal static class ConstTestValues
{
    //  Data Field Indicators
    public const char TEST_INDICATOR_1 = '1';
    public const char TEST_INDICATOR_2 = '2';

    //  Subfield Codes
    public const char TEST_SUBFIELD_CODE_1 = 'a';
    public const char TEST_SUBFIELD_CODE_2 = 'b';
    public const char TEST_SUBFIELD_CODE_3 = 'c';

    //  Test data for both ControlFields and Subfields
    //  Test data is from Ulysses by Alfred Lord Tennyson
    public const string TEST_DATA_1 = "It may be that the gulfs will wash us down, it may be we shall touch the Happy Isles";
    public const string TEST_DATA_2 = "though we are not now that strength which in old days moved earth and heaven, that which we are, we are";
    public const string TEST_DATA_3 = "made weak by time and fate, but strong in will, to strive, to seek, to find, and not to yield";
}

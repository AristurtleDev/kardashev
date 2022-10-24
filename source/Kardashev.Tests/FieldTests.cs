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

public class FieldTests
{
    [Fact]
    public void IsControlField_ReturnsTrue()
    {
        Field cField = new ControlField("001", TEST_DATA_1);
        bool shouldBeTrue = cField.IsControlField;
        Assert.True(shouldBeTrue);
    }

    [Fact]
    public void IsControlField_ReturnsFalse()
    {
        Field dField = new DataField("010", TEST_INDICATOR_1, TEST_INDICATOR_2);
        bool shoudlBeFalse = dField.IsControlField;
        Assert.False(shoudlBeFalse);
    }

    [Fact]
    public void IsDataField_ReturnsTrue()
    {
        Field dField = new DataField("010", TEST_INDICATOR_1, TEST_INDICATOR_2);
        bool shouldBeTrue = dField.IsDataField;
        Assert.True(shouldBeTrue);
    }

    [Fact]
    public void IsDataField_ReturnsFalse()
    {
        Field cField = new ControlField("001", TEST_DATA_1);
        bool shoudlBeFalse = cField.IsDataField;
        Assert.False(shoudlBeFalse);
    }
}

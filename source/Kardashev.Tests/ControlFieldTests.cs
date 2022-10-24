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

public class ControlFieldTests
{
    [Fact]
    public void Constructor_TagAndDataPropertiesSet()
    {
        string expectedTag = "001";
        string expectedData = TEST_DATA_1;

        ControlField field = new(expectedTag, expectedData);

        string actualTag = field.Tag;
        string actualData = field.Data;

        Assert.Equal(expectedTag, actualTag);
        Assert.Equal(expectedData, actualData);
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("NaN")]
    [InlineData("010")]
    [InlineData("09")]
    [InlineData("7")]
    [InlineData("")]
    public void Constructor_OnInvalidTagGiven_ThrowsException(string tag)
    {
        string data = string.Empty;

        Assert.Throws<ArgumentException>(() =>
        {
            return new ControlField(tag, data);
        });
    }

    [Fact]
    public void IsEmpty_ReturnsTrue()
    {
        ControlField field = new("001", string.Empty);
        bool shouldBeTrue = field.IsEmpty;

        Assert.True(shouldBeTrue);
    }

    [Fact]
    public void IsEmpty_ReturnsFalse()
    {
        ControlField field = new("001", TEST_DATA_1);
        bool shouldBeFalse = field.IsEmpty;

        Assert.False(shouldBeFalse);
    }

    [Fact]
    public void ToMarc_ReturnsExpectedString()
    {
        string tag = "001";
        string data = TEST_DATA_1;

        ControlField field = new(tag, data);

        string expected = $"{TEST_DATA_1}\u001E";
        string actual = field.ToMarc();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToString_ReturnsExpectedString()
    {
        string tag = "001";
        string data = TEST_DATA_1;

        ControlField field = new(tag, data);

        string expected = $"{tag} {data}";
        string actual = field.ToString();

        Assert.Equal(expected, actual);
    }
}

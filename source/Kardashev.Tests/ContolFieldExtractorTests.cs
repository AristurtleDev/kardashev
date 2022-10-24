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

using Kardashev.PatternExtraction;

namespace Kardashev.Tests;

public class ControlFieldExtractorTests
{
    [Theory]
    [InlineData("001")]
    [InlineData("002")]
    [InlineData("003")]
    [InlineData("004")]
    [InlineData("005")]
    [InlineData("006")]
    [InlineData("007")]
    [InlineData("008")]
    [InlineData("009")]
    public void Constructor_OnValidPatternGiven_TagTest_NoExceptionThrown(string pattern)
    {
        Exception? ex = Xunit.Record.Exception(() => new ControlFieldExtractor(pattern));
        Assert.Null(ex);
    }

    [Theory]
    [InlineData("001[1]")]
    [InlineData("001[1-2]")]
    [InlineData("001[01]")]
    [InlineData("001[01-02]")]
    public void Construcotr_OnValidPatternGiven_SliceTest_NoExceptionThrown(string pattern)
    {
        Exception? ex = Xunit.Record.Exception(() => new ControlFieldExtractor(pattern));
        Assert.Null(ex);
    }

    [Theory]
    [InlineData("0")]       //  Tag must be 3 characters minimum
    [InlineData("01")]      //  Tag must be 3 characters minimum
    [InlineData("010")]     //  Tag must be < 10
    [InlineData("00A")]     //  Tag must be numeric only
    public void Constructor_OnInvalidPatternGiven_InvalidTagTest_ThrowsExceptin(string pattern)
    {
        Exception? ex = Xunit.Record.Exception(() => new ControlFieldExtractor(pattern));
        Assert.NotNull(ex);
    }

    [Theory]
    [InlineData("001[1")]       //  Range slice must be enclosed in []'s
    [InlineData("0011]")]       //  Range slice must be enclosed in []'s
    [InlineData("001[A]")]      //  Range slice must be numeric only
    [InlineData("001[01:02")]   //  Range slice values must be seprated by '-'
    public void Constructor_OnInvalidPatternGiven_InvalidSliceTest_ThrowsExeption(string pattern)
    {
        Exception? ex = Xunit.Record.Exception(() => new ControlFieldExtractor(pattern));
        Assert.NotNull(ex);
    }



    [Theory]
    [InlineData("1")]           //  Tag must be 3 characters
    [InlineData("01")]          //  Tag must be 3 characters
    [InlineData("010")]         //  Tag must be < 10
    [InlineData("00A")]         //  Tag must be numerical
    [InlineData("001[1")]       //  Range must be enclosed in []
    [InlineData("0011]")]       //  Range must be enclosed in []
    [InlineData("001[A]")]      //  Range must be numerical
    [InlineData("001[A-1]")]    //  Both range values must be numerical
    [InlineData("001[1-A]")]    //  Both range values must be numerical
    public void Constructor_OnInvalidPatternGiven_ExceptionThrown(string pattern)
    {
        Exception? ex = Xunit.Record.Exception(() => new ControlFieldExtractor(pattern));
        Assert.NotNull(ex);
    }

    [Theory]
    [InlineData("001", "001", "abcd", "abcd")]
    [InlineData("001[2]", "001", "abcd", "c")]
    [InlineData("001[2-3]", "001", "abcd", "cd")]
    public void Extract_ReturnsExpectedValue(string pattern, string tag, string data, string expected)
    {
        //  Create the record with the test data
        Record record = new();
        record.AddField(new ControlField(tag, data));

        //  Create the extractor
        ControlFieldExtractor extractor = new(pattern);

        //  Extract the value
        string[] extracted = extractor.Extract(record, new());

        string actual = extracted[0];
        Assert.Equal(expected, actual);
    }
}

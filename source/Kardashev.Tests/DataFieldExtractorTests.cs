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

public class DataFieldExtractorTests
{
    [Fact]
    public void Constructor_OnValidPatternGiven_TagTest_NoExceptionThrown()
    {
        Exception? ex = Xunit.Record.Exception(() => new DataFieldExtractor("010"));
        Assert.Null(ex);
    }

    [Theory]
    [InlineData("010|12|")]
    [InlineData("010|1*|")]
    [InlineData("010|*2|")]
    [InlineData("010|1 |")]
    [InlineData("010| 2|")]
    [InlineData("010|a2|")]
    [InlineData("010|2a|")]
    public void Constructor_OnValidPatternGiven_IndicatorTest_NoExceptionThrown(string pattern)
    {
        Exception? ex = Xunit.Record.Exception(() => new DataFieldExtractor(pattern));
        Assert.Null(ex);
    }

    [Fact]
    public void Constructor_OnValidPatternGiven_SubfieldCodeTest_NoExceptionThrown()
    {
        string pattern = "010abcdefghijklmnopqrstuvwxyz0123456789";
        Exception? ex = Xunit.Record.Exception(() => new DataFieldExtractor(pattern));
        Assert.Null(ex);

    }

    [Fact]
    public void Constructor_OnValidPatternGiven_IncludingIndicators_IncludingSubfields_NoExceptionThrown()
    {
        string pattern = "010|12|abcdefghijklmnopqrstuvwxyz0123456789";
        Exception? ex = Xunit.Record.Exception(() => new DataFieldExtractor(pattern));
        Assert.Null(ex);
    }

    [Theory]
    [InlineData("0")]       //  Tag must be 3 characters minimum
    [InlineData("00")]      //  Tag must be 3 characters minimum
    [InlineData("009")]     //  Data field tag must be >= 10
    [InlineData("00A")]     //  Data field tag must be numeric only.
    public void Constructor_OnInvalidPatternGiven_InvalidTagTest_ExceptionThrown(string pattern)
    {
        Exception? ex = Xunit.Record.Exception(() => new DataFieldExtractor(pattern));
        Assert.NotNull(ex);
    }

    [Theory]
    [InlineData("010|")]        //  Indicators must be enclused within ||'s
    [InlineData("010|1")]       //  Indicators must be enclosed within ||'s
    [InlineData("0101|")]       //  Indicators must be enclosed withing ||'s
    [InlineData("010|1|")]      //  There must be two indicators given
    [InlineData("010|123|")]    //  There can only be two indicators given
    public void Constructor_OnInvalidPatternGiven_InvalidIndicatorsTest_ExceptionThrown(string pattern)
    {
        Exception? ex = Xunit.Record.Exception(() => new DataFieldExtractor(pattern));
        Assert.NotNull(ex);
    }

    [Theory]
    [InlineData("010|")]    //  Subfield code must be 0-9 or a-z.
    [InlineData("010.")]    //  Subfield code must be 0-9 or a-z.
    [InlineData("010A")]    //  Subfield code must be lowercase
    public void Constructor_OnInvalidPatternGiven_InvalidSubfieldCode_ExcetionThrown(string pattern)
    {
        Exception? ex = Xunit.Record.Exception(() => new DataFieldExtractor(pattern));
        Assert.NotNull(ex);
    }



    [Fact]
    public void Extract_ReturnsExpectedValues()
    {
        //  Create the record with the test data
        Record record = new();
        List<Subfield> subfields = new();
        subfields.Add(new(TEST_SUBFIELD_CODE_1, TEST_DATA_1));
        subfields.Add(new(TEST_SUBFIELD_CODE_2, TEST_DATA_2));
        record.AddField(new DataField("010", TEST_INDICATOR_1, TEST_INDICATOR_2, subfields));

        //  Create the extractor
        string pattern = $"010{TEST_SUBFIELD_CODE_1}";
        DataFieldExtractor extractor = new(pattern);

        //  Extract the value
        string[] extracted = extractor.Extract(record, new());

        string actual = extracted[0];

        Assert.Equal(TEST_DATA_1, actual);

    }
}

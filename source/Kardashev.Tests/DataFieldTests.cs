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

public class DataFieldTests
{
    [Fact]
    public void Constructor_OnNoSubfieldsGiven_TagAndIndicatorPropertiesSet()
    {
        string expectedTag = "010";
        char expectedIndicator1 = TEST_INDICATOR_1;
        char expectedIndicator2 = TEST_INDICATOR_2;

        DataField field = new(expectedTag, expectedIndicator1, expectedIndicator2);

        string actualTag = field.Tag;
        char actualIndicator1 = field.Indicator1;
        char actualIndicator2 = field.Indicator2;

        Assert.Equal(expectedTag, actualTag);
        Assert.Equal(expectedIndicator1, actualIndicator1);
        Assert.Equal(expectedIndicator2, actualIndicator2);
    }

    [Fact]
    public void Constructor_OnSubfieldsGiven_TagAndIndicatorsAndSubfieldCollectionSet()
    {
        Subfield subfield1 = new(TEST_SUBFIELD_CODE_1, TEST_DATA_1);
        Subfield subfield2 = new(TEST_SUBFIELD_CODE_2, TEST_DATA_2);

        List<Subfield> subfields = new();
        subfields.Add(subfield1);
        subfields.Add(subfield2);
        int expectedSubfieldCount = subfields.Count;

        string expectedTag = "010";
        char expectedIndicator1 = TEST_INDICATOR_1;
        char expectedIndicator2 = TEST_INDICATOR_2;

        DataField field = new(expectedTag, expectedIndicator1, expectedIndicator2, subfields);

        string actualTag = field.Tag;
        char actualIndicator1 = field.Indicator1;
        char actualIndicator2 = field.Indicator2;
        int actualSubfieldCount = field.Subfields.Count;

        Assert.Equal(expectedTag, actualTag);
        Assert.Equal(expectedIndicator1, actualIndicator1);
        Assert.Equal(expectedIndicator2, actualIndicator2);
        Assert.Equal(expectedSubfieldCount, actualSubfieldCount);
        Assert.Contains(subfield1, field.Subfields);
        Assert.Contains(subfield2, field.Subfields);
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("NaN")]
    [InlineData("009")]
    [InlineData("10")]
    [InlineData("")]
    public void Constructor_OnInvalidTagGiven_ThrowsException(string tag)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            return new DataField(tag, TEST_INDICATOR_1, TEST_INDICATOR_2);
        });
    }

    [Theory]
    [InlineData('1', '*')]
    [InlineData('*', '1')]
    public void Constructor_OnInvalidIndicatorGiven_ThrowsException(char indicator1, char indicator2)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            return new DataField("010", indicator1, indicator2);
        });
    }

    [Fact]
    public void IsEmpty_ReturnsTrue()
    {
        DataField field = new("010", TEST_INDICATOR_1, TEST_INDICATOR_2);

        bool shouldBeTrue = field.IsEmpty;

        Assert.True(shouldBeTrue);

    }

    [Fact]
    public void IsEmpty_ReturnsFalse()
    {
        Subfield subfield = new(TEST_SUBFIELD_CODE_1, TEST_DATA_1);
        List<Subfield> subfields = new() { subfield };

        DataField testField = new("010", TEST_INDICATOR_1, TEST_INDICATOR_2, subfields);

        bool shouldBeFalse = testField.IsEmpty;
        Assert.False(shouldBeFalse);
    }

    [Fact]
    public void EnumerateSubfieldsByCode_SubfieldCodesMatch_CountIterations()
    {
        Subfield subfield1 = new(TEST_SUBFIELD_CODE_1, TEST_DATA_1);
        Subfield subfield2 = new(TEST_SUBFIELD_CODE_2, TEST_DATA_2);

        List<Subfield> subfields = new() { subfield1, subfield2 };

        DataField field = new("010", TEST_INDICATOR_1, TEST_INDICATOR_2, subfields);

        int expectedIterationCount = 1;
        int actualIterationCount = 0;

        char expectedSubfieldCode = TEST_SUBFIELD_CODE_1;

        foreach (Subfield subfield in field.EnumerateSubfieldsByCode(expectedSubfieldCode))
        {
            char actualSubfieldCode = subfield.Code;
            Assert.Equal(expectedSubfieldCode, actualSubfieldCode);

            actualIterationCount++;
        }

        Assert.Equal(expectedIterationCount, actualIterationCount);
    }

    [Fact]
    public void GetFirstSubFieldByCode_ReturnsExpectedSubfield()
    {
        //  Two subfields usin the same code, but different data, to test if
        //  only the first one is returned
        Subfield subfield1 = new(TEST_SUBFIELD_CODE_1, TEST_DATA_1);
        Subfield subfield2 = new(TEST_SUBFIELD_CODE_1, TEST_DATA_2);
        Subfield subfield3 = new(TEST_SUBFIELD_CODE_2, TEST_DATA_3);

        List<Subfield> subfields = new() { subfield1, subfield2, subfield3 };

        DataField field = new("010", TEST_INDICATOR_1, TEST_INDICATOR_2, subfields);

        Subfield? result = field[TEST_SUBFIELD_CODE_1];

        Assert.Same(result, subfield1);
    }

    [Fact]
    public void GetSubfieldByIndex_ReturnsExpectedSubfield()
    {
        Subfield expectedSubfield1 = new(TEST_SUBFIELD_CODE_1, TEST_DATA_1);
        Subfield expectedSubfield2 = new(TEST_SUBFIELD_CODE_2, TEST_DATA_2);
        Subfield expectedSubfield3 = new(TEST_SUBFIELD_CODE_3, TEST_DATA_3);

        List<Subfield> subfields = new() { expectedSubfield1, expectedSubfield2, expectedSubfield3 };

        DataField field = new("010", TEST_INDICATOR_1, TEST_INDICATOR_2, subfields);

        Subfield actualSubfield1 = field[0];
        Subfield actualSubfield2 = field[1];
        Subfield actualSubfield3 = field[2];

        Assert.Same(expectedSubfield1, actualSubfield1);
        Assert.Same(expectedSubfield2, actualSubfield2);
        Assert.Same(expectedSubfield3, actualSubfield3);
    }

    [Fact]
    public void GetEnumerator_EnumeratesAllSubfields()
    {
        Subfield expectedSubfield1 = new(TEST_SUBFIELD_CODE_1, TEST_DATA_1);
        Subfield expectedSubfield2 = new(TEST_SUBFIELD_CODE_2, TEST_DATA_2);
        Subfield expectedSubfield3 = new(TEST_SUBFIELD_CODE_3, TEST_DATA_3);

        List<Subfield> subfields = new() { expectedSubfield1, expectedSubfield2, expectedSubfield3 };

        DataField field = new("010", TEST_INDICATOR_1, TEST_INDICATOR_2, subfields);

        List<Subfield> actualSubfields = new();
        foreach (Subfield subfield in field)
        {
            actualSubfields.Add(subfield);
        }

        Assert.Contains(expectedSubfield1, actualSubfields);
        Assert.Contains(expectedSubfield2, actualSubfields);
        Assert.Contains(expectedSubfield3, actualSubfields);
    }

    [Fact]
    public void ToMarc_ReturnsExpectedString()
    {
        Subfield expectedSubfield1 = new(TEST_SUBFIELD_CODE_1, TEST_DATA_1);
        List<Subfield> subfields = new() { expectedSubfield1 };

        string tag = "010";
        DataField field = new(tag, TEST_INDICATOR_1, TEST_INDICATOR_2, subfields);

        string expected = $"{TEST_INDICATOR_1}{TEST_INDICATOR_2}\u001F{TEST_SUBFIELD_CODE_1}{TEST_DATA_1}\u001E";
        string actual = field.ToMarc();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToString_ReturnsExpectedString()
    {
        Subfield expectedSubfield1 = new(TEST_SUBFIELD_CODE_1, TEST_DATA_1);
        List<Subfield> subfields = new() { expectedSubfield1 };

        string tag = "010";
        DataField field = new(tag, TEST_INDICATOR_1, TEST_INDICATOR_2, subfields);

        string expected = $"{tag} {TEST_INDICATOR_1} {TEST_INDICATOR_2} {TEST_SUBFIELD_CODE_1}| {TEST_DATA_1}";
        string actual = field.ToString();

        Assert.Equal(expected, actual);
    }

}

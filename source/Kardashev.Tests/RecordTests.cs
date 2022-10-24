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

public class RecordTests
{
    [Fact]
    public void AddTwoFields_ThenGetFieldByIndex_ReturnsExpectedField()
    {
        Field field001 = new ControlField("001", TEST_DATA_1);
        Field expectedField = new DataField("100", TEST_INDICATOR_1, TEST_INDICATOR_2);

        Record record = new();
        record.AddField(field001);
        record.AddField(expectedField);

        Field actualField = record[1];

        Assert.Same(expectedField, actualField);
    }

    [Fact]
    public void AddTwoFields_RemoveAField_CountReturnsOne()
    {
        Field field001 = new ControlField("001", TEST_DATA_1);
        Field field100 = new DataField("100", TEST_INDICATOR_1, TEST_INDICATOR_2);

        Record record = new();
        record.AddField(field001);
        record.AddField(field100);

        record.RemoveField(field001);

        int expectedCount = 1;
        int actualCount = record.Count;

        Assert.Equal(expectedCount, actualCount);
    }

    [Fact]
    public void AddTwoFields_CountsReturnTwo()
    {
        Field field001 = new ControlField("001", TEST_DATA_1);
        Field field100 = new DataField("100", TEST_INDICATOR_1, TEST_INDICATOR_2);

        Record record = new();
        record.AddField(field001);
        record.AddField(field100);

        int expectedCount = 2;
        int actualCount = record.Count;

        Assert.Equal(expectedCount, actualCount);
    }

    [Fact]
    public void AddFourFields_ThreeWithSameTag_GetFieldsByTag_CountReturnsThree()
    {
        string expectedTag = "001";

        Field field1 = new ControlField(expectedTag, TEST_DATA_1);
        Field field2 = new ControlField(expectedTag, TEST_DATA_2);
        Field field3 = new ControlField(expectedTag, TEST_DATA_3);
        Field field4 = new DataField("010", TEST_INDICATOR_1, TEST_INDICATOR_2);

        Record record = new();
        record.AddField(field1);
        record.AddField(field2);
        record.AddField(field3);
        record.AddField(field4);

        List<Field> actualFields = record.GetFields(expectedTag);

        int expectedCount = 3;
        int actualCount = actualFields.Count;

        Assert.Equal(expectedCount, actualCount);
        Assert.Contains(field1, actualFields);
        Assert.Contains(field2, actualFields);
        Assert.Contains(field3, actualFields);
        Assert.DoesNotContain(field4, actualFields);
    }

    [Fact]
    public void AddFourFields_GetEnumerator_AllFieldsEnumerated()
    {
        Field field1 = new ControlField("001", TEST_DATA_1);
        Field field2 = new ControlField("002", TEST_DATA_2);
        Field field3 = new DataField("010", TEST_INDICATOR_1, TEST_INDICATOR_2);
        Field field4 = new DataField("020", TEST_INDICATOR_1, TEST_INDICATOR_2);

        Record record = new();
        record.AddField(field1);
        record.AddField(field2);
        record.AddField(field3);
        record.AddField(field4);

        List<Field> actualFields = new();

        foreach (Field field in record)
        {
            actualFields.Add(field);
        }

        Assert.Contains(field1, actualFields);
        Assert.Contains(field2, actualFields);
        Assert.Contains(field3, actualFields);
        Assert.Contains(field4, actualFields);
    }

    [Fact]
    public void ToString_ReturnsExpectedString()
    {
        string tag1 = "001";
        string tag2 = "010";

        //  Create the Record instance
        Record record = new();

        //  Create the ControlField and add to record
        record.AddField(new ControlField(tag1, TEST_DATA_1));

        //  Create a DataField and add to record
        Subfield subfield = new(TEST_SUBFIELD_CODE_1, TEST_DATA_2);
        List<Subfield> subfields = new() { subfield };
        record.AddField(new DataField(tag2, TEST_INDICATOR_1, TEST_INDICATOR_2, subfields));

        //  Create an empty ControlField and add to record. This should be
        //  skipped in the ToString process and not be part of the output
        record.AddField(new ControlField("002", string.Empty));

        //  Create an empty DataField and add to record.  This should be skipped
        //  in the ToString process and not be part of the output.
        record.AddField(new DataField("020", '1', '2'));

        string expected = string.Format("LEADER 00243     2200049   4500{0}{1} {2}{0}{3} {4} {5} {6}| {7}",
            Environment.NewLine,
            tag1,
            TEST_DATA_1,
            tag2,
            TEST_INDICATOR_1,
            TEST_INDICATOR_2,
            TEST_SUBFIELD_CODE_1,
            TEST_DATA_2);

        string actual = record.ToString();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToMarc_ReturnsExpectedString()
    {
        string tag1 = "001";
        string tag2 = "010";

        //  Create the Record instance
        Record record = new();

        //  Create the ControlField and add to record
        record.AddField(new ControlField(tag1, TEST_DATA_1));

        //  Create a DataField and add to record
        Subfield subfield = new(TEST_SUBFIELD_CODE_1, TEST_DATA_2);
        List<Subfield> subfields = new() { subfield };
        record.AddField(new DataField(tag2, TEST_INDICATOR_1, TEST_INDICATOR_2, subfields));

        //  Create an empty ControlField and add to record. This should be
        //  skipped in the ToString process and not be part of the output
        record.AddField(new ControlField("002", string.Empty));

        //  Create an empty DataField and add to record.  This should be skipped
        //  in the ToString process and not be part of the output.
        record.AddField(new DataField("020", '1', '2'));

        string expected = string.Format("00243    82200049   4500{0}008500000{1}010800085\u001E{2}\u001E{3}{4}\u001F{5}{6}\u001E\u001D",
            tag1,
            tag2,
            TEST_DATA_1,
            TEST_INDICATOR_1,
            TEST_INDICATOR_2,
            TEST_SUBFIELD_CODE_1,
            TEST_DATA_2
            );

        string actual = record.ToMarc(System.Text.Encoding.UTF8);

        Assert.Equal(expected, actual);
    }
}

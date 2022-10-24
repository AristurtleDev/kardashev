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

using System.Text;
using Kardashev.Serialization;
using MARC;

namespace Kardashev.Tests;

public class DeserializerTests
{
    private string _singleControlFieldMarc = "00043    82200037   4500001000400000\u001Eaaaa\u001E\u001D";

    private static Record CreateTestRecord()
    {
        Record record = new Record();
        record.AddField(new ControlField("001", TEST_DATA_1));
        record.AddField(new ControlField("002", TEST_DATA_2));
        record.AddField(new DataField("010", TEST_INDICATOR_1, TEST_INDICATOR_2));

        List<Subfield> subfields = new();
        subfields.Add(new(TEST_SUBFIELD_CODE_1, TEST_DATA_3));
        record.AddField(new DataField("020", TEST_INDICATOR_1, TEST_INDICATOR_2, subfields));

        return record;
    }

    [Fact]
    public void Deserialize_FromBytes_UTF8Encoding_ReturnsExcpected()
    {
        //  Create the test record
        Record expected = CreateTestRecord();

        //  Get the MARC-21 representation of the record
        string marc = expected.ToMarc(Encoding.UTF8);

        //  Create the byte encoded representation
        byte[] asBytes = Encoding.UTF8.GetBytes(marc);

        //  Deserialize byte bytes back into a record
        Record actual = MarcDeserializer.Deserialize(asBytes);

        //  Compare
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Deserialize_FromBytes_MARC8Encoding_ReturnsExpected()
    {
        //  Create the test record
        Record expected = CreateTestRecord();

        //  Get the MARC-21 representation of the record
        string marc = expected.ToMarc(new MARC8());

        //  Create the byte encoded representation
        byte[] asBytes = Encoding.UTF8.GetBytes(marc);

        //  Deserialize byte bytes back into a record
        Record actual = MarcDeserializer.Deserialize(asBytes);

        //  Compare
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Deserialize_FromString_UTF8Encoding_ReturnsExpected()
    {
        //  Create the test record
        Record expected = CreateTestRecord();

        //  Get the MARC-21 representation of the record
        string marc = expected.ToMarc(Encoding.UTF8);

        //  Deserialize byte bytes back into a record
        Record actual = MarcDeserializer.Deserialize(marc);

        //  Compare
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Deserialize_FromString_MARC8Encoding_ReturnsExpected()
    {
        //  Create the test record
        Record expected = CreateTestRecord();

        //  Get the MARC-21 representation of the record
        string marc = expected.ToMarc(new MARC8());

        //  Deserialize byte bytes back into a record
        Record actual = MarcDeserializer.Deserialize(marc);

        //  Compare
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Deserialize_FromString_InvalidRecordLengthInLeader_ThrowsException()
    {
        //  A marc string with an invalid leader.
        //                 Invalid character 'A'
        //                 v
        string marc = "0004A    82200037   4500001000400000\u001Eaaaa\u001E\u001D";

        Exception? ex = Xunit.Record.Exception(() => MarcDeserializer.Deserialize(marc));
        Assert.NotNull(ex);
    }

    [Fact]
    public void Deserialize_FromString_RecordLengthValueLargerThanActualLength_ThrowsException()
    {
        //  A marc string with an invalid leader.
        //             Record not 99999 bytes in length
        //             v
        string marc = "99999    82200037   4500001000400000\u001Eaaaa\u001E\u001D";

        Exception? ex = Xunit.Record.Exception(() => MarcDeserializer.Deserialize(marc));
        Assert.NotNull(ex);
    }

    [Fact]
    public void Deserialize_FromString_InvalidBaseAddressOfDataValueInLeader_ThrowsException()
    {
        //  A marc string with an invalid leader.
        //                             Invalid character 'A'
        //                             v
        string marc = "00043    8220003A   4500001000400000\u001Eaaaa\u001E\u001D";

        Exception? ex = Xunit.Record.Exception(() => MarcDeserializer.Deserialize(marc));
        Assert.NotNull(ex);
    }

    [Fact]
    public void Deserialize_FromSTring_BaseAddressOfDataValueLargerThanRecordLength_ThrowsException()
    {
        //  A marc string with an invalid leader.
        //                         Base Address does not start at 99999
        //                         v
        string marc = "00043    82299999   4500001000400000\u001Eaaaa\u001E\u001D";

        Exception? ex = Xunit.Record.Exception(() => MarcDeserializer.Deserialize(marc));
        Assert.NotNull(ex);
    }

    [Fact]
    public void Deserialize_FromString_LeaderDoesNotEndIn4500_ThrowsException()
    {
        //  A marc string with an invalid leader.
        //                                 Not 4500
        //                                 v
        string marc = "00043    82200037   4501001000400000\u001Eaaaa\u001E\u001D";

        Exception? ex = Xunit.Record.Exception(() => MarcDeserializer.Deserialize(marc));
        Assert.NotNull(ex);
    }

    [Fact]
    public void Deserialize_FromString_RecordTerminatorMissing_GivesWarning()
    {
        //  A valid marc string with one 001 Control Field containing the data
        //  "aaaa", but the record terminator is missing from the end.
        string marc = "00042    82200037   4500001000400000\u001Eaaaa\u001E";

        //  Create the record
        Record record = MarcDeserializer.Deserialize(marc);

        //  Ensure the warning
        int expectedWarningCount = 1;
        int actualWarningCount = record.Warnings.Count;
        Assert.Equal(expectedWarningCount, actualWarningCount);

        string expectedWarningText = "Record does not end with a Record Terminator (hex 1D).";
        string actualWarningText = record.Warnings[0];
        Assert.Equal(expectedWarningText, actualWarningText);
    }

    [Fact]
    public void Deserialize_FromString_DirectoryContainsExtraCharacters_GivesWarning()
    {
        //  A valid marc string with one 001 Control Field containing the data
        //  "aaaa", but the record terminator is missing from the end.
        //  A valid marc string with one 001 Control Field containig the data
        //  "aaaa", but the directory contains 1 extract chracter making the
        //  expected directory length wrong.
        //                                                 Extra '0' added
        //                                                 v
        string marc = "00044    82200038   45000010004000000\u001Eaaaa\u001E\u001D";

        //  Create the record
        Record record = MarcDeserializer.Deserialize(marc);

        //  Ensure the warning
        int expectedWarningCount = 1;
        int actualWarningCount = record.Warnings.Count;
        Assert.Equal(expectedWarningCount, actualWarningCount);

        string expectedWarningText = "Directory contains 1 extra chracter(s).  Removing extra characters";
        string actualWarningText = record.Warnings[0];
        Assert.Equal(expectedWarningText, actualWarningText);
    }

    [Fact]
    public void Deserialize_FromString_DirectoryContainsMoreEntriesThanActualFields_ThrowsException()
    {
        //  Invalid marc string, the direcotry contains 2 field entries but
        //  there is only 1 field in the record.
        string marc = "00055    82200049   4500001000400000002000400005\u001Eaaaa\u001E\u001D";

        //  Check for exception
        Exception? ex = Xunit.Record.Exception(() => MarcDeserializer.Deserialize(marc));
        Assert.NotNull(ex);
    }

    [Fact]
    public void Deserialize_FromString_DirectoryEntryContainsInvalidTag_ThrowsException()
    {
        //  Invalid marc string, the directory contains an entry with an invalid
        //  tag value.
        //                                       Invalid character in tag value
        //                                       V
        string marc = "00043    82200037   450000X000400000\u001Eaaaa\u001E\u001D";

        //  Check for exception
        Exception? ex = Xunit.Record.Exception(() => MarcDeserializer.Deserialize(marc));
        Assert.NotNull(ex);
    }

    [Fact]
    public void Deserialize_FromString_DataFieldContainsTooManyIndicators_GivesWarning()
    {
        //  Valid marc string, however the indicator values for the Data Field
        //  contain too many indicators..  The deserializer should force them to
        //  be blanks instead.
        //                                                         Third indicator, invalid.
        //                                                         v
        string marc = "00048    82200037   4500010001000000\u001E123\u001Faaaaa\u001E\u001D";

        Record record = MarcDeserializer.Deserialize(marc);
        DataField? field = (DataField?)record.GetFields("010").FirstOrDefault();

        if (field is null)
        {
            Assert.True(false, "Record does not contain the expected data field");
        }
        else
        {
            int expectedWarningCount = 1;
            int actualWarningCount = record.Warnings.Count;
            Assert.Equal(expectedWarningCount, actualWarningCount);

            char expectedIndicator = ' ';
            char actualIndicator1 = field.Indicator1;
            Assert.Equal(expectedIndicator, actualIndicator1);

            char actualIndicator2 = field.Indicator2;
            Assert.Equal(expectedIndicator, actualIndicator2);
        }
    }

    [Fact]
    public void Deserialize_FromString_DataFieldContainsInvalidFirstIndicator_GivesWarning()
    {
        //  Valid marc string, howeer, the first indicator for the Data Field
        //  contains an invalid first indicator character.  The deserializer
        //  should force it to blank and give warning.
        //                                                       Invalid indicator value
        //                                                       v
        string marc = "00047    82200037   4500010000900000\u001E*1\u001Faaaaa\u001E\u001D";

        Record record = MarcDeserializer.Deserialize(marc);
        DataField? field = (DataField?)record.GetFields("010").FirstOrDefault();

        if (field is null)
        {
            Assert.True(false, "Record does not contain the expected data field");
        }
        else
        {
            int expectedWarningCount = 1;
            int actualWarningCount = record.Warnings.Count;
            Assert.Equal(expectedWarningCount, actualWarningCount);

            char expectedIndicator = ' ';
            char actualIndicator = field.Indicator1;
            Assert.Equal(expectedIndicator, actualIndicator);
        }
    }

    [Fact]
    public void Deserialize_FromString_DataFieldContainsInvalidSecondIndicator_GivesWarning()
    {
        //  Valid marc string, howeer, the first indicator for the Data Field
        //  contains an invalid second indicator character.  The deserializer
        //  should force it to blank and give warning.
        //                                                        Invalid indicator value
        //                                                        v
        string marc = "00047    82200037   4500010000900000\u001E1*\u001Faaaaa\u001E\u001D";

        Record record = MarcDeserializer.Deserialize(marc);
        DataField? field = (DataField?)record.GetFields("010").FirstOrDefault();

        if (field is null)
        {
            Assert.True(false, "Record does not contain the expected data field");
        }
        else
        {
            int expectedWarningCount = 1;
            int actualWarningCount = record.Warnings.Count;
            Assert.Equal(expectedWarningCount, actualWarningCount);

            char expectedIndicator = ' ';
            char actualIndicator = field.Indicator2;
            Assert.Equal(expectedIndicator, actualIndicator);
        }
    }

    [Fact]
    public void Deserlaize_FromString_DataFieldContainsZeroLengthSubfield_GivesWarning()
    {
        //  Valid marc string, however the Data Field contains a subfield with
        //  a zero length and no actual value.
        //  zero length and has no actual value.
        string marc = "00048    82200037   4500010001000000\u001E11\u001faaaaa\u001F\u001E\u001D";

        Record record = MarcDeserializer.Deserialize(marc);
        DataField? field = (DataField?)record.GetFields("010").FirstOrDefault();

        if (field is null)
        {
            Assert.True(false, "Record does not contain the expected data field");
        }
        else
        {
            int expectedWarningCount = 1;
            int actualWarningCount = record.Warnings.Count;
            Assert.Equal(expectedWarningCount, actualWarningCount);
        }
    }

    [Fact]
    public void Deserlaize_FromString_DataFieldContainsNoSubfield_GivesWarning()
    {
        //  Valid marc string, however the Data Field containsno subfields.
        //  The deserializer should produce a warning.
        string marc = "00041    82200037   4500010000300000\u001E11\u001E\u001D";

        Record record = MarcDeserializer.Deserialize(marc);
        DataField? field = (DataField?)record.GetFields("010").FirstOrDefault();

        if (field is null)
        {
            Assert.True(false, "Record does not contain the expected data field");
        }
        else
        {
            int expectedWarningCount = 1;
            int actualWarningCount = record.Warnings.Count;
            Assert.Equal(expectedWarningCount, actualWarningCount);
        }
    }
}

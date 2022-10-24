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

using System.Reflection;
using Kardashev.IO;

namespace Kardashev.Tests;

public class MarcStreamReaderTests
{
    private string _singleRecordPath;
    private string _oneHundredRecordsPath;
    private string _firstRecordInvalidPath;

    public MarcStreamReaderTests()
    {
        string? root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        _singleRecordPath = Path.Join(root, "Files", "single-record.marc");
        _oneHundredRecordsPath = Path.Join(root, "Files", "one-hundred-records.marc");
        _firstRecordInvalidPath = Path.Join(root, "Files", "first-record-invalid.marc");

        if (!File.Exists(_singleRecordPath))
        {
            throw new InvalidOperationException($"Unable to locate MARC file for tests.  Locations searched are: '{_singleRecordPath}'");
        }

        if (!File.Exists(_oneHundredRecordsPath))
        {
            throw new InvalidOperationException($"Unable to locate MARC file for tests.  Locations searched are: '{_oneHundredRecordsPath}'");
        }

        if (!File.Exists(_firstRecordInvalidPath))
        {
            throw new InvalidOperationException($"Unable to locate MARC file for tests.  Locations searched are: '{_firstRecordInvalidPath}'");
        }
    }

    [Fact]
    public void Constructor_OnGivenValidFilePath_DoesNotThrowException()
    {
        Exception? ex = Xunit.Record.Exception(() =>
        {
            MarcStreamReader reader = new(_singleRecordPath);
            reader.Dispose();
        });

        Assert.Null(ex);
    }

    [Fact]
    public void Constructor_OnGivenInvalidFilePath_ThrowException()
    {
        Exception? ex = Xunit.Record.Exception(() => new MarcStreamReader(""));
        Assert.NotNull(ex);
    }

    [Fact]
    public void GetEnumerator_SingleRecordFile_CountExpectToBeOne()
    {
        using MarcStreamReader reader = new(_singleRecordPath);

        int expected = 1;
        int actual = 0;

        foreach (Record record in reader)
        {
            actual++;
        }

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetEnumerator_OneHundredRecodsFile_CountExpectedToBeOneHundred()
    {
        using MarcStreamReader reader = new(_oneHundredRecordsPath);

        int expected = 100;
        int actual = 0;

        foreach (Record record in reader)
        {
            actual++;
        }

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetEnumerator_SkipOnError()
    {
        MarcStreamReaderOptions options = new(SkipOnError: true);

        using MarcStreamReader reader = new(_firstRecordInvalidPath, options);

        int expected = 1;
        int actual = 0;

        foreach (Record? record in reader)
        {
            if (record is not null)
            {
                actual++;
            }
        }

        Assert.Equal(expected, actual);
        Assert.Equal(expected, reader.Exceptions.Count);
    }
}

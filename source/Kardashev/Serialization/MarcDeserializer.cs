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

using System.Globalization;
using System.Text;
using MARC;

namespace Kardashev.Serialization;

/// <summary>
///     Exposes methods for deserializing a MARC-21 record in either byte
///     array buffer or string representation.
/// </summary>
internal static class MarcDeserializer
{
    //  The Byte Order Mark preamble that can appear at the start of a UTF-8
    //  encoded buffer.
    private static byte[] _utf8Preamble = Encoding.UTF8.GetPreamble();

    /// <summary>
    ///     Returns a new <see cref="Record"/> instance deserialized from the
    ///     given byte encoded array of MARC-21 data.
    /// </summary>
    /// <param name="buffer">
    ///     A byte encoded array of MARC-21 data to deserialize.
    /// </param>
    /// <param name="forceUtf8">
    ///     Whether UTF-8 encoding should be force regardless of whether the
    ///     Leader[9] value of the record specifies otherwise.
    /// </param>
    /// <returns>
    ///     A new <see cref="Record"/> deserialized from the MARC-21 data
    ///     provided.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Thrown if the Leader of the MARC-21 data appears to be invalid or
    ///     if a Tag value in a Directory Entry is invalid.
    /// </exception>
    public static Record Deserialize(byte[] buffer, bool forceUtf8 = false)
    {
        //  Will hold the string representation of the MARC record from
        //  the buffer given.
        string marc;

        //  Check the leader for the encoding flag
        char encodingFlag = Convert.ToChar(buffer[9]);

        //  Marc-8 or UTF-8
        if (encodingFlag == ' ' && !forceUtf8)
        {
            Encoding encoding = new MARC8();
            marc = encoding.GetString(buffer);
        }
        else
        {
            //  Check if the buffer start with the UTF-8 byte mark order
            //  preamble
            byte[] toCheck = buffer[0.._utf8Preamble.Length];
            if (Enumerable.SequenceEqual<byte>(toCheck, _utf8Preamble))
            {
                buffer = buffer[_utf8Preamble.Length..];
            }

            marc = Encoding.UTF8.GetString(buffer);
        }

        //  Deserialize the record from the string representation.
        Record record = Deserialize(marc);

        //  Return the record.
        return record;

    }

    /// <summary>
    ///     Returns a new <see cref="Record"/> instance deserialized from the
    ///     given MARC-21 string data.
    /// </summary>
    /// <param name="marc">
    ///     A string containing the MARC-21 data to deserialized.
    /// </param>
    /// <param name="forceUtf8">
    ///     Whether UTF-8 encoding should be force regardless of whether the
    ///     Leader[9] value of the record specifies otherwise.
    /// </param>
    /// <returns>
    ///     A new <see cref="Record"/> deserialized from the MARC-21 data
    ///     provided.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Thrown if the Leader of the MARC-21 data appears to be invalid or
    ///     if a Tag value in a Directory Entry is invalid.
    /// </exception>
    public static Record Deserialize(string marc)
    {
        Record record = new();

        //  Check that the leader is valid
        if (!IsLeaderValid(marc))
        {
            throw new ArgumentException("The MARC-21 data provided does not contain a valid leader", nameof(marc));
        }

        record.Leader = marc[0..LEADER_LENGTH];

        //  Remove the leader, don't need it
        marc = marc[LEADER_LENGTH..];

        //  Does the record end with a record terminator
        if (marc[^1] != RECORD_TERMINATOR)
        {
            record.Warnings.Add("Record does not end with a Record Terminator (hex 1D).");
        }
        else
        {
            //  Remove the terminator
            marc = marc[..^1];
        }

        //  Split the remaining marc on the Field Terminator character
        List<string> fields = new(marc.Split(FIELD_TERMINATOR, StringSplitOptions.RemoveEmptyEntries));

        //  The first index in the split is the directory. Pull it out and remove
        //  it from the collection
        string directory = fields[0];
        fields.RemoveAt(0);

        //  The directory is a series of 12-character entries, validate the
        //  directory is the correct size
        int extraDir = directory.Length % DIRECTORY_ENTRY_LENGTH;
        if (extraDir != 0)
        {
            record.Warnings.Add($"Directory contains {extraDir} extra chracter(s).  Removing extra characters");
            directory = directory[..^extraDir];
        }

        //  Divide by the directory entry lenght to get the total number of
        //  directory entries which shoudl also be the total number of
        //  fields to process from the split
        int entryCount = directory.Length / DIRECTORY_ENTRY_LENGTH;

        //  Split the directory into the 12-character entries
        List<string> entries = new(entryCount);
        for (int i = 0, s = 0; s < directory.Length; i++, s += DIRECTORY_ENTRY_LENGTH)
        {
            entries.Add(directory.Substring(s, DIRECTORY_ENTRY_LENGTH));
        }


        //  Make sure that's how many fields there are.
        if (entries.Count != fields.Count)
        {
            throw new InvalidOperationException("The directory does not contain the same number of entries as there are fields.  Record is invalid");
        }

        //  Process each field by going entry-by-entry
        for (int i = 0; i < entries.Count; i++)
        {
            string entry = entries[i];
            string data = fields[i];

            //  The first three characters of the entry are the tag, get it
            //  and validate it
            string sTag = entry[0..3];
            if (!int.TryParse(sTag, out int tag))
            {
                //  Tag values cannot be invalid.
                throw new ArgumentException($"Invalid Tag value of '{sTag}' found in Directory Entry #{i}");
            }

            //  The field should end with a field terminator, if so, remove it
            if (data[^1] == FIELD_TERMINATOR)
            {
                data = data[..^1];
            }

            //  Process based on the type of field
            if (tag < 10)
            {
                //  Control Fields only have the tag and the data
                record.AddField(new ControlField(sTag, data));
            }
            else
            {
                //  Varaible Data Fields have two indicators and then a series
                //  of zero or more subfields.  Each subfield, however, is
                //  proceeded with a subfield deliminator (hex 1F).  So if we
                //  split the data using the deliminator, the first index will
                //  be the indicators, and the remaining will the the subfields
                List<string> subfieldData = new(data.Split(SUBFIELD_DELIMINATOR));
                string indicators = subfieldData[0];
                subfieldData.RemoveAt(0);

                char indicator1;
                char indicator2;

                //  The first index should contain the indicators, and it should
                //  be exactly 2 characters in len
                if (indicators.Length != 2)
                {
                    record.Warnings.Add($"Invalid indicators '{indicators}' found for tag {sTag} from directory entry #{i}.  Forcing indicators to ' '");
                    indicator1 = ' ';
                    indicator2 = ' ';
                }
                else
                {
                    indicator1 = char.ToLower(indicators[0]);
                    indicator2 = char.ToLower(indicators[1]);

                    //  Indicators must be
                    //  -   basic latin (U+0000 - U+007F) (0 - 127)
                    //  -   A letter, a digit, or a ' ' blank space
                    if (indicator1 != ' ' && (indicator1 > 127 || !char.IsLetterOrDigit(indicator1)))
                    {
                        record.Warnings.Add($"Invalid first indicator '{indicator1}' found for tag {sTag} from directory entry #{i}. Forcing indicator1 to ' '");
                        indicator1 = ' ';
                    }

                    if (indicator2 != ' ' && (indicator2 > 127 || !char.IsLetterOrDigit(indicator2)))
                    {
                        record.Warnings.Add($"Invalid first indicator '{indicator2}' found for tag {sTag} from directory entry #{i}. Forcing indicator2 to ' '");
                        indicator2 = ' ';
                    }
                }

                //  Process the remaining data from the split as subfields
                List<Subfield> subfields = new();
                for (int j = 0; j < subfieldData.Count; j++)
                {
                    if (subfieldData[j].Length > 0)
                    {
                        subfields.Add(new(subfieldData[j][0], subfieldData[j][1..]));
                    }
                    else
                    {
                        record.Warnings.Add($"Subfied #{j} in tag {sTag} from directory entry #{i} has a length of zero");
                    }
                }

                if (subfields.Count == 0)
                {
                    record.Warnings.Add($"No subfields found for tag {sTag} from directory entry #{i}");
                }

                record.AddField(new DataField(sTag, indicator1, indicator2, subfields));
            }
        }

        return record;
    }

    /// <summary>
    ///     Returns a value that indicates whether the given string containing
    ///     the MARC-21 record data has a valid Leader.
    /// </summary>
    /// <param name="marc">
    ///     The MARC-21 record data as a string.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the data string has a valid Leader;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    private static bool IsLeaderValid(string marc)
    {
        //  There's actually no values in the leader that we can use to say
        //  "yep this is the leader, and yep it's valid". What we can do instead
        //  is use the values in the leader with other values within the entire
        //  marc string to validate that the leader values appear correct. And
        //  if they all appear to correct, then we can assume that the leader
        //  is valid.

        //  First, we know from the MARC specification that the leader is always
        //  a fixed 24-bytes in length. So our first assumption will be that
        //  the first 24-characters are the leader
        string leader = marc[0..LEADER_LENGTH];

        //  Next, the first 5 characters of the leader should be digits only
        //  and they shoudl represent the length of the entire record
        //  (including the leader)
        string sRecordLen = leader[0..5];
        if (!int.TryParse(sRecordLen, out int recordLen))
        {
            //  The leader does not contain a valid record len value, so this
            //  is not a valid leader
            return false;
        }

        //  Now that we have the record length, we can validate that against
        //  the length of the record
        if (recordLen != marc.Length)
        {
            //  Before we throw this out, the recordLen value may be wrong
            //  because of multi-byte character encoding.  So let's check
            //  that first before assuming the leader is invalid
            StringInfo stringInfo = new(marc);

            int exBytes = marc.Length - stringInfo.LengthInTextElements;
            int exBytesUtf8 = Encoding.UTF8.GetByteCount(marc) - marc.Length;

            if (recordLen != exBytes + marc.Length || recordLen != exBytesUtf8 + marc.Length)
            {
                //  The record length value does not match as a whole or because
                //  of multi-byte characters. So we assume the leader is invalid
                //  and return false
                return false;
            }
        }

        //  The next thing we can validate is the Base Address of Data value
        //  from the leader.  This tells us the starting character position of
        //  the first field that comes directly after the Field Terminator
        //  after the directory.
        //  This value is from Leader[12-16], and just like the record length,
        //  it must be 5 characters that are digits only
        string sBaseAddress = leader[12..17];
        if (!int.TryParse(sBaseAddress, out int baseAddress))
        {
            //  Base address is not valid, so we assume leader is invalid and
            //  return false
            return false;
        }

        //  Check that the base address value given is valid by look at the
        //  character just before that address and validating that it is
        //  the Field Terminator
        if (baseAddress >= marc.Length || marc[baseAddress - 1] != FIELD_TERMINATOR)
        {
            //  Either the base address value is wrong, or, there was no
            //  field terminator input after the directory.  So we can't
            //  validate that the leader is correct, so return false
            return false;
        }

        //  As one final check, the last four characters of the leader should
        //  be '4500'.
        if (leader[^4..] != "4500")
        {
            //  Not valid, return false
            return false;
        }

        //  All things above that we can use to assume that the leader is valid
        //  have passed, so we'll assume it is valid and return true.
        return true;
    }
}

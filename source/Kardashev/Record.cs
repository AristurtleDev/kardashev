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

using System.Collections;
using System.Text;

namespace Kardashev;

/// <summary>
///     Represents a single MARC-21 bibliographic record.
/// </summary>
/// <remarks>
///     Records consist of a Leader, Control Fields, and Variable Data Fields.
/// </remarks>
public sealed class Record : IEnumerable<Field>, IEnumerable, IEquatable<Record>
{
    //  Backing collection for all fields added to this record.
    private List<Field> _fields = new();

    //  A lookup dictionary used to lookup up a grouping of fields by their
    //  tag value quickly.
    private Dictionary<string, List<Field>> _tagLookup = new();

    /// <summary>
    ///     Gets the string representatin of the leader for this
    ///     <see cref="Record"/>.
    /// </summary>
    public string Leader { get; internal set; } = new string(' ', LEADER_LENGTH);

    /// <summary>
    ///     Gets the collection of warnings generated for this record if it was
    ///     created from parsing a local file.
    /// </summary>
    public List<string> Warnings { get; } = new();

    /// <summary>
    ///     Gets the total number of <see cref="Field"/> elements within this
    ///     <see cref="Record"/>.
    /// </summary>
    public int Count => _fields.Count;

    /// <summary>
    ///     Gets the <see cref="Field"/> element at the <paramref name="index"/>
    ///     specified from this <see cref="Record"/>.
    /// </summary>
    /// <param name="index">
    ///     The index of the <see cref="Field"/>.
    /// </param>
    /// <returns>
    ///     The <see cref="Field"/> element at the <paramref name="index"/>
    ///     specified.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the <paramref name="index"/> value given is out of bounds.
    /// </exception>
    public Field this[int index] => _fields[index];

    /// <summary>
    ///     Creates a new <see cref="Record"/> class instance.
    /// </summary>
    public Record() { }

    /// <summary>
    ///     Adds the given <paramref name="field"/> to this
    ///     <see cref="Record"/>.
    /// </summary>
    /// <param name="field">
    ///     The <see cref="Field"/> to add.
    /// </param>
    public void AddField(Field field)
    {
        //  Add it to the main field collection.
        _fields.Add(field);

        //  Add it to the lookup
        if (_tagLookup.ContainsKey(field.Tag))
        {
            _tagLookup[field.Tag].Add(field);
        }
        else
        {
            _tagLookup.Add(field.Tag, new() { field });
        }
    }

    /// <summary>
    ///     Removes the given <paramref name="field"/> from this
    ///     <see cref="Record"/>.
    /// </summary>
    /// <param name="field">
    ///     The <see cref="Field"/> to remove.
    /// </param>
    public void RemoveField(Field field)
    {
        //  Remove it from the main field collection
        _fields.Remove(field);

        //  Remove it from the lookup
        if (_tagLookup.ContainsKey(field.Tag))
        {
            _tagLookup[field.Tag].Remove(field);

            if (_tagLookup[field.Tag].Count == 0)
            {
                _tagLookup.Remove(field.Tag);
            }
        }
    }

    /// <summary>
    ///     Gets a new <see cref="List{T}"/> collection of <see cref="Field"/>
    ///     instances from this <see cref="Record"/>, filtered by the
    ///     <paramref name="tag"/> value given.
    /// </summary>
    /// <param name="tag">
    ///     <param>
    ///         The value that a <see cref="Field.Tag"/> value must match to be
    ///         added to the return result.
    ///     </param>
    ///     <param>
    ///         If this value is equal to <see langword="null"/> or
    ///         <see cref="string.Empty"/> then no matching will be applied and
    ///         all <see cref="Field"/> instances in this <see cref="Record"/>
    ///         will be included.
    ///     </param>
    /// </param>
    /// <returns>
    ///     A new <see cref="List{T}"/> collection of <see cref="Field"/>
    ///     instnaces from this <see cref="Record"/>, filtered by the
    ///     <paramref name="tag"/> value given.
    /// </returns>
    public List<Field> GetFields(string? tag = default)
    {
        List<Field> fields = new();

        if (string.IsNullOrEmpty(tag))
        {
            //  If the tag is null or empty string, we don't need to check field
            //  tags for match, so we just add all fields.
            fields.AddRange(_fields);
        }
        else if (_tagLookup.TryGetValue(tag, out List<Field>? byTag))
        {
            //  We add only the fields that had the matching tag
            fields.AddRange(byTag);
        }

        return fields;
    }

    /// <summary>
    ///     Returns an enumerator that iterates through all <see cref="Field"/>
    ///     elements in this <see cref="Record"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="List{T}.Enumerator"/> for this <see cref="Record"/>.
    /// </returns>
    public IEnumerator<Field> GetEnumerator() => _fields.GetEnumerator();

    /// <summary>
    ///     Returns an enumerator that iterates through all <see cref="Field"/>
    ///     elements in this <see cref="Record"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="List{T}.Enumerator"/> for this <see cref="Record"/>.
    /// </returns>
    IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    ///     Returns an enumerator that iterates through all <see cref="Field"/>
    ///     elements in this <see cref="Record"/> that have a
    ///     <see cref="Field.Tag"/> value that matches the
    ///     <paramref name="tag"/> specified.
    /// </summary>
    /// <param name="tag">
    ///     The tag value to match.
    /// </param>
    /// <returns>
    ///     A <see cref="List{T}.Enumerator"/> for this <see cref="Record"/>
    ///     of all <see cref="Field"/> elements with a <see cref="Field.Tag"/>
    ///     value matching the <paramref name="tag"/> specified.
    /// </returns>
    public IEnumerable<Field> EnumerateFieldsByTag(string tag)
    {
        List<Field> byTag = GetFields(tag);

        for (int i = 0; i < byTag.Count; i++)
        {
            yield return byTag[i];
        }
    }

    /// <summary>
    ///     Returns a new string containing the MARC-21 formatted representation
    ///     of this <see cref="Record"/>.
    /// </summary>
    /// <param name="encoding">
    ///     The encoding to use.
    /// </param>
    /// <returns>
    ///     A new string containin the MARC-21 formatted represetnation of this
    ///     <see cref="Record"/>.
    /// </returns>
    public string ToMarc(Encoding? encoding = default)
    {
        //  Use UTF-8 if no other provided.
        if (encoding == null)
        {
            encoding = Encoding.UTF8;
        }

        //  Build the directory
        string mFields = string.Empty;
        string directory = string.Empty;
        int dataEnd = 0;
        int count = 0;

        foreach (Field field in this)
        {
            //  Only non-empty fields
            if (!field.IsEmpty)
            {
                //  Get the MARC formatted string of the field
                string mField = field.ToMarc();

                //  Append it to the overall field string.,
                mFields += mField;

                //  Get the length of the field as marc in bytes.
                int mFieldLen = encoding.GetBytes(mField).Length;

                //  Create the directoyr entry of this field
                string directoryEntry = string.Empty;
                directoryEntry += field.Tag.PadLeft(3, '0');                //  Tag is 3 alphanumeric characters.
                directoryEntry += mFieldLen.ToString().PadLeft(4, '0');     //  Field length is 4 alphanumeric characters.
                directoryEntry += dataEnd.ToString().PadLeft(5, '0');       //  The field start position is 5 alphanumeric characters.

                //  Append the entry to the directory string
                directory += directoryEntry;

                //  Increment the data end value
                dataEnd += mFieldLen;

                //  Increment the field count
                count++;
            }
        }

        //  Calculate the position within the record that is the base
        //  address of where the record body starts.
        int bodyStartAddress = LEADER_LENGTH + (count * DIRECTORY_ENTRY_LENGTH) + 1;

        //  Calculate the length of the record in bytes
        int recordLen = bodyStartAddress + dataEnd + 1;

        //  But we'll need them as a string in a moment. It should always be
        //  five characters, padding to the left with 0's if needed
        string sRecordLen = recordLen.ToString().PadLeft(5, '0');
        string sBodyStart = bodyStartAddress.ToString().PadLeft(5, '0');

        //  Build the leader
        char codingScheme = encoding == Encoding.UTF8 ? '8' : ' ';
        Span<char> leader = stackalloc char[LEADER_LENGTH];
        leader.Clear();

        leader[0] = sRecordLen[0];  //  Record length (first character)
        leader[1] = sRecordLen[1];  //  Record Length (second character)
        leader[2] = sRecordLen[2];  //  Record Lenght (third character)
        leader[3] = sRecordLen[3];  //  Record Length (fourth character)
        leader[4] = sRecordLen[4];  //  Record Length (fifth character)
        leader[5] = Leader[5];      //  Record Status
        leader[6] = Leader[6];      //  Type of Record
        leader[7] = Leader[7];      //  Bibliographic level
        leader[8] = Leader[8];      //  Type of control
        leader[9] = codingScheme;   //  Character coding scheme
        leader[10] = '2';           //  Indicator count
        leader[11] = '2';           //  Subfield code count
        leader[12] = sBodyStart[0]; //  Body start address (first character)
        leader[13] = sBodyStart[1]; //  Body start address (second character)
        leader[14] = sBodyStart[2]; //  Body start address (third character)
        leader[15] = sBodyStart[3]; //  Body start address (fourth character)
        leader[16] = sBodyStart[4]; //  Body start address (fifth character)
        leader[17] = Leader[17];    //  Encoding Level
        leader[18] = Leader[18];    //  Descriptive cataloging form
        leader[19] = Leader[19];    //  Multipart resource record level
        leader[20] = '4';           //   Length of the length-of-field porition.
        leader[21] = '5';           //  Length of the starting-character-position porition.
        leader[22] = '0';           //  Lengthof the implementation-defined porition.
        leader[23] = '0';           //  Undefined

        //  Build the full record string
        string recordMarc = leader.ToString() + directory + FIELD_TERMINATOR + mFields + RECORD_TERMINATOR;

        return recordMarc;
    }

    /// <summary>
    ///     Returns a new string representation of this <see cref="Record"/>.
    /// </summary>
    /// <returns>
    ///     A new string representation of this <see cref="Record"/>.
    /// </returns>
    public override string ToString()
    {
        //  Attach the leader line first.
        string result = "LEADER " + GetInternalLeader() + Environment.NewLine;

        //  Go through each field and get that field as a string.
        List<Field> fields = GetFields();
        List<string> asStrings = new();

        for (int i = 0; i < fields.Count; i++)
        {
            //  Only add non-empty fields
            if (!fields[i].IsEmpty)
            {
                asStrings.Add(fields[i].ToString());
            }
        }

        //  Append each field as a string ot the result with a new line
        //  separating each field
        result += string.Join(Environment.NewLine, asStrings);

        return result;
    }

    /// <summary>
    ///     <para>
    ///         Gets a new string representation of the Leader for this
    ///         <see cref="Record"/> based on the actual elements that make up
    ///         this instance currently.
    ///     </para>
    ///     <para>
    ///         This will produce a Leader that is most likely different than
    ///         the Leader that was provided during initialization of this
    ///         <see cref="Record"/> instance.
    ///     </para>
    /// </summary>
    /// <returns>
    ///     A new string represetnation of the Leader for this
    ///     <see cref="Record"/>
    /// </returns>
    private string GetInternalLeader()
    {
        int dataEnd = 0;
        int count = 0;

        foreach (Field field in this)
        {
            //  Only process non-emtpy fields
            if (!field.IsEmpty)
            {
                //  Get the length of the field as a MARC formatted string.
                dataEnd += field.ToMarc().Length;

                //  Increment the count of fields
                count++;
            }

        }

        //  Calculate the position within the record that is the base
        //  address of where the record body starts.
        int bodyStartAddress = LEADER_LENGTH + (count * DIRECTORY_ENTRY_LENGTH) + 1;

        //  Calculate the length of the record in bytes
        int recordLen = bodyStartAddress + dataEnd + 1;

        //  But we'll need them as a string in a moment. It should always be
        //  five characters, padding to the left with 0's if needed
        string sRecordLen = recordLen.ToString().PadLeft(5, '0');
        string sBodyStart = bodyStartAddress.ToString().PadLeft(5, '0');

        //  Build the leader
        Span<char> leader = stackalloc char[LEADER_LENGTH];
        leader.Clear();

        leader[0] = sRecordLen[0];  //  Record length (first character)
        leader[1] = sRecordLen[1];  //  Record Length (second character)
        leader[2] = sRecordLen[2];  //  Record Lenght (third character)
        leader[3] = sRecordLen[3];  //  Record Length (fourth character)
        leader[4] = sRecordLen[4];  //  Record Length (fifth character)
        leader[5] = Leader[5];      //  Record Status
        leader[6] = Leader[6];      //  Type of Record
        leader[7] = Leader[7];      //  Bibliographic level
        leader[8] = Leader[8];      //  Type of control
        leader[9] = Leader[9];      //  Character coding scheme
        leader[10] = '2';           //  Indicator count
        leader[11] = '2';           //  Subfield code count
        leader[12] = sBodyStart[0]; //  Body start address (first character)
        leader[13] = sBodyStart[1]; //  Body start address (second character)
        leader[14] = sBodyStart[2]; //  Body start address (third character)
        leader[15] = sBodyStart[3]; //  Body start address (fourth character)
        leader[16] = sBodyStart[4]; //  Body start address (fifth character)
        leader[17] = Leader[17];    //  Encoding Level
        leader[18] = Leader[18];    //  Descriptive cataloging form
        leader[19] = Leader[19];    //  Multipart resource record level
        leader[20] = '4';           //   Length of the length-of-field porition.
        leader[21] = '5';           //  Length of the starting-character-position porition.
        leader[22] = '0';           //  Lengthof the implementation-defined porition.
        leader[23] = '0';           //  Undefined

        return leader.ToString();
    }

    public override bool Equals(object? obj)
    {
        Record? other = obj as Record;
        if (other == null) { return false; }
        return Equals(other);
    }

    public bool Equals(Record? other)
    {
        if (other == null) { return false; }
        if (ReferenceEquals(this, other)) { return true; }

        //  Compare the two records by their MARC-21 formtted strings
        string thisAsMarc = this.ToMarc();
        string otherAsMarc = other.ToMarc();

        return string.Equals(thisAsMarc, otherAsMarc);
    }

    public override int GetHashCode() => ToMarc().GetHashCode();

}

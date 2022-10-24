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
using System.Text.RegularExpressions;

namespace Kardashev;

/// <summary>
///     Represnets a Variable Data Field in a MARC-21 formatted record.
/// </summary>
/// <remarks>
///     Variable Data Fields contain Tag values &gt;= 10 and consist of two
///     single character indicators and zero or more subfields.
/// </remarks>
public sealed class DataField : Field, IEnumerable
{
    /// <summary>
    ///     Gets or Sets the first indicator character of this
    ///     <see cref="DataField"/>.
    /// </summary>
    public char Indicator1 { get; set; }

    /// <summary>
    ///     Gets or Sets the second indicator character of this
    ///     <see cref="DataField"/>.
    /// </summary>
    public char Indicator2 { get; set; }

    /// <summary>
    ///     Gets or Sets the <see cref="List{T}"/> of <see cref="Subfield"/>
    ///     elements that make up this <see cref="DataField"/>.
    /// </summary>
    public List<Subfield> Subfields { get; set; } = new();

    /// <summary>
    ///     Gets a value that indicates whether this <see cref="DataField"/> is
    ///     considered empty.
    /// </summary>
    /// <remarks>
    ///     A <see cref="DataField"/> is considered empty when it contains
    ///     zero <see cref="Subfield"/> elements.
    /// </remarks>
    public override bool IsEmpty => Subfields.Count == 0;

    /// <summary>
    ///     Gets the first <see cref="Subfield"/> element in this
    ///     <see cref="DataField"/> that has a <see cref="Subfield.Code"/> value
    ///     that matches the <paramref name="code"/> value given.
    /// </summary>
    /// <param name="code">
    ///     The subfield code value to match.
    /// </param>
    /// <returns>
    ///     The first <see cref="Subfield"/> element instance in this
    ///     <see cref="DataField"/> that has a <see cref="Subfield.Code"/> value
    ///     that matched the <paramref name="code"/> value specified, if a
    ///     match is found; otherwise, <see langword="null"/>.
    /// </returns>
    public Subfield? this[char code]
    {
        get
        {
            for (int i = 0; i < Subfields.Count; i++)
            {
                if (Subfields[i].Code == code)
                {
                    return Subfields[i];
                }
            }

            return null;
        }
    }

    /// <summary>
    ///     Returns the <see cref="Subfield"/> element from this
    ///     <see cref="DataField"/> at the <paramref name="index"/> specified.
    /// </summary>
    /// <param name="index">
    ///     The index of the <see cref="Subfield"/> element to retrieve.
    /// </param>
    /// <returns>
    ///     The <see cref="Subfield"/> element from this <see cref="DataField"/>
    ///     at the <paramref name="index"/> specifried.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the <paramref name="index"/> value specified is out of the
    ///     bound of the underlying collection.
    /// </exception>
    public Subfield this[int index]
    {
        get
        {
            if (index < 0 || index > Subfields.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return Subfields[index];
        }
    }

    /// <summary>
    ///     Creates a new <see cref="DataField"/> class instance initialize
    ///     with the values provided.
    /// </summary>
    /// <param name="tag">
    ///     The tag value of this <see cref="DataField"/>.
    /// </param>
    /// <param name="indicator1">
    ///     The first indicator value of this <see cref="DataField"/>.
    /// </param>
    /// <param name="indicator2">
    ///     The second indicator value of this <see cref="DataField"/>.
    /// </param>
    /// <exception cref="ArgumentException"></exception>
    public DataField(string tag, char indicator1, char indicator2)
        : base(tag)
    {
        //  Validate the indicators
        if (!ValidateIndicator(indicator1))
        {
            throw new ArgumentException($"The first indicator value of '{indicator1}' is invalid.", nameof(indicator1));
        }

        if (!ValidateIndicator(indicator2))
        {
            throw new ArgumentException($"The second indicator value of '{indicator2}' is invalid.", nameof(indicator2));
        }

        Indicator1 = indicator1;
        Indicator2 = indicator2;
    }

    /// <summary>
    ///     Creates a new <see cref="DataField"/> class instance initialized
    ///     with the values provided.
    /// </summary>
    /// <param name="tag">
    ///     The tag value of this <see cref="DataField"/>.
    /// </param>
    /// <param name="indicator1">
    ///     The first indicator value of this <see cref="DataField"/>.
    /// </param>
    /// <param name="indicator2">
    ///     The second indicator value of this <see cref="DataField"/>.
    /// </param>
    /// <param name="subfields"
    ///     A collection of <see cref="Subfield"/> elements to add to this
    ///     <see cref="DataField"/>.
    /// </param>
    public DataField(string tag, char indicator1, char indicator2, IEnumerable<Subfield> subfields)
        : this(tag, indicator1, indicator2) => Subfields.AddRange(subfields);

    /// <summary>
    ///     Validates that the char given represents a valid indicator vlaue.
    /// </summary>
    /// <remarks>
    ///     A valid indicator is a single character 0 - 9, lower case a - z, or
    ///     an empty character ' '.
    /// </remarks>
    /// <param name="indicator">
    ///     The indicator value to validate.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the <paramref name="indicator"/> given is
    ///     valid; otherwise, <see langword="false"/>.
    /// </returns>
    private bool ValidateIndicator(char indicator)
    {
        Match indicatorMatch = Regex.Match($"{indicator}", "^[0-9a-z]{1}$");
        return indicatorMatch.Captures.Count > 0 || indicator == EMPTY_INDICATOR;
    }


    /// <summary>
    ///     Returns an enumerator that iterates through all
    ///     <see cref="Subfield"/> elements in this <see cref="DataField"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="List{T}.Enumerator"/> for this <see cref="DataField"/>.
    /// </returns>
    public IEnumerator GetEnumerator() => Subfields.GetEnumerator();

    /// <summary>
    ///     Returns an enumerator that iterates through all
    ///     <see cref="Subfield"/> elements in this <see cref="DataField"/>
    ///     that have a <see cref="Subfield.Code"/> value that matches the
    ///     <paramref name="code"/> value given.
    /// </summary>
    /// <param name="code">
    ///     The code value to match.
    /// </param>
    /// <returns>
    ///     A <see cref="List{T}.Enumerator"/> for this <see cref="DataField"/>
    ///     of all <see cref="Subfield"/> elements with a
    ///     <see cref="Subfield.Code"/> vlaue matching the
    ///     <paramref name="code"/> specified.
    /// </returns>
    public IEnumerable<Subfield> EnumerateSubfieldsByCode(char code)
    {
        for (int i = 0; i < Subfields.Count; i++)
        {
            if (Subfields[i].Code == code || code == '*')
            {
                yield return Subfields[i];
            }
        }
    }

    /// <summary>
    ///     Returns a new string containing the MARC-21 formatted representation
    ///     of this <see cref="DataField"/>.
    /// </summary>
    /// <returns>
    ///     A new string containing the MARC-21 formatted representation of this
    ///     <see cref="DataField"/>.
    /// </returns>
    public override string ToMarc()
    {
        StringBuilder builder = new();

        builder.Append(Indicator1);
        builder.Append(Indicator2);

        for (int i = 0; i < Subfields.Count; i++)
        {
            //  No empty subfields
            if (!Subfields[i].IsEmpty)
            {
                builder.Append(Subfields[i].ToMarc());
            }
        }

        builder.Append(FIELD_TERMINATOR);

        return builder.ToString();
    }

    /// <summary>
    ///     Returns a new string representation of this <see cref="DataField"/>.
    /// </summary>
    /// <returns>
    ///     A new string representation of this <see cref="DataField"/>.
    /// </returns>
    public override string ToString()
    {
        string result = base.ToString() + " " + Indicator1 + " " + Indicator2 + " ";

        List<string> subfieldsAsString = new();
        for (int i = 0; i < Subfields.Count; i++)
        {
            subfieldsAsString.Add(Subfields[i].ToString());
        }

        result += string.Join(' ', subfieldsAsString);
        return result;
    }

    /// <summary>
    ///     Validates that the <paramref name="tag"/> value given represents a
    ///     valid Tag value for a Variable Data Field.
    /// </summary>
    /// <remarks>
    ///     A Variable Data Field tag must be 3 characters in length that are
    ///     numerical characters representing a value of 10 or greater.
    /// </remarks>
    /// <param name="tag">
    ///     The Tag value to validate.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the <paramref name="tag"/> given is valid;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    protected override bool ValidateTag(string tag) => tag.Length == 3 &&
                                                       int.TryParse(tag, out int value) &&
                                                       value >= 10;

}

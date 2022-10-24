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
using System.Collections.ObjectModel;
using Kardashev.Serialization;

namespace Kardashev.IO;

public sealed class MarcStreamReader : IEnumerable, IDisposable
{
    //  Wehther the resources held by this instance have been released.
    private bool _isDisposed;

    //  The options provided to use while reading the fiels.
    private readonly MarcStreamReaderOptions _options;

    //  The underlying stream used to read the byte content of the MARC file.
    private readonly Stream _stream;

    //  Collection of all exceptions that occur while deserializing. This is
    //  only popuplated if the option to skip on error is set ot true. Cosumers
    //  can get the exceptions that occured after the deserialization through
    //  the Exceptions property below.
    private List<Exception> _deserializationExceptions;

    /// <summary>
    ///     Contains all exceptions that occured while deserializing the
    ///     records, if hte option to skip on error was set to true.
    /// </summary>
    public ReadOnlyCollection<Exception> Exceptions;

    /// <summary>
    ///     Gets a value that indicates the progress of this reader in terms how
    ///     how much data has been read from the file.
    /// </summary>
    public double Progress => (double)_stream.Position / _stream.Length;

    /// <summary>
    ///     Creates a new <see cref="MarcStreamReader"/> class instance
    ///     initialized to read the contents of the MARC file at the given
    ///     <paramref name="path"/>.
    /// </summary>
    /// <param name="path">
    ///     The absolute file path to the MARC file to read.
    /// </param>
    public MarcStreamReader(string path) : this(path, new(false, true)) { }

    /// <summary>
    ///     Creates a new <see cref="MarcStreamReader"/> class instance
    ///     initialized to read the contents of the MARC file at the given
    ///     <paramref name="path"/>.
    /// </summary>
    /// <param name="path">
    /// <param name="forceUtf8">
    ///     Whether UTF-8 encoding should be enforced for all records read even
    ///     if the record's leader specifies otherwise.
    /// </param>
    /// <exception cref="StreamInitializationException">
    ///     Thrown if an exception occurs when initializing the underlying
    ///     file stream.
    /// </exception>
    public MarcStreamReader(string path, MarcStreamReaderOptions options)
    {
        _options = options;
        _deserializationExceptions = new();
        Exceptions = _deserializationExceptions.AsReadOnly();

        //  Attempt to open the file as a read-only stream.  All exceptions
        //  throw by the open read are caught, wrapped by a generic
        //  initialization exception and then rethrown from here.
        try
        {
            _stream = File.OpenRead(path);
        }
        catch (Exception ex)
        {
            throw new StreamInitializationException("An error occured when attempting to initialize stream to read MARC file. See inner exception for details", ex);
        }
    }

    //  Finalizer implementation to internally call Dispose passing false.
    ~MarcStreamReader() => Dispose(false);

    /// <summary>
    ///     Returns an enumerator that iterates through all <see cref="Record"/>
    ///     elements from this <see cref="MarcStreamReader"/>.
    /// </summary>
    /// <returns>
    ///     Each iteration returns the next <see cref="Record"/> element read
    ///     from this <see cref="MarcStreamReader"/> until the end of stream
    ///     is reached.
    /// </returns>
    /// <exception cref="EndOfStreamException">
    ///     Thrown if a new iteration attempt occurs after the end of stream
    ///     has been reached.
    /// </exception>
    public IEnumerator GetEnumerator()
    {
        //  Leader[00 - 04] define the size, in bytes, of a MARC record, meaning
        //  the max allowable set value is 99999.  Buffer len is based on this
        //   and is set to 10MiB
        int bufferLen = 1024 * 1024 * 10;

        //  Tracks the position within the stream where we begin to read a new
        //  record.
        long recordStart = 0;

        //  Calculated as the data chunks are read from the stream. Determines
        //  the total length, in bytes, of the record to read.
        int recoredLen = 0;

        //  Buffer to hold the data as it's read from the stream.
        //  Allocate once
        byte[] buffer = new byte[bufferLen];

        while (_stream.Position < _stream.Length)
        {
            //  Clear any data in the buffer out
            Array.Clear(buffer);

            //  Read from the stream into the buffer
            int bytesRead = _stream.Read(buffer);

            //  Check the data read for an instance of the record terminator
            int rtPos = Array.IndexOf(buffer, Convert.ToByte(RECORD_TERMINATOR));

            if (rtPos == -1)
            {
                //  No record terminator found, increment the record length by
                //  the number of bytes read and continue to the next chunk
                //  read
                recoredLen += bytesRead;
                continue;
            }
            else
            {
                //  Record terminator found, increment the record length by the
                //  number of bytes to get to the terminator only.  Plus 1 is
                //  added becuase zero-index arrays
                recoredLen += rtPos + 1;

                //  Create the buffer to hold the record
                byte[] recoredBuffer = new byte[recoredLen];

                //  Reset the stream position back to the start of the record
                _stream.Position = recordStart;

                //  Read the record into the record buffer
                if (_stream.Read(recoredBuffer) != recoredLen)
                {
                    //  End of stream reached while reading record.  This should
                    //  not be possible. The world is probably on fire. Go
                    //  outside and touch some grass.
                    throw new EndOfStreamException("End of stream was reached while attempting to read another record");
                }

                Record? record;

                try
                {
                    //  Deserialize the record
                    record = MarcDeserializer.Deserialize(recoredBuffer, _options.ForceUtf8);
                }
                catch (Exception ex)
                {
                    if (_options.SkipOnError)
                    {
                        _deserializationExceptions.Add(ex);
                        record = default;
                    }
                    else
                    {
                        Dispose();
                        throw;
                    }
                }

                yield return record;

                //  Setup values for next record read
                recordStart = _stream.Position;
                recoredLen = 0;
            }
        }
    }

    /// <summary>
    ///     Disposes of unmanaged resources held by this instance.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Disposes of unmanaged, and optionally managed, resources held by
    ///     this instance.
    /// </summary>
    /// <param name="disposeManaged">
    ///     Whether managed resources should also be disposed of.
    /// </param>
    private void Dispose(bool disposeManaged)
    {
        //  Return early if already disposed
        if (_isDisposed)
        {
            return;
        }

        //  Are we disposing managed?
        if (disposeManaged)
        {
            _stream.Dispose();
        }

        _isDisposed = true;
    }

}

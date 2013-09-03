/**************************************************************************\
    Copyright Microsoft Corporation. All Rights Reserved.
\**************************************************************************/

namespace Standard
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;

    // disambiguate with System.Runtime.InteropServices.STATSTG
    using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;

    // All these methods return void.  Does the standard marshaller convert them to HRESULTs?
    /// <summary>
    /// Wraps a managed stream instance into an interface pointer consumable by COM.
    /// </summary>
    internal sealed class ManagedIStream : IStream, IDisposable
    {
        private const int STGTY_STREAM = 2;
        private const int STGM_READWRITE = 2;
        private const int LOCK_EXCLUSIVE = 2;

        private Stream _source;

        /// <summary>
        /// Initializes a new instance of the ManagedIStream class with the specified managed Stream object.
        /// </summary>
        /// <param name="source">
        /// The stream that this IStream reference is wrapping.
        /// </param>
        public ManagedIStream(Stream source)
        {
            Verify.IsNotNull(source, "source");
            _source = source;
        }

        private void _Validate()
        {
            if (null == _source)
            {
                throw new ObjectDisposedException("this");
            }
        }

        // Comments are taken from MSDN IStream documentation.
        #region IStream Members

        /// <summary>
        /// Creates a new stream object with its own seek pointer that
        /// references the same bytes as the original stream. 
        /// </summary>
        /// <param name="ppstm">
        /// When this method returns, contains the new stream object. This parameter is passed uninitialized.
        /// </param>
        /// <remarks>
        /// For more information, see the existing documentation for IStream::Clone in the MSDN library.
        /// This class doesn't implement Clone.  A COMException is thrown if it is used.
        /// </remarks>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Standard.HRESULT.ThrowIfFailed(System.String)")]
        [Obsolete("The method is not implemented", true)]
        public void Clone(out IStream ppstm)
        {
            ppstm = null;
            HRESULT.STG_E_INVALIDFUNCTION.ThrowIfFailed("The method is not implemented.");
        }

        /// <summary>
        /// Ensures that any changes made to a stream object that is open in transacted
        /// mode are reflected in the parent storage. 
        /// </summary>
        /// <param name="grfCommitFlags">
        /// A value that controls how the changes for the stream object are committed. 
        /// </param>
        /// <remarks>
        /// For more information, see the existing documentation for IStream::Commit in the MSDN library.
        /// </remarks>
        public void Commit(int grfCommitFlags)
        {
            _Validate();
            _source.Flush();
        }

        /// <summary>
        /// Copies a specified number of bytes from the current seek pointer in the
        /// stream to the current seek pointer in another stream. 
        /// </summary>
        /// <param name="pstm">
        /// A reference to the destination stream. 
        /// </param>
        /// <param name="cb">
        /// The number of bytes to copy from the source stream. 
        /// </param>
        /// <param name="pcbRead">
        /// On successful return, contains the actual number of bytes read from the source.
        /// (Note the native signature is to a ULARGE_INTEGER*, so 64 bits are written
        /// to this parameter on success.)
        /// </param>
        /// <param name="pcbWritten">
        /// On successful return, contains the actual number of bytes written to the destination.
        /// (Note the native signature is to a ULARGE_INTEGER*, so 64 bits are written
        /// to this parameter on success.)
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
        {
            Verify.IsNotNull(pstm, "pstm");

            _Validate();

            // Reasonbly sized buffer, don't try to copy large streams in bulk.
            var buffer = new byte[4096];
            long cbWritten = 0;
            
            while (cbWritten < cb)
            {
                int cbRead = _source.Read(buffer, 0, buffer.Length);
                if (0 == cbRead)
                {
                    break;
                }

                // COM documentation is a bit vague here whether NULL is valid for the third parameter.
                // Going to assume it is, as most implementations I've seen treat it as optional.
                // It's possible this will break on some IStream implementations.
                pstm.Write(buffer, cbRead, IntPtr.Zero);
                cbWritten += cbRead;
            }

            if (IntPtr.Zero != pcbRead)
            {
                Marshal.WriteInt64(pcbRead, cbWritten);
            }

            if (IntPtr.Zero != pcbWritten)
            {
                Marshal.WriteInt64(pcbWritten, cbWritten);
            }
        }

        /// <summary>
        /// Restricts access to a specified range of bytes in the stream. 
        /// </summary>
        /// <param name="libOffset">
        /// The byte offset for the beginning of the range. 
        /// </param>
        /// <param name="cb">
        /// The length of the range, in bytes, to restrict.
        /// </param>
        /// <param name="dwLockType">
        /// The requested restrictions on accessing the range.
        /// </param>
        /// <remarks>
        /// For more information, see the existing documentation for IStream::LockRegion in the MSDN library.
        /// This class doesn't implement LockRegion.  A COMException is thrown if it is used.
        /// </remarks>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Standard.HRESULT.ThrowIfFailed(System.String)"), Obsolete("The method is not implemented", true)]
        public void LockRegion(long libOffset, long cb, int dwLockType)
        {
            HRESULT.STG_E_INVALIDFUNCTION.ThrowIfFailed("The method is not implemented.");
        }

        /// <summary>
        /// Reads a specified number of bytes from the stream object into memory starting at the current seek pointer. 
        /// </summary>
        /// <param name="pv">
        /// When this method returns, contains the data read from the stream. This parameter is passed uninitialized.
        /// </param>
        /// <param name="cb">
        /// The number of bytes to read from the stream object. 
        /// </param>
        /// <param name="pcbRead">
        /// A pointer to a ULONG variable that receives the actual number of bytes read from the stream object.
        /// </param>
        /// <remarks>
        /// For more information, see the existing documentation for ISequentialStream::Read in the MSDN library.
        /// </remarks>
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public void Read(byte[] pv, int cb, IntPtr pcbRead)
        {
            _Validate();

            int cbRead = _source.Read(pv, 0, cb);

            if (IntPtr.Zero != pcbRead)
            {
                Marshal.WriteInt32(pcbRead, cbRead);
            }
        }


        /// <summary>
        /// Discards all changes that have been made to a transacted stream since the last Commit call.
        /// </summary>
        /// <remarks>
        /// This class doesn't implement Revert.  A COMException is thrown if it is used.
        /// </remarks>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Standard.HRESULT.ThrowIfFailed(System.String)"), Obsolete("The method is not implemented", true)]
        public void Revert()
        {
            HRESULT.STG_E_INVALIDFUNCTION.ThrowIfFailed("The method is not implemented.");
        }

        /// <summary>
        /// Changes the seek pointer to a new location relative to the beginning of the
        /// stream, to the end of the stream, or to the current seek pointer.
        /// </summary>
        /// <param name="dlibMove">
        /// The displacement to add to dwOrigin.
        /// </param>
        /// <param name="dwOrigin">
        /// The origin of the seek. The origin can be the beginning of the file, the current seek pointer, or the end of the file. 
        /// </param>
        /// <param name="plibNewPosition">
        /// On successful return, contains the offset of the seek pointer from the beginning of the stream.
        /// (Note the native signature is to a ULARGE_INTEGER*, so 64 bits are written
        /// to this parameter on success.)
        /// </param>
        /// <remarks>
        /// For more information, see the existing documentation for IStream::Seek in the MSDN library.
        /// </remarks>
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
        {
            _Validate();

            long position = _source.Seek(dlibMove, (SeekOrigin)dwOrigin);

            if (IntPtr.Zero != plibNewPosition)
            {
                Marshal.WriteInt64(plibNewPosition, position);
            }
        }

        /// <summary>
        /// Changes the size of the stream object. 
        /// </summary>
        /// <param name="libNewSize">
        /// The new size of the stream as a number of bytes. 
        /// </param>
        /// <remarks>
        /// For more information, see the existing documentation for IStream::SetSize in the MSDN library.
        /// </remarks>
        public void SetSize(long libNewSize)
        {
            _Validate();
            _source.SetLength(libNewSize);
        }

        /// <summary>
        /// Retrieves the STATSTG structure for this stream. 
        /// </summary>
        /// <param name="pstatstg">
        /// When this method returns, contains a STATSTG structure that describes this stream object.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <param name="grfStatFlag">
        /// Members in the STATSTG structure that this method does not return, thus saving some memory allocation operations. 
        /// </param>
        public void Stat(out STATSTG pstatstg, int grfStatFlag)
        {
            pstatstg = default(STATSTG);
            _Validate();

            pstatstg.type = STGTY_STREAM;
            pstatstg.cbSize = _source.Length;
            pstatstg.grfMode = STGM_READWRITE;
            pstatstg.grfLocksSupported = LOCK_EXCLUSIVE;
        }

        /// <summary>
        /// Removes the access restriction on a range of bytes previously restricted with the LockRegion method.
        /// </summary>
        /// <param name="libOffset">The byte offset for the beginning of the range.
        /// </param>
        /// <param name="cb">
        /// The length, in bytes, of the range to restrict.
        /// </param>
        /// <param name="dwLockType">
        /// The access restrictions previously placed on the range.
        /// </param>
        /// <remarks>
        /// For more information, see the existing documentation for IStream::UnlockRegion in the MSDN library.
        /// This class doesn't implement UnlockRegion.  A COMException is thrown if it is used.
        /// </remarks>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Standard.HRESULT.ThrowIfFailed(System.String)")]
        [Obsolete("The method is not implemented", true)]
        public void UnlockRegion(long libOffset, long cb, int dwLockType)
        {
            HRESULT.STG_E_INVALIDFUNCTION.ThrowIfFailed("The method is not implemented.");
        }

        /// <summary>
        /// Writes a specified number of bytes into the stream object starting at the current seek pointer.
        /// </summary>
        /// <param name="pv">
        /// The buffer to write this stream to.
        /// </param>
        /// <param name="cb">
        /// The number of bytes to write to the stream. 
        /// </param>
        /// <param name="pcbWritten">
        /// On successful return, contains the actual number of bytes written to the stream object. 
        /// If the caller sets this pointer to null, this method does not provide the actual number
        /// of bytes written.
        /// </param>
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public void Write(byte[] pv, int cb, IntPtr pcbWritten)
        {
            _Validate();

            _source.Write(pv, 0, cb);

            if (IntPtr.Zero != pcbWritten)
            {
                Marshal.WriteInt32(pcbWritten, cb);
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Releases resources controlled by this object.
        /// </summary>
        /// <remarks>
        /// Dispose can be called multiple times, but trying to use the object
        /// after it has been disposed will generally throw ObjectDisposedExceptions.
        /// </remarks>
        public void Dispose()
        {
            _source = null;
        }

        #endregion
    }
}

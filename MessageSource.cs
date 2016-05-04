#region # using statements #

using System;
using StringLocalizerExtractor.Source;

#endregion

namespace StringLocalizerExtractor
{

    /// <summary>
    /// Represents where a translation message has been found.
    /// </summary>
    public sealed class MessageSource
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSource"/>
        /// class.
        /// </summary>
        /// <param name="file">The file where the translation message was
        /// found.</param>
        /// <param name="lineNumber">The number of the line where the
        /// translation message was found.</param>
        public MessageSource(SourceFile file, int lineNumber)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            File = file;
            LineNumber = lineNumber;
        }

        #region # Properties #

        #region == Public ==

        /// <summary>
        /// Gets the file where the translation was found.
        /// </summary>
        public SourceFile File { get; }

        /// <summary>
        /// Gets the number of the line where the translation message was
        /// found.
        /// </summary>
        public int LineNumber { get; }

        #endregion

        #endregion

        #region # Methods #

        #region == Overrides ==

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return File.Path + ":" + LineNumber;
        }

        #endregion

        #endregion
    }
}
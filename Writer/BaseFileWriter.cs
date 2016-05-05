#region # using statements #

using System;

#endregion

namespace StringLocalizerExtractor.Writer
{

    public abstract class BaseFileWriter : IWriter
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFileWriter"/>
        /// class.
        /// </summary>
        /// <param name="path"></param>
        protected BaseFileWriter(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            Path = System.IO.Path.GetFullPath(path);
        }

        #region # Properties #

        #region == Public ==

        /// <summary>
        /// Gets the absolute path to the file that will be generated.
        /// </summary>
        public string Path { get; }

        #endregion

        #endregion

        #region # IWriter #

        /// <summary>
        /// Writes all the translation messages.
        /// </summary>
        /// <param name="messages">The messages to be written.</param>
        public abstract void Write(ReadOnlyCollection<Message> messages);

        #endregion
    }
}
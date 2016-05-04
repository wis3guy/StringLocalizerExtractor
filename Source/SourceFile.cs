#region # using statements #

using System;
using System.IO;

#endregion

namespace StringLocalizerExtractor.Source
{

    /// <summary>
    /// Represents a file.
    /// </summary>
    public class SourceFile : BaseSource
    {
        #region # Variables #

        private string mContent;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceFile"/> class.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="directory"></param>
        public SourceFile(string path, SourceDirectory directory)
            : base(path)
        {
            if (!File.Exists(path) || !IsSupported(path))
            {
                // The path is not a valid file
                throw new ArgumentException(
                    $"The parameter {nameof(path)} must be a valid file.",
                    nameof(path));
            }
            if (directory == null)
                throw new ArgumentNullException(nameof(directory));

            Directory = directory;
        }

        #region # Properties #

        #region == Public ==

        /// <summary>
        /// Gets the <see cref="SourceDirectory"/> where this file is located.
        /// </summary>
        public virtual SourceDirectory Directory { get; }

        /// <summary>
        /// Gets the content of the file.
        /// </summary>
        public virtual string Content
            => mContent ?? (mContent = File.ReadAllText(Path));

        #endregion

        #endregion

        #region # Methods #

        #region == Internal ==

        internal static bool IsSupported(string file)
        {
            var ext = System.IO.Path.GetExtension(file);
            if (string.IsNullOrWhiteSpace(ext) || ext.Trim() == ".")
                return false;

            return ext.Equals(".cs", StringComparison.OrdinalIgnoreCase) ||
                   ext.Equals(".vb", StringComparison.OrdinalIgnoreCase) ||
                   ext.Equals(".cshtml", StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #endregion
    }
}
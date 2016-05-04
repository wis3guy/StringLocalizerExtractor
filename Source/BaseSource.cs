#region # using statements #

using System;

#endregion

namespace StringLocalizerExtractor.Source
{

    public abstract class BaseSource
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSource"/> class.
        /// </summary>
        /// <param name="path"></param>
        protected BaseSource(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            Path = System.IO.Path.GetFullPath(path);
        }

        #region # Properties #

        #region == Public ==

        /// <summary>
        /// Gets the absolute path to the source.
        /// </summary>
        public virtual string Path { get; }

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
            return Path;
        }

        #endregion

        #endregion
    }
}
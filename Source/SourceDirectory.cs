#region # using statements #

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

#endregion

namespace StringLocalizerExtractor.Source
{

    /// <summary>
    /// Represents a directory.
    /// </summary>
    public class SourceDirectory : BaseSource
    {
        #region # Variables #

        private ReadOnlyCollection<BaseSource> mChildren;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceDirectory"/>
        /// class as the root directory.
        /// </summary>
        /// <param name="path"></param>
        public SourceDirectory(string path)
            : this(path, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceDirectory"/>
        /// class.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        private SourceDirectory(string path, SourceDirectory parent)
            : base(path)
        {
            if (!Directory.Exists(Path))
            {
                // The path is not a valid directory
                throw new ArgumentException(
                    $"The parameter {nameof(path)} must be a valid directory",
                    nameof(path));
            }

            Parent = parent;
        }

        #region # Properties #

        #region == Public ==

        /// <summary>
        /// Gets the direct parent of this directory. The highest a parent can
        /// reach is the root directory.
        /// </summary>
        public virtual SourceDirectory Parent { get; }

        /// <summary>
        /// Gets the root directory.
        /// </summary>
        public virtual SourceDirectory Root
        {
            get
            {
                var root = this;
                while (root.Parent != null)
                    root = root.Parent;

                return root;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is the root directory.
        /// </summary>
        public virtual bool IsRoot => Parent == null;

        /// <summary>
        /// Gets a value indicating whether the directory is empty.
        /// </summary>
        public virtual bool IsEmpty => Children.Count == 0;

        /// <summary>
        /// Gets a <see cref="ReadOnlyCollection{T}"/> containing all the
        /// directory files and sub-directories.
        /// </summary>
        public virtual ReadOnlyCollection<BaseSource> Children
        {
            get
            {
                if (!IsAllowed(Path))
                    return new ReadOnlyCollection<BaseSource>(new BaseSource[0]);
                if (mChildren != null)
                    return mChildren;

                var items = new List<BaseSource>();

                // Retrieve all sub-directories
                items.AddRange(from d in Directory.GetDirectories(Path)
                               where IsAllowed(d)
                               select new SourceDirectory(d, this));

                // Retrieve all files
                items.AddRange(from f in Directory.GetFiles(Path)
                               where SourceFile.IsSupported(f)
                               select new SourceFile(f, this));

                // Done
                mChildren = new ReadOnlyCollection<BaseSource>(items);
                return mChildren;
            }
        }

        #endregion

        #endregion

        #region # Methods #

        #region == Internal ==

        internal static bool IsAllowed(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            return !EndsWith(path, "bin") && !EndsWith(path, "obj") &&
                   !EndsWith(path, "node_modules") && !EndsWith(path, ".git") &&
                   !EndsWith(path, ".vs");
        }

        #endregion

        #region == Private ==

        private static bool EndsWith(string path, string value)
        {
            var s1 = System.IO.Path.DirectorySeparatorChar;
            var s2 = System.IO.Path.AltDirectorySeparatorChar;

            return path.EndsWith(s1 + value, StringComparison.OrdinalIgnoreCase) ||
                   path.EndsWith(s2 + value, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #endregion
    }
}
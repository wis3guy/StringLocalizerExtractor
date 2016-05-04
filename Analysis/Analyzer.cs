#region # using statements #

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StringLocalizerExtractor.Logging;
using StringLocalizerExtractor.Source;

#endregion

namespace StringLocalizerExtractor.Analysis
{

    public sealed class Analyzer
    {
        #region # Variables #

        private readonly SourceDirectory mRootDirectory;
        private readonly List<Message> mMessages = new List<Message>();
        private int mAnalyzedFilesCount;
        private ILogger mLogger;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Analyzer"/> class.
        /// </summary>
        /// <param name="rootDirectory"></param>
        public Analyzer(SourceDirectory rootDirectory)
        {
            if (rootDirectory == null)
                throw new ArgumentNullException(nameof(rootDirectory));

            mRootDirectory = rootDirectory;
            mLogger = new NoLoggingLogger();
        }

        #region # Properties #

        #region == Public ==

        /// <summary>
        /// Gets or sets the <see cref="IWriter"/> used to save the translation
        /// messages.
        /// </summary>
        public IWriter Writer { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ILogger"/> to use.
        /// </summary>
        public ILogger Logger
        {
            get
            {
                return mLogger;
            }
            set
            {
                mLogger = value ?? new NoLoggingLogger();
            }
        }

        #endregion

        #endregion

        #region # Methods #

        #region == Public ==

        /// <summary>
        /// Analyzes all the files under the given root directory and it's
        /// sub-directories.
        /// </summary>
        /// <returns>
        /// The current instance of the code analyst.
        /// </returns>
        public Analyzer Analyze()
        {
            Logger.Information("Analysis is starting");
            mMessages.Clear();
            mAnalyzedFilesCount = 0;
            foreach (var child in mRootDirectory.Children)
                AnalyzeFileOrDirectory(child);

            // Done
            var sumOf = mMessages.Sum(m => m.Sources.Count);
            Logger.Information("Analysis completed");
            Logger.Information($"A total of {mMessages.Count} unique messages " +
                               $"out of {sumOf} were found in {mAnalyzedFilesCount} " +
                               "files.");

            return this;
        }

        /// <summary>
        /// Saves all the messages retrieved by the analyzer.
        /// </summary>
        /// <returns>
        /// The current instance of the code analyst.
        /// </returns>
        public Analyzer Save()
        {
            if (mMessages.Count == 0)
                return this; // Maybe not analyzed yet?

            Writer?.Write(new ReadOnlyCollection<Message>(mMessages));
            return this;
        }

        #endregion

        #region == Private ==

        [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
        private void AnalyzeFileOrDirectory(BaseSource source)
        {
            var dir = source as SourceDirectory;
            if (dir != null)
            {
                foreach (var child in dir.Children)
                    AnalyzeFileOrDirectory(child);

                return;
            }

            // Analyze file
            mAnalyzedFilesCount++;
            var fileAnalyzer = new FileAnalyzer(this, (SourceFile)source);
            foreach (var item in fileAnalyzer.Analyze())
            {
                var other = mMessages.FirstOrDefault(m => m.Text == item.Text);
                if (other == null)
                {
                    other = new Message(item.Text);
                    other.Sources.Add(item.Source);
                    mMessages.Add(other);
                }
                else
                {
                    var index = mMessages.IndexOf(other);
                    mMessages[index].Sources.Add(item.Source);
                }
            }
        }

        #endregion

        #endregion
    }
}
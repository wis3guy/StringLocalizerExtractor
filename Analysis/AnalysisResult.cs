#region # using statements #

using System;

#endregion

namespace StringLocalizerExtractor.Analysis
{

    public sealed class AnalysisResult
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisResult"/>
        /// class.
        /// </summary>
        /// <param name="text">The translation message content.</param>
        /// <param name="source">The translation message source.</param>
        internal AnalysisResult(string text, MessageSource source)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Text = text;
            Source = source;
        }

        #region # Properties #

        #region == Public ==

        /// <summary>
        /// Gets the text of the translation message content.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the source for the translation message.
        /// </summary>
        public MessageSource Source { get; }

        #endregion

        #endregion
    }
}
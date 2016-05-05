#region # using statements #

using System;
using System.Collections.Generic;

#endregion

namespace StringLocalizerExtractor
{

    /// <summary>
    /// Represents a translation message.
    /// </summary>
    public sealed class Message
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="text">The translation message content.</param>
        public Message(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            Text = text.Trim();
            Sources = new List<MessageSource>();
        }

        #region # Properties #

        #region == Public ==

        /// <summary>
        /// Gets the text of the translation message content.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets or sets the commant for this message.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets a collection containing all the sources where this message has
        /// been found.
        /// </summary>
        public List<MessageSource> Sources { get; }

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
            return Text;
        }

        #endregion

        #endregion
    }
}
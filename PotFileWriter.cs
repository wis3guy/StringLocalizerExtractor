#region # using statements #

using System;
using System.IO;
using System.Text;

#endregion

namespace StringLocalizerExtractor
{

    /// <summary>
    /// Represents a <see cref="IWriter"/> that generates a POT file.
    /// </summary>
    public sealed class PotFileWriter : IWriter
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PotFileWriter"/>
        /// class.
        /// </summary>
        /// <param name="path"></param>
        public PotFileWriter(string path)
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
        public void Write(ReadOnlyCollection<Message> messages)
        {
            var builder = new StringBuilder();

            // Write headers
            builder.AppendLine("#, fuzzy")
                   .AppendLine("msgid \"\"")
                   .AppendLine("msgstr \"\"")
                   .AppendLine("\"Project-Id-Version: \\n\"")
                   .AppendLine("\"POT-Creation-Date: \\n\"")
                   .AppendLine("\"PO-Revision-Date: \\n\"")
                   .AppendLine("\"Last-Translator: \\n\"")
                   .AppendLine("\"Language-Team: \\n\"")
                   .AppendLine("\"MIME-Version: 1.0\\n\"")
                   .AppendLine("\"Content-Type: text/plain; charset=iso-8859-1\\n\"")
                   .AppendLine(
                       "\"Plural - Forms: nplurals = 2; plural = (n != 1);\\n\"")
                   .AppendLine("\"Content-Transfer-Encoding: 8bit\\n\"")
                   .AppendLine("\"X-Generator: StringLocalizerExtractor 1.0.0\\n\"");

            // Write all messages with sources
            foreach (var message in messages)
            {
                builder.AppendLine();
                foreach (var source in message.Sources)
                    builder.AppendLine("#: " + source.File + ":" + source.LineNumber);

                // Write the message
                builder.Append("msgid \"").Append(message.Text).AppendLine("\"");
                builder.AppendLine("msgstr \"\"");
            }

            // Done
            File.WriteAllText(Path, builder.ToString());
        }

        #endregion
    }
}
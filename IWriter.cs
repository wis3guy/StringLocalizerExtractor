namespace StringLocalizerExtractor
{

    /// <summary>
    /// Defines the basic functionality of a writter.
    /// </summary>
    public interface IWriter
    {

        /// <summary>
        /// Writes all the translation messages.
        /// </summary>
        /// <param name="messages">The messages to be written.</param>
        void Write(ReadOnlyCollection<Message> messages);

    }
}
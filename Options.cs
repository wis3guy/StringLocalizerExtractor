using CommandLine;

namespace StringLocalizerExtractor
{

    internal class Options
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Options"/> class.
        /// </summary>
        public Options()
        {
        }
        
        [Value(0, MetaName = "src", HelpText = "The path to the source to analyze.",
            Required = true)]
        public string SourcePath { get; set; }

        [Option('o', "output",
            HelpText = "The name of the output file that will be generated.",
            Default = "./generated.pot")]
        public string OutputPath { get; set; }

    }
}
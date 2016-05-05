#region # using statements #

using CommandLine;
using StringLocalizerExtractor.Analysis;
using StringLocalizerExtractor.Logging;
using StringLocalizerExtractor.Source;
using StringLocalizerExtractor.Writer;

#endregion

namespace StringLocalizerExtractor
{

    internal static class Program
    {
        
        public static int Main(string[] args)
        {
            var options = Parser.Default.ParseArguments<Options>(args);
            var resultCode = options.MapResult(
                opt =>
                {
                    var analyst = new Analyzer(new SourceDirectory(opt.SourcePath))
                    {
                        Writer = new PotFileWriter(opt.OutputPath),
                        Logger = new ConsoleLogger(),
                        AppendDataAnnotationErrorMessages = true
                    };

                    analyst.Analyze().Save();
                    return 0;
                },
                error => 1);
            
            return resultCode;
        }
    }
}
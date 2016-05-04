#region # using statements #

using CommandLine;
using StringLocalizerExtractor.Analysis;
using StringLocalizerExtractor.Logging;
using StringLocalizerExtractor.Source;

#endregion

namespace StringLocalizerExtractor
{

    internal static class Program
    {

        private static int m = 33;

        public static int Main(string[] args)
        {
            var Localizer = new TestLocalizer();
            var r = Localizer[402f];
            var r2 = Localizer[401L];
            int n = 33;
            var r3 = Localizer[n];
            var r4 = Localizer[(object)"Localizer"];
            var r5 = Localizer[((object)"Localizer") as string];
            var r6 = Localizer["Localizer" as string];
            var r7 = Localizer[(object)"Localizer" as string];


            var options = Parser.Default.ParseArguments<Options>(args);
            var resultCode = options.MapResult(
                opt =>
                {
                    var analyst = new Analyzer(new SourceDirectory(opt.SourcePath))
                    {
                        Writer = new PotFileWriter(opt.OutputPath),
                        Logger = new ConsoleLogger()
                    };

                    analyst.Analyze().Save();
                    return 0;
                },
                error => 1);
            
            return resultCode;
        }

        private class TestLocalizer
        {

            public object this[object name] => name;

        }
    }
}
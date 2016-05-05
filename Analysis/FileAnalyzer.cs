#region # using statements #

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNet.Razor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StringLocalizerExtractor.Source;

#endregion

namespace StringLocalizerExtractor.Analysis
{

    public sealed class FileAnalyzer
    {
        #region # Constants #

        private const string LocalizerIdentifier = "Localizer";

        #endregion

        #region # Variables #

        private static RazorEngineHost mRazorEngineHost;
        private static RazorTemplateEngine mRazorTemplateEngine;

        private readonly Analyzer mAnalyzer;
        private readonly SourceFile mFile;
        private readonly FileType mType = FileType.Invalid;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAnalyzer"/>
        /// </summary>
        /// <param name="analyzer"></param>
        /// <param name="file"></param>
        internal FileAnalyzer(Analyzer analyzer, SourceFile file)
        {
            if (analyzer == null)
                throw new ArgumentNullException(nameof(analyzer));
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            mAnalyzer = analyzer;
            mFile = file;

            // Retrieve file type
            var ext = Path.GetExtension(file.Path) ?? "";
            if (ext.Equals(".cs", StringComparison.OrdinalIgnoreCase))
                mType = FileType.CSharp;
            else if (ext.Equals(".cshtml", StringComparison.OrdinalIgnoreCase))
                mType = FileType.Razor;
        }

        #region # Methods #

        #region == Public ==

        /// <summary>
        /// Analyzes the given file and retrieves all the translated messages
        /// as a <see cref="IReadOnlyCollection{T}"/>.
        /// </summary>=
        /// <returns>
        /// A <see cref="ReadOnlyCollection{T}"/> containing all the
        /// translation messages for the file.
        /// </returns>
        public ReadOnlyCollection<AnalysisResult> Analyze()
        {
            var path = Path.GetFileName(mFile.Path);
            mAnalyzer.Logger.Information("Analyzing file " + path);

            try
            {
                switch (mType)
                {
                    case FileType.CSharp:
                        return
                            new ReadOnlyCollection<AnalysisResult>(
                                AnalyzeTree(CSharpSyntaxTree.ParseText(mFile.Content)));
                    case FileType.Razor:
                        using (var reader = new StringReader(mFile.Content))
                        {
                            var generated =
                                GetRazorTemplateEngine().GenerateCode(reader);
                            return
                                new ReadOnlyCollection<AnalysisResult>(
                                    AnalyzeTree(
                                        CSharpSyntaxTree.ParseText(
                                            generated.GeneratedCode)));
                        }
                }
            }
            catch (Exception e)
            {
                mAnalyzer.Logger.Error("Failed to complete file analysis successfully", e);
            }

            // Failed to analyze file
            return ReadOnlyCollection<AnalysisResult>.Empty;

        }

        #endregion

        #region == Private ==

        private static bool IsDisplayAttribute(AttributeSyntax syntax)
        {
            var identifier = syntax.Name as IdentifierNameSyntax;
            if (identifier == null)
                return false;

            var text = identifier.Identifier.Text;
            return text.Equals("display", StringComparison.OrdinalIgnoreCase) ||
                   text.Equals("displayattribute", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsDisplayNameAttribute(AttributeSyntax syntax)
        {
            var identifier = syntax.Name as IdentifierNameSyntax;
            if (identifier == null)
                return false;

            var text = identifier.Identifier.Text;
            return text.Equals("displayname", StringComparison.OrdinalIgnoreCase) ||
                   text.Equals("displaynameattribute",
                       StringComparison.OrdinalIgnoreCase);
        }

        private static RazorTemplateEngine GetRazorTemplateEngine()
        {
            if (mRazorTemplateEngine != null)
                return mRazorTemplateEngine;

            // Initialize engine host
            mRazorEngineHost = new RazorEngineHost(new CSharpRazorCodeLanguage())
            {
                DefaultNamespace = "RazorOutput",
                DefaultClassName = "Template",
                DesignTimeMode = true
            };
            mRazorEngineHost.NamespaceImports.Add("System");

            // Done
            mRazorTemplateEngine = new RazorTemplateEngine(mRazorEngineHost);
            return mRazorTemplateEngine;
        }

        private MessageSource CreateSource(SyntaxNode node)
        {
            var position = node.GetLocation().GetLineSpan();

            // Calculate the line number
            var lineNumber = position.StartLinePosition.Line;
            switch (mType)
            {
                case FileType.CSharp:
                    lineNumber++;
                    break;
                case FileType.Razor:
                    break;
            }

            // Done
            return new MessageSource(mFile, lineNumber);
        }

        private static string GetNameFromExpression(ExpressionSyntax syntax)
        {
            if (syntax == null)
                return null; // Nothing to do with an empty expression, is there?

            // Retrieve alias name
            while (syntax.Kind() == SyntaxKind.AliasQualifiedName)
                syntax = ((AliasQualifiedNameSyntax)syntax).Name;

            // Retrieve qualified name
            while (syntax.Kind() == SyntaxKind.QualifiedName)
                syntax = ((QualifiedNameSyntax)syntax).Right;

            // Retrieve simple name
            var simpleName = syntax as SimpleNameSyntax;
            return simpleName?.Identifier.ValueText;
        }

        #region GetExpressionValue

        [SuppressMessage("ReSharper", "InvertIf")]
        [SuppressMessage("ReSharper", "TailRecursiveCall")]
        private static string GetExpressionValue(ExpressionSyntax syntax)
        {
            if (syntax == null)
                return null; // Nothing to do with a null expression, is there?

            // Remove the parenthesis expression
            while (syntax.Kind() == SyntaxKind.ParenthesizedExpression)
                syntax = ((ParenthesizedExpressionSyntax)syntax).Expression;

            // Remove the cast expression
            while (syntax.Kind() == SyntaxKind.CastExpression)
                syntax = ((CastExpressionSyntax)syntax).Expression;

            // Retrieve the value of a literal expression
            var literal = syntax as LiteralExpressionSyntax;
            if (literal != null)
            {
                return literal.Token.Kind() != SyntaxKind.StringLiteralToken
                    ? null
                    : literal.Token.ValueText;
            }

            // Retrieve the value of a interpolated string expression
            var interpolated = syntax as InterpolatedStringExpressionSyntax;
            if (interpolated != null)
            {
                var builder = new StringBuilder();
                foreach (var part in interpolated.Contents)
                {
                    if (part is InterpolatedStringTextSyntax)
                    {
                        var interString = part as InterpolatedStringTextSyntax;
                        builder.Append(interString.TextToken.ValueText);
                    }
                    else if (part is InterpolationSyntax)
                    {
                        var inter = part as InterpolationSyntax;
                        builder.Append(inter.OpenBraceToken.ValueText)
                               .Append(GetExpressionValue(inter.Expression));

                        // Append format (if any)
                        if (inter.FormatClause != null)
                        {
                            var frmt = inter.FormatClause;
                            builder.Append(frmt.ColonToken.ValueText)
                                   .Append(frmt.FormatStringToken.ValueText);
                        }

                        // Close brace
                        builder.Append(inter.CloseBraceToken.ValueText);
                    }
                }

                return builder.ToString();
            }

            // Retrieve the value of a binary expression
            var binary = syntax as BinaryExpressionSyntax;
            if (binary != null)
            {
                switch (binary.Kind())
                {
                    case SyntaxKind.AsExpression:
                        return GetExpressionValue(binary.Left);
                    case SyntaxKind.AddExpression:
                        var leftValue = GetExpressionValue(binary.Left);
                        var rightValue = GetExpressionValue(binary.Right);

                        if (binary.OperatorToken.ValueText == "+")
                            return leftValue + rightValue;
                        break;
                }
            }

            // Unable to retrieve expression value
            return null;
        }

        #endregion

        #region AnalyzeTree

        private IEnumerable<AnalysisResult> AnalyzeTree(SyntaxTree syntaxTree)
        {
            var treeRoot = (CompilationUnitSyntax)syntaxTree.GetRoot();
            var result = new List<AnalysisResult>();

            // Retrieve all element access
            var elements =
                from n in
                    treeRoot.DescendantNodes()
                            .OfType<ElementAccessExpressionSyntax>()
                let name = GetNameFromExpression(n.Expression)
                where
                    !string.IsNullOrWhiteSpace(name) &&
                    name.EndsWith(LocalizerIdentifier,
                        StringComparison.OrdinalIgnoreCase) &&
                    n.ArgumentList.Arguments.Any()
                select AnalyzeElementAccess(n);

            result.AddRange(elements.Where(e => e != null));

            // Retrieve all member access
            var members =
                from n in
                    treeRoot.DescendantNodes().OfType<MemberAccessExpressionSyntax>()
                let name1 = GetNameFromExpression(n.Expression)
                let name2 = GetNameFromExpression(n.Name)
                let invocation = n.Parent as InvocationExpressionSyntax
                where
                    invocation != null && invocation.ArgumentList.Arguments.Any() &&
                    !string.IsNullOrWhiteSpace(name1) &&
                    !string.IsNullOrWhiteSpace(name2) &&
                    name1.EndsWith(LocalizerIdentifier,
                        StringComparison.OrdinalIgnoreCase) &&
                    name2.Equals("GetString")
                select AnalyzeMemberAccess(n, invocation);

            result.AddRange(members.Where(m => m != null));

            // Analyze models (if the file is not a razor file)
            if (mType == FileType.Razor || mType == FileType.Invalid)
                return result;

            var classes =
                from n in treeRoot.DescendantNodes().OfType<ClassDeclarationSyntax>()
                let name = n.Identifier.ValueText
                where
                    !string.IsNullOrWhiteSpace(name) &&
                    (name.EndsWith("viewmodel", StringComparison.OrdinalIgnoreCase) ||
                     name.EndsWith("model", StringComparison.OrdinalIgnoreCase))
                select n;

            result.AddRange(classes.SelectMany(AnalyzeClassDeclaration));

            // Done
            return result;
        }

        #endregion

        #region AnalyzeElementAccess

        private AnalysisResult AnalyzeElementAccess(
            ElementAccessExpressionSyntax syntax)
        {
            var args = syntax.ArgumentList.Arguments;
            var value = GetExpressionValue(args.First().Expression);

            return string.IsNullOrWhiteSpace(value)
                ? null
                : new AnalysisResult(value, CreateSource(syntax));
        }

        #endregion

        #region AnalyzeMemberAccess

        private AnalysisResult AnalyzeMemberAccess(SyntaxNode syntax,
            InvocationExpressionSyntax invocation)
        {
            var args = invocation.ArgumentList.Arguments;
            var value = GetExpressionValue(args.First().Expression);

            return string.IsNullOrWhiteSpace(value)
                ? null
                : new AnalysisResult(value, CreateSource(syntax));
        }

        #endregion

        #region AnalyzeClassDeclaration

        private IEnumerable<AnalysisResult> AnalyzeClassDeclaration(
            ClassDeclarationSyntax syntax)
        {
            var result = new List<AnalysisResult>();

            // Retrieve all class properties
            var properties =
                syntax.DescendantNodes().OfType<PropertyDeclarationSyntax>();

            // Append all properties display/display name attributes
            foreach (var prop in properties)
            {
                var displays =
                    prop.AttributeLists.SelectMany(p => p.Attributes)
                        .Select(AnalyzeAttribute)
                        .Where(r => r != null)
                        .ToArray();

                if (displays.Length > 0)
                {
                    // Append any display/display attribute
                    result.AddRange(displays);
                    continue;
                }

                // Append direct property name
                result.Add(new AnalysisResult(prop.Identifier.ValueText,
                    CreateSource(prop)));
            }

            // Done
            return result;
        }

        #endregion

        #region AnalyzeAttribute

        private AnalysisResult AnalyzeAttribute(AttributeSyntax attribute)
        {
            if (attribute?.ArgumentList == null ||
                !attribute.ArgumentList.Arguments.Any())
                return null;

            var args = attribute.ArgumentList.Arguments;
            if (IsDisplayNameAttribute(attribute))
            {
                // Retrieve the DisplayName value
                var value = GetExpressionValue(args.FirstOrDefault()?.Expression);
                return string.IsNullOrWhiteSpace(value)
                    ? null
                    : new AnalysisResult(value, CreateSource(attribute));
            }
            if (IsDisplayAttribute(attribute))
            {
                // Retrieve the name argument
                var nameArg = (from a in args
                               let name = GetNameFromExpression(a.NameEquals?.Name)
                               where
                                   !string.IsNullOrWhiteSpace(name) &&
                                   name.Equals("name",
                                       StringComparison.OrdinalIgnoreCase)
                               select a).FirstOrDefault();

                if (nameArg == null)
                    return null;

                return new AnalysisResult(GetExpressionValue(nameArg.Expression),
                    CreateSource(nameArg));
            }

            // Retrieve error message
            var errMsgArg = (from a in args
                             let name = GetNameFromExpression(a.NameEquals?.Name)
                             where
                                 !string.IsNullOrWhiteSpace(name) &&
                                 name.Equals("errormessage",
                                     StringComparison.OrdinalIgnoreCase)
                             select a).FirstOrDefault();

            return errMsgArg == null
                ? null
                : new AnalysisResult(GetExpressionValue(errMsgArg.Expression),
                    CreateSource(errMsgArg));
        }

        #endregion

        #endregion

        #endregion

        private enum FileType
        {

            Invalid,
            CSharp,
            Razor
            
        }

    }
}
#region # using statements #

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StringLocalizerExtractor.Logging;
using StringLocalizerExtractor.Source;
using StringLocalizerExtractor.Writer;

#endregion

namespace StringLocalizerExtractor.Analysis
{

    public sealed class Analyzer
    {
        #region # Variables #

        private static readonly string[] DataAnnotationErrorMessages =
        {

            // ArgumentCannotBeNullOrEmpty
            "Value cannot be null or empty.",

            // NumericClientModelValidator_FieldMustBeNumber
            "The field {0} must be a number.",

            // ValidatableObjectAdapter_IncompatibleType
//            "The model object inside the metadata claimed to be compatible " +
//            "with '{0}', but was actually '{1}'.",

            // ArgumentIsNullOrWhitespace
            "The argument '{0}' cannot be null, empty or contain only white " +
            "space.",

            // AssociatedMetadataTypeTypeDescriptor_MetadataTypeContainsUnknownProperties
//            "The associated metadata type for type '{0}' contains the following" +
//            " unknown properties or fields: {1}. Please make sure that the " +
//            "names of these members match the names of the properties on the " +
//            "main type.",

            // AttributeStore_Type_Must_Be_Public
            "The type '{0}' must be public.",

            // AttributeStore_Unknown_Method
            "The type '{0}' does not contain a public method named '{1}'.",

            // AttributeStore_Unknown_Property
            "The type '{0}' does not contain a public property named '{1}'.",

            // Common_NullOrEmpty
            "Value cannot be null or empty.",

            // Common_PropertyNotFound
            "The property {0}.{1} could not be found.",

            // CompareAttribute_MustMatch
            "'{0}' and '{1}' do not match.",

            // CompareAttribute_UnknownProperty
            "Could not find a property named {0}.",

            // CreditCardAttribute_Invalid
            "The {0} field is not a valid credit card number.",

            // CustomValidationAttribute_Method_Must_Return_ValidationResult
            "The CustomValidationAttribute method '{0}' in type '{1}' must " +
            "return System.ComponentModel.DataAnnotations.ValidationResult. " +
            " Use System.ComponentModel.DataAnnotations.ValidationResult.Success " +
            "to represent success.",

            // CustomValidationAttribute_Method_Not_Found
            "The CustomValidationAttribute method '{0}' does not exist in type " +
            "'{1}' or is not public and static.",

            // CustomValidationAttribute_Method_Required
            "The CustomValidationAttribute.Method was not specified.",

            // CustomValidationAttribute_Method_Signature
            "The CustomValidationAttribute method '{0}' in type '{1}' must " +
            "match the expected signature: public static ValidationResult {0}" +
            "(object value, ValidationContext context).  The value can be " +
            "strongly typed.  The ValidationContext parameter is optional.",

            // CustomValidationAttribute_Type_Conversion_Failed
            "Could not convert the value of type '{0}' to '{1}' as expected by " +
            "method {2}.{3}.",

            // CustomValidationAttribute_Type_Must_Be_Public
            "The custom validation type '{0}' must be public.",

            // CustomValidationAttribute_ValidationError
            "{0} is not valid.",

            // CustomValidationAttribute_ValidatorType_Required
            "The CustomValidationAttribute.ValidatorType was not specified.",

            // DataTypeAttribute_EmptyDataTypeString
//            "The custom DataType string cannot be null or empty.",

            // DisplayAttribute_PropertyNotSet
//            "The {0} property has not been set.  Use the {1} method to get the " +
//            "value.",

            // EmailAddressAttribute_Invalid
            "The {0} field is not a valid e-mail address.",

            // EnumDataTypeAttribute_TypeCannotBeNull
//            "The type provided for EnumDataTypeAttribute cannot be null.",

            // EnumDataTypeAttribute_TypeNeedsToBeAnEnum
//            "The type '{0}' needs to represent an enumeration type.",

            // FileExtensionsAttribute_Invalid
            "The {0} field only accepts files with the following extensions: {1}",

            // LocalizableString_LocalizationFailed
//            "Cannot retrieve property '{0}' because localization failed.  Type " +
//            "'{1}' is not public or does not contain a public static string " +
//            "property with the name '{2}'.",

            // MaxLengthAttribute_InvalidMaxLength
//            "MaxLengthAttribute must have a Length value that is greater than " +
//            "zero. Use MaxLength() without parameters to indicate that the " +
//            "string or array can have the maximum allowable length.",

            // MaxLengthAttribute_ValidationError
            "The field {0} must be a string or array type with a maximum length " +
            "of '{1}'.",

            // MetadataTypeAttribute_TypeCannotBeNull
//            "MetadataClassType cannot be null.",

            // MinLengthAttribute_InvalidMinLength
//            "MinLengthAttribute must have a Length value that is zero or greater.",

            // MinLengthAttribute_ValidationError
            "The field {0} must be a string or array type with a minimum length " +
            "of '{1}'.",

            // PhoneAttribute_Invalid
            "The {0} field is not a valid phone number.",

            // RangeAttribute_ArbitraryTypeNotIComparable
//            "The type {0} must implement {1}.",

            // RangeAttribute_MinGreaterThanMax
            "The maximum value '{0}' must be greater than or equal to the " +
            "minimum value '{1}'.",

            // RangeAttribute_Must_Set_Min_And_Max
//            "The minimum and maximum values must be set.",

            // RangeAttribute_Must_Set_Operand_Type
//            "The OperandType must be set when strings are used for minimum and " +
//            "maximum values.",

            // RangeAttribute_ValidationError
            "The field {0} must be between {1} and {2}.",

            // RegexAttribute_ValidationError
            "The field {0} must match the regular expression '{1}'.",

            // RegularExpressionAttribute_Empty_Pattern
//            "The pattern must be set to a valid regular expression.",

            // RequiredAttribute_ValidationError
            "The {0} field is required.",

            // StringLengthAttribute_InvalidMaxLength
//            "The maximum length must be a nonnegative integer.",

            // StringLengthAttribute_ValidationError
            "The field {0} must be a string with a maximum length of {1}.",

            // StringLengthAttribute_ValidationErrorIncludingMinimum
            "The field {0} must be a string with a minimum length of {2} and " +
            "a maximum length of {1}.",

            // UIHintImplementation_ControlParameterKeyIsNotAString
//            "The key parameter at position {0} with value '{1}' is not a " +
//            "string. Every key control parameter must be a string.",

            // UIHintImplementation_ControlParameterKeyIsNull
//            "The key parameter at position {0} is null. Every key control " +
//            "parameter must be a string.",

            // UIHintImplementation_ControlParameterKeyOccursMoreThanOnce
//            "The key parameter at position {0} with value '{1}' occurs more " +
//            "than once.",

            // UIHintImplementation_NeedEvenNumberOfControlParameters
//            "The number of control parameters must be even.",

            // UrlAttribute_Invalid
            "The {0} field is not a valid fully-qualified http, https, or ftp " +
            "URL.",

            // ValidationAttribute_Cannot_Set_ErrorMessage_And_Resource
//            "Either ErrorMessageString or ErrorMessageResourceName must be set, " +
//            "but not both.",

            // ValidationAttribute_IsValid_NotImplemented
//            "IsValid(object value) has not been implemented by this class.  The " +
//            "preferred entry point is GetValidationResult() and classes should " +
//            "override IsValid(object value, ValidationContext context).",

            // ValidationAttribute_NeedBothResourceTypeAndResourceName
//            "Both ErrorMessageResourceType and ErrorMessageResourceName need to" +
//            " be set on this attribute.",

            // ValidationAttribute_ResourcePropertyNotStringType
//            "The property '{0}' on resource type '{1}' is not a string type.",

            // ValidationAttribute_ResourceTypeDoesNotHaveProperty
//            "The resource type '{0}' does not have an accessible static " +
//            "property named '{1}'.",

            // ValidationAttribute_ValidationError
            "The field {0} is invalid.",

            // ValidationContext_Must_Be_Method
//            "The ValidationContext for the type '{0}', member name '{1}' must " +
//            "provide the MethodInfo.",

            // ValidationContextServiceContainer_ItemAlreadyExists
//            "A service of type '{0}' already exists in the container.",

            // Validator_InstanceMustMatchValidationContextInstance
//            "The instance provided must match the ObjectInstance on the " +
//            "ValidationContext supplied.",

            // Validator_Property_Value_Wrong_Type
//            "The value for property '{0}' must be of type '{1}'."
        };

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

        /// <summary>
        /// Gets or sets whether the data annotations error messages should be
        /// appended to the localizable messages.
        /// </summary>
        public bool AppendDataAnnotationErrorMessages { get; set; }

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

            // Determine if should append data annotation error messages
            if (AppendDataAnnotationErrorMessages)
            {
                foreach (var msg in DataAnnotationErrorMessages)
                {
                    var exist =
                        (from n in mMessages where n.Text.Equals(msg) select n).Any();
                    if (exist)
                        continue;

                    mMessages.Add(new Message(msg)
                    {
                        Comment = "DataAnnotations error message."
                    });
                }
            }

            // Show result information
            var sumOf = mMessages.Sum(m => m.Sources.Count);
            Logger.Information("Analysis completed");
            Logger.Information($"A total of {mMessages.Count} unique messages " +
                               $"out of {sumOf} were found in {mAnalyzedFilesCount} " +
                               "files.");

            // Done
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
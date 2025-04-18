using System;

namespace Carpenter
{
    public class SchemaParsingException : Exception
    {
        public SchemaParsingException() { }
        public SchemaParsingException(string message) : base(message) { }
        public SchemaParsingException(string message, Exception inner) : base(message, inner) { }
    }

    public class SchemaValidationException : Exception
    {
        public SchemaValidator.ValidationResults ValidationResults { get; set; }

        public SchemaValidationException(SchemaValidator.ValidationResults results)
        {
            ValidationResults = results;
        }

        public SchemaValidationException(SchemaValidator.ValidationResults results, string message) : base(message)
        {
            ValidationResults = results;
        }
        
        public SchemaValidationException(string message, Exception inner) : base(message, inner) { }
    }
}
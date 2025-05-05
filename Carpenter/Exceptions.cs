using System;

namespace Carpenter
{
    public class PageParsingException : Exception
    {
        public PageParsingException() { }
        public PageParsingException(string message) : base(message) { }
        public PageParsingException(string message, Exception inner) : base(message, inner) { }
    }

    public class PageValidationException : Exception
    {
        public PageValidator.ValidationResults ValidationResults { get; set; }

        public PageValidationException(PageValidator.ValidationResults results)
        {
            ValidationResults = results;
        }

        public PageValidationException(PageValidator.ValidationResults results, string message) : base(message)
        {
            ValidationResults = results;
        }
        
        public PageValidationException(string message, Exception inner) : base(message, inner) { }
    }
}
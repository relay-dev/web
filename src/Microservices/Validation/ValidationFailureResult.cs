namespace Microservices.Validation
{
    public class ValidationFailureResult
    {
        public string Message { get; set; }

        public ValidationFailureDetail[] Errors { get; set; }
    }

    public class ValidationFailureDetail
    {
        public string Property { get; set; }

        public string ErrorMessage { get; set; }
    }
}

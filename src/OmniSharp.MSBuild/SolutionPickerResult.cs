namespace OmniSharp.MSBuild
{
    public class SolutionPickerResult
    {
        public SolutionPickerResult(string solution) : this(solution, message: null)
        {
        }

        public SolutionPickerResult(string solution, string message)
        {
            Solution = solution;
            Message = message;
        }

        public string Solution { get; }

        public string Message { get; }
    }
}
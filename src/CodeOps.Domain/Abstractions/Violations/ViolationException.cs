using System.Text;

namespace CodeOps.Domain.Abstractions.Violations
{
    public sealed class ViolationException : Exception
    {
        public ViolationSource SourceInfo { get; }

        public IReadOnlyCollection<Violation> Violations { get; }

        public ViolationException(ViolationSource sourceInfo, IReadOnlyCollection<Violation> violations)
            : base(BuildMessage(sourceInfo, violations))
        {
            ArgumentNullException.ThrowIfNull(sourceInfo);
            ArgumentNullException.ThrowIfNull(violations);

            SourceInfo = sourceInfo;
            Violations = violations;
        }

        private static string BuildMessage(ViolationSource sourceInfo, IReadOnlyCollection<Violation> violations)
        {
            ArgumentNullException.ThrowIfNull(sourceInfo);
            ArgumentNullException.ThrowIfNull(violations);

            if (violations.Count == 0)
                return $"{sourceInfo} failed with no violations.";

            var builder = new StringBuilder();
            builder.Append($"{sourceInfo} failed: ");
            builder.Append(string.Join("; ", violations.Select(static x => x.Message)));

            return builder.ToString();
        }
    }
}
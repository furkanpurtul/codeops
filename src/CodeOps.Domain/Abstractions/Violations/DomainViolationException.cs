using System.Text;

namespace CodeOps.Domain.Abstractions.Violations
{
    public sealed class DomainViolationException : Exception
    {
        public DomainViolationSource SourceInfo { get; }

        public IReadOnlyCollection<DomainViolation> Violations { get; }

        public DomainViolationException(DomainViolationSource sourceInfo, IReadOnlyCollection<DomainViolation> violations)
            : base(BuildMessage(sourceInfo, violations))
        {
            ArgumentNullException.ThrowIfNull(sourceInfo);
            ArgumentNullException.ThrowIfNull(violations);

            SourceInfo = sourceInfo;
            Violations = violations;
        }

        private static string BuildMessage(DomainViolationSource sourceInfo, IReadOnlyCollection<DomainViolation> violations)
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
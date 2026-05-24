using CodeOps.Domain.Abstractions.Violations;

namespace CodeOps.Application.Abstractions.Results
{
    public static class DomainViolationErrorMapper
    {
        public static Error Map(DomainViolationException exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            var violations = exception.Violations.ToList();

            if (violations.Count == 0)
            {
                var error = Errors.Unexpected
                (
                    code: "domain.mapping.no_violations",
                    message: "The domain violation exception did not contain any violations.",
                    metadata: new Dictionary<string, object?>
                    {
                        ["source"] = exception.SourceInfo.ToString()
                    }
                );

                return error;
            }

            var forbiddenViolations = violations
                .Where(x => x.Kind == ViolationKind.Forbidden)
                .ToList();

            if (forbiddenViolations.Count > 0)
            {
                var error = Errors.Forbidden
                (
                    code: "domain.forbidden",
                    message: exception.Message,
                    metadata: new Dictionary<string, object?>
                    {
                        ["source"] = exception.SourceInfo.ToString(),
                        ["violations"] = forbiddenViolations
                    }
                );

                return error;
            }

            var validationViolations = violations
                .Where(x => x.Kind == ViolationKind.Validation)
                .ToList();

            if (validationViolations.Count > 0)
            {
                var error = Errors.Validation
                (
                    code: "domain.validation",
                    message: exception.Message,
                    metadata: new Dictionary<string, object?>
                    {
                        ["source"] = exception.SourceInfo.ToString(),
                        ["violations"] = validationViolations
                    }
                );

                return error;
            }

            var conflictViolations = violations
                .Where(x => x.Kind == ViolationKind.Conflict)
                .ToList();

            if (conflictViolations.Count > 0)
            {
                var error = Errors.Conflict
                (
                    code: "domain.conflict",
                    message: exception.Message,
                    metadata: new Dictionary<string, object?>
                    {
                        ["source"] = exception.SourceInfo.ToString(),
                        ["violations"] = conflictViolations
                    }
                );

                return error;
            }

            var unknownKindError = Errors.Unexpected
            (
                code: "domain.mapping.unknown_kind",
                message: "The domain violation exception contained violations that could not be mapped.",
                metadata: new Dictionary<string, object?>
                {
                    ["source"] = exception.SourceInfo.ToString(),
                    ["violations"] = violations
                }
            );

            return unknownKindError;
        }
    }
}

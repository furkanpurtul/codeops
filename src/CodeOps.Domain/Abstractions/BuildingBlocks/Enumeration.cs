using System.Reflection;

namespace CodeOps.Domain.Abstractions.BuildingBlocks
{
    public abstract class Enumeration<TEnumeration> :
        IComparable<TEnumeration>,
        IEquatable<TEnumeration>
        where TEnumeration : Enumeration<TEnumeration>
    {
        private static readonly Lazy<IReadOnlyCollection<TEnumeration>> _declarations =
            new(LoadDeclarations);

        public int Value { get; }

        public string Name { get; }

        protected Enumeration(int value, string name)
        {
            ArgumentNullException.ThrowIfNull(name);

            Name = name;
            Value = value;
        }

        public bool Equals(TEnumeration? other)
            => ReferenceEquals(this, other) ||
               other is not null &&
               GetType() == other.GetType() &&
               Value == other.Value;

        public override bool Equals(object? obj)
            => obj is TEnumeration other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(GetType(), Value);

        public int CompareTo(TEnumeration? other)
            => other is null ? 1 : Value.CompareTo(other.Value);
        public override string ToString() => Name;

        public static bool operator ==(Enumeration<TEnumeration>? left, Enumeration<TEnumeration>? right)
            => ReferenceEquals(left, right) ||
               left is not null &&
               right is not null &&
               left.Equals((TEnumeration)right);

        public static bool operator !=(Enumeration<TEnumeration>? left, Enumeration<TEnumeration>? right)
            => !(left == right);

        public static TEnumeration FromValue(int value)
            => GetDeclarations().FirstOrDefault(x => x.Value == value)
               ?? throw new InvalidOperationException(
                   $"No {typeof(TEnumeration).Name} with Value={value} found.");

        public static TEnumeration FromName(string name)
        {
            ArgumentNullException.ThrowIfNull(name);

            return GetDeclarations().FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.Ordinal))
                   ?? throw new InvalidOperationException($"No {typeof(TEnumeration).Name} with Name='{name}' found.");
        }

        public static IReadOnlyCollection<TEnumeration> GetDeclarations() => _declarations.Value;

        private static IReadOnlyCollection<TEnumeration> LoadDeclarations()
        {
            var type = typeof(TEnumeration);

            var declarations = type
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(field => field.FieldType == type)
                .Select(field => field.GetValue(null))
                .OfType<TEnumeration>()
                .ToArray();

            EnsureNoDuplicateValues(type, declarations);
            EnsureNoDuplicateNames(type, declarations);

            return Array.AsReadOnly(declarations);
        }

        private static void EnsureNoDuplicateValues(Type type, IReadOnlyCollection<TEnumeration> declarations)
        {
            var duplicateValues = declarations
                .GroupBy(x => x.Value)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToArray();

            if (duplicateValues.Length > 0)
            {
                throw new InvalidOperationException(
                    $"{type.Name} contains duplicate Value declarations: {string.Join(", ", duplicateValues)}");
            }
        }

        private static void EnsureNoDuplicateNames(Type type, IReadOnlyCollection<TEnumeration> declarations)
        {
            var duplicateNames = declarations
                .GroupBy(x => x.Name, StringComparer.Ordinal)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToArray();

            if (duplicateNames.Length > 0)
            {
                throw new InvalidOperationException(
                    $"{type.Name} contains duplicate Name declarations: {string.Join(", ", duplicateNames)}");
            }
        }
    }
}
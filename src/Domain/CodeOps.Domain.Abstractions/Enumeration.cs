using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reflection;

namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Base type for smart enumerations that model named constants with a backing integer value,
    /// supporting equality, comparison, and reflective discovery of declared instances.
    /// </summary>
    /// <typeparam name="TEnumeration">
    /// The concrete enumeration type using the CRTP pattern. Declared instances should be <c>public static</c> fields or properties.
    /// </typeparam>
    public abstract class Enumeration<TEnumeration> : 
        IComparable, IComparable<TEnumeration>, IEquatable<TEnumeration>
        where TEnumeration : Enumeration<TEnumeration>
    {
        #region Properties
        /// <summary>
        /// Gets the integer value associated with the enumeration instance.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Gets the display name associated with the enumeration instance.
        /// </summary>
        public string Name { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new enumeration instance with the specified value and name.
        /// </summary>
        /// <param name="value">The integer value.</param>
        /// <param name="name">The display name.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
        protected Enumeration(int value, string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value;
        }
        #endregion

        #region Equality Members
        /// <summary>
        /// Determines whether the specified enumeration is equal to the current instance.
        /// </summary>
        /// <param name="other">The other enumeration instance.</param>
        /// <returns><c>true</c> if equal; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Equality requires same concrete type and equal <see cref="Value"/>.
        /// </remarks>
        public bool Equals(TEnumeration? other)
            => other is not null &&
               GetType() == other.GetType() &&
               Value == other.Value;

        /// <summary>
        /// Determines whether the specified object is equal to the current instance.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><c>true</c> if equal; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
            => obj is TEnumeration other && Equals(other);

        /// <summary>
        /// Returns a hash code for the enumeration instance.
        /// </summary>
        public override int GetHashCode() =>
            HashCode.Combine(GetType(), Value);

        /// <summary>
        /// Determines whether two enumeration instances are equal.
        /// </summary>
        public static bool operator ==(Enumeration<TEnumeration>? a, Enumeration<TEnumeration>? b)
            => ReferenceEquals(a, b) || a is not null && b is not null && a.Equals(b);

        /// <summary>
        /// Determines whether two enumeration instances are not equal.
        /// </summary>
        public static bool operator !=(Enumeration<TEnumeration>? a, Enumeration<TEnumeration>? b)
            => !(a == b);
        #endregion

        #region Comparison Members
        /// <summary>
        /// Compares this instance to another of the same type by <see cref="Value"/>.
        /// </summary>
        /// <param name="other">The other enumeration instance.</param>
        /// <returns>
        /// A value less than zero if this instance precedes <paramref name="other"/>,
        /// zero if equal, greater than zero if it follows.
        /// </returns>
        public int CompareTo(TEnumeration? other)
            => other is null ? 1 : Value.CompareTo(other.Value);

        /// <summary>
        /// Compares this instance to another object, ensuring type compatibility.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>Comparison result as per <see cref="CompareTo(TEnumeration)"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="obj"/> is not of the expected type.</exception>
        public int CompareTo(object? obj)
        {
            if (obj is null) return 1;
            if (obj is TEnumeration t) return CompareTo(t);
            throw new ArgumentException("Object is not a valid enumeration type.", nameof(obj));
        }
        #endregion

        #region Static Cached Values
        private static readonly ConcurrentDictionary<Type, IReadOnlyList<TEnumeration>> _cachedValues = new();

        /// <summary>
        /// Gets all declared instances of <typeparamref name="TEnumeration"/> discovered via reflection.
        /// </summary>
        /// <remarks>
        /// Caches the discovered instances per concrete enumeration type for performance.
        /// Declared instances must be public static fields or properties of the concrete type.
        /// </remarks>
        private static IReadOnlyCollection<TEnumeration> Values
        {
            get
            {
                var type = typeof(TEnumeration);
                return _cachedValues.GetOrAdd(type, t => LoadDeclaredInstances(t));
            }
        }
        #endregion

        #region Static Factory / Lookup Methods
        /// <summary>
        /// Gets the enumeration instance for the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The integer value.</param>
        /// <returns>The matching enumeration instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no matching declaration is found.
        /// </exception>
        public static TEnumeration FromValue(int value)
            => TryFromValue(value, out var found)
                ? found!
                : throw new InvalidOperationException(
                    $"No {typeof(TEnumeration).Name} with Value={value} found.");

        /// <summary>
        /// Attempts to get the enumeration instance for the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The integer value.</param>
        /// <param name="result">The resulting enumeration instance if found; otherwise <c>null</c>.</param>
        /// <returns><c>true</c> if found; otherwise, <c>false</c>.</returns>
        public static bool TryFromValue(int value, out TEnumeration? result)
        {
            result = Values.FirstOrDefault(v => v.Value == value);
            return result is not null;
        }

        /// <summary>
        /// Gets the enumeration instance for the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The display name to match.</param>
        /// <param name="ignoreCase">Whether to ignore case during comparison.</param>
        /// <returns>The matching enumeration instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no matching declaration is found.
        /// </exception>
        public static TEnumeration FromName(string name, bool ignoreCase = false)
            => TryFromName(name, ignoreCase, out var found)
                ? found!
                : throw new InvalidOperationException(
                    $"No {typeof(TEnumeration).Name} with Name='{name}' found.");

        /// <summary>
        /// Attempts to get the enumeration instance for the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The display name to match.</param>
        /// <param name="ignoreCase">Whether to ignore case during comparison.</param>
        /// <param name="result">The resulting enumeration instance if found; otherwise <c>null</c>.</param>
        /// <returns><c>true</c> if found; otherwise, <c>false</c>.</returns>
        public static bool TryFromName(string name, bool ignoreCase, out TEnumeration? result)
        {
            if (name == null) { result = null; return false; }

            var comparison = ignoreCase
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;

            result = Values.FirstOrDefault(v => string.Equals(v.Name, name, comparison));
            return result is not null;
        }

        /// <summary>
        /// Determines whether an enumeration with the specified <paramref name="value"/> is defined.
        /// </summary>
        public static bool IsDefined(int value)
            => Values.Any(v => v.Value == value);

        /// <summary>
        /// Determines whether an enumeration with the specified <paramref name="name"/> is defined.
        /// </summary>
        /// <param name="name">The display name to check.</param>
        /// <param name="ignoreCase">Whether to ignore case during comparison.</param>
        public static bool IsDefined(string name, bool ignoreCase = false)
            => TryFromName(name, ignoreCase, out _);

        /// <summary>
        /// Gets all declared enumeration instances for the concrete type.
        /// </summary>
        public static IEnumerable<TEnumeration> GetDeclarations() => Values;

        /// <summary>
        /// Gets a factory delegate that creates an enumeration instance from an integer value.
        /// </summary>
        public static Func<int, TEnumeration> FactoryFromValue => v => FromValue(v);
        #endregion

        #region Private Helpers
        /// <summary>
        /// Loads declared static instances (fields/properties) of the concrete enumeration type.
        /// </summary>
        /// <param name="type">The concrete enumeration type.</param>
        /// <returns>A read-only collection of discovered instances.</returns>
        private static ReadOnlyCollection<TEnumeration> LoadDeclaredInstances(Type type)
        {
            var instances = new List<TEnumeration>();

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                             .Where(f => f.FieldType == type);

            foreach (var f in fields)
            {
                if (f.GetValue(null) is TEnumeration val)
                    instances.Add(val);
            }

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                            .Where(p => p.PropertyType == type && p.GetMethod != null);

            foreach (var p in props)
            {
                if (p.GetValue(null) is TEnumeration val)
                    instances.Add(val);
            }

            return instances.AsReadOnly();
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Returns the display name.
        /// </summary>
        public override string ToString() => Name;
        #endregion
    }
}

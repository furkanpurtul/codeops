namespace CodeOps.Application.Abstractions.Validation
{
    public interface IValidator<in T>
        where T : class
    {
        void Validate(T value);
    }
}

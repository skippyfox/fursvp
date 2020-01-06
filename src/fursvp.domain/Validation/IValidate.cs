namespace fursvp.domain.Validation
{
    /// <summary>
    /// For use with Domain validation, not endpoint request validation
    /// </summary>
    public interface IValidate<T>
    {
        void ValidateState(T oldState, T newState);
    }
}

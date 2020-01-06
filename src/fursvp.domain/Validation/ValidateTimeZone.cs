using System;

namespace fursvp.domain.Validation
{
    public class ValidateTimeZone
    {
        public void Validate(string id)
        {
            try
            {
                TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ValidationException<string>(ex.Message, ex);
            }
        }
    }
}

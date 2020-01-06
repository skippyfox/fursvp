using System;
using System.Net.Mail;

namespace fursvp.domain.Validation
{
    public class ValidateEmail : IValidateEmail
    {
        public void Validate(string address)
        { 
            try
            {
                new MailAddress(address);
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

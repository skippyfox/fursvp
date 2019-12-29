using System;
using System.Collections.Generic;
using System.Text;

namespace fursvp.domain
{
    public class UtcDateTimeProvider : IProvideDateTime
    {
        public DateTime Now => DateTime.UtcNow;

        public DateTime Today => DateTime.UtcNow.Date;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace fursvp.domain
{
    public interface IProvideDateTime
    {
        DateTime Now { get; }
        DateTime Today { get; }
    }
}

using AutoMapper;
using Fursvp.Domain;

namespace Fursvp.Api.Responses
{
    /// <summary>
    /// Response object Mapping Profile for use with AutoMapper.
    /// </summary>
    public class ResponseMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseMappingProfile"/> class.
        /// </summary>
        public ResponseMappingProfile()
        {
            CreateMap<Event, EventResponse>();
            CreateMap<Member, MemberResponse>();
        }
    }
}

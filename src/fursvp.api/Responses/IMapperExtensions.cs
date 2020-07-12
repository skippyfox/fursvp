using AutoMapper;
using Fursvp.Api.Responses;
using Fursvp.Domain;
using Fursvp.Helpers;
using System;

namespace Fursvp.Api.Responses
{
    public static class IMapperExtensions
    {
        public static EventResponse MapResponse(this IMapper mapper, Event source)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            var result = mapper.Map<Event, EventResponse>(source);

            if (result != null)
            {
                try
                {
                    var targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(result.TimeZoneId);

                    result.StartsAtLocal = result.StartsAtUtc.ToLocal(targetTimeZone);
                    result.EndsAtLocal = result.EndsAtUtc.ToLocal(targetTimeZone);
                    
                    result.TimeZoneOffset = targetTimeZone.IsDaylightSavingTime(result.StartsAtLocal) ? targetTimeZone.DaylightName : targetTimeZone.StandardName;

                    if (result.RsvpClosesAtUtc.HasValue)
                    {
                        result.RsvpClosesAtLocal = result.RsvpClosesAtUtc.Value.ToLocal(targetTimeZone);
                    }

                    if (result.Members != null)
                    {
                        foreach (var member in result.Members)
                        {
                            member.RsvpedAtLocal = member.RsvpedAtUtc.ToLocal(targetTimeZone);
                        }
                    }
                }
                catch
                {
                    result.StartsAtLocal = result.StartsAtUtc;
                    result.EndsAtLocal = result.EndsAtUtc;
                    result.RsvpClosesAtLocal = result.RsvpClosesAtUtc;

                    if (result.Members != null)
                    {
                        foreach (var member in result.Members)
                        {
                            member.RsvpedAtLocal = member.RsvpedAtUtc;
                        }
                    }
                }
            }

            return result;
        }

        public static MemberResponse MapResponse(this IMapper mapper, Member source, string timeZoneId)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            var result = mapper.Map<Member, MemberResponse>(source);

            if (result != null)
            {
                result.RsvpedAtUtc.TryToLocal(timeZoneId, out var rsvpedAtLocal);
                result.RsvpedAtLocal = rsvpedAtLocal;
            }

            return result;
        }
    }
}

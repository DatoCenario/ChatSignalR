using MediatR;
using WebApplication2.Handlers.Interfaces;

namespace WebApplication2.Handlers.Users;

public class GetUsersRequest: IPaginationRequest, IRequest<GetUsersResponse>
{
    public long[] AdditionalIds { get; set; }
    public bool? ExcludeMe { get; set; }
    public bool? ExcludeHasChatsWithMe { get; set; }
    public string? FullNameFilter { get; set; }
    public string? PhoneOrMailFilter { get; set; }
    public long[]? IdFilter { get; set; }
    public long[]? ChatIdFilter { get; set; }
    public int Offset { get; set; }
    public int Count { get; set; }
}
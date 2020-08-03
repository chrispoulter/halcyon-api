using System.Collections.Generic;

namespace Halcyon.Web.Models.User
{
    public class ListUsersResponse
    {
        public ListUsersResponse()
        {
            Items = new List<GetUserResponse>();
        }

        public List<GetUserResponse> Items { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPreviousPage { get; set; }
    }
}
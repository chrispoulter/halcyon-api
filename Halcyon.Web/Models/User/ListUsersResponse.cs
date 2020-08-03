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

        public int Page { get; set; }

        public int Size { get; set; }

        public int Total { get; set; }
    }
}
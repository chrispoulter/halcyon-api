using System.Collections.Generic;

namespace Halcyon.Web.Models.User
{
    public class ListUsersResult
    {
        public ListUsersResult()
        {
            Items = new List<UserResult>();
        }

        public List<UserResult> Items { get; set; }

        public int Page { get; set; }

        public int Size { get; set; }

        public int Total { get; set; }
    }
}
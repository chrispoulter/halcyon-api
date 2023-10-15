using System.ComponentModel.DataAnnotations;

namespace Halcyon.Api.Features.Users.SearchUsers
{
    public class SearchUsersRequest
    {
        public string Search { get; set; }

        public UserSort Sort { get; set; } = UserSort.NAME_ASC;

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 50)]
        public int Size { get; set; } = 50;
    }
}

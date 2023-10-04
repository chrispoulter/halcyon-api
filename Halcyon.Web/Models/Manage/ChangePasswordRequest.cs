namespace Halcyon.Web.Models.Manage
{
    public class ChangePasswordRequest : UpdateRequest
    {
        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
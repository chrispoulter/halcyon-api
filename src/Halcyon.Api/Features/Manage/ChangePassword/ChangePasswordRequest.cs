namespace Halcyon.Api.Features.Manage.ChangePassword
{
    public class ChangePasswordRequest : UpdateRequest
    {
        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace URLShortener.ModelHelpers
{
    public class UserUpdate
    {
        [EmailAddress]
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? oldPassword { get; set; }
        public string? Password { get; set; }
    }
}

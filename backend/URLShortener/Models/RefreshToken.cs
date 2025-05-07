using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace URLShortener.Models
{
    public class RefreshToken
    {
        [Key]
        public Guid Guid { get; set; } = Guid.NewGuid();

        public DateOnly ExpiryDate { get; set; }

        public DateOnly CreationDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

    }
}

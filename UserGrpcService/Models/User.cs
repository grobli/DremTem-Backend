using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using UserGrpcService.Utilities.Security;

namespace UserGrpcService.Models
{
    // [Table(nameof(User))]
    // [Index(nameof(Email), IsUnique = true)]
    // public record User
    // {
    //     [Key]
    //     [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //     public int Id { get; set; }
    //
    //     [Required] public string FirstName { get; set; }
    //     public string LastName { get; set; }
    //     [Required] [EmailAddress] public string Email { get; private set; }
    //
    //     [Required]
    //     [MaxLength(Sha512HashLength), MinLength(Sha512HashLength)]
    //     public string Password { get; private set; }
    //
    //     [ Required]
    //     [MaxLength(SaltLength), MinLength(SaltLength)]
    //     public string PasswordSalt { get; private set; }
    //
    //     private const int SaltLength = 32;
    //     private const int Sha512HashLength = 128;
    //
    //     private User()
    //     {
    //     }
    //
    //     [return: NotNull]
    //     public static User CreateUser(string firstName, string email, string password, string lastName = null)
    //     {
    //         var user = new User
    //         {
    //             FirstName = firstName.Trim(), LastName = lastName?.Trim(), Email = email.Trim(),
    //             PasswordSalt = KeyGenerator.GetUniqueKey(SaltLength)
    //         };
    //         user.ChangePassword(password);
    //         return user;
    //     }
    //
    //     public void ChangePassword(string newPassword)
    //     {
    //         Password = Sha512Generator.CreateHash(newPassword + PasswordSalt);
    //     }
    // }
}
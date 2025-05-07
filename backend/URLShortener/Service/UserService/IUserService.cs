using URLShortener.DTOs;
using URLShortener.DTOs.User;
using URLShortener.ModelHelpers;
using URLShortener.Models;

namespace URLShortener.Service.User
{
    public interface IUserService
    {
        IEnumerable<UserUrls> GetAllUsers();
        UserUrls GetUserById(int id);
        UserUrls GetUserWithUrls(int id);
        LoginTokens Login(LoginModel req);
        void Logout(int id, string rt);
        string SilentLogin(string rt, string t, int id);
        SignUpModel AddUser(SignUpModel request);
        UserUpdate UpdateUser(int id, UserUpdate userInput);
        void DeleteUser(int id);

    }
}

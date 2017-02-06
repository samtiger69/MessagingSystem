using Domain.Entities;
using System.Collections.Generic;

namespace Core.Abstract
{
    public interface IUserService
    {
        // Takes no parameters, and returns a list of all users.
        IList<User> GetAllUsers();

        // Takes an int parameter, returns all the users of that UserType.
        IList<User> GetAllUsersByTypeId(int typeId);

        // Takes an int parameter, returns all the users of that Organization.
        IList<User> GetAllUsersByOrganizationId(int organizationId, UserTypes userType = UserTypes.User);

        // Takes an int parameter, returns the user with that id or null if it's not found.
        User GetUserById(int userId);

        // Takes an string parameter, returns the user with that username or null if not found.
        User GetUserByUsername(string username);

        // Takes a user object and inserts it to the database.
        int InsertUser(User user);

        // Takes a user object and updates it in the database.
        int UpdateUser(User user);

        // Takes a user object and removes it from the database.
        int DeleteUser(User user);

        IList<User> GetMessageReceiversByMessageId(int messageId);

        // Gets the users that a sender can address
        IList<User> GetUsersBySenderId(int senderId);

        // Gets the users that an admin can manage
        IList<User> GetUsersByAdminId(int adminId);

        // Handles user login.
        bool Login(string username, string password);
    }
}

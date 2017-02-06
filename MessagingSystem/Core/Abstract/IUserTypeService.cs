using Domain.Entities;
using System.Collections.Generic;

namespace Core.Abstract
{
    public interface IUserTypeService
    {

        // Returns all the UserTypes from the database.
        IList<UserType> GetAllUserTypes();

        // Takes and int parameter, and return that UserType form the database or null if not found.
        UserType GetUserTypeById(int UserTypeId);

        // Takes a string parameter, and return that UserType form the database or null if not found.
        UserType GetUserTypeByName(string Name);

        // Takes an int parameter, and returns the UserType for that user id or nul if not found.
        UserType GetUserTypeByUserId(int UserId);

        // Takes a message object and inserts it into the database.
        int InsesrtUserType(UserType userType);

        // Takes a message object and updates it in the database.
        int UpdateUserType(UserType userType);

        // Takes a UserType object and removes from in the database.
        int DeleteUserType(UserType userType);

    }
}

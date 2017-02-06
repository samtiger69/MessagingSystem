using Core.Abstract;
using Core.Implementation;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Helpers
{
    public class BaseHelper
    {
        private IUserService _userService;
        public BaseHelper()
        {
            _userService = new UserService();
        }
        public User GetUser(string name)
        {
            return _userService.GetUserByUsername(name);
        }

        public User GetUserById(int Id)
        {
            return _userService.GetUserById(Id);
        }
    }
}
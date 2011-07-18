using System;
using System.Web.Security;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace MongoMvcApplication.Models
{
    public class AccountRepository
    {
        public MembershipUser CreateUser(string username, string password, string email)
        {
            const string connection = "mongodb://localhost/Users";
            var database = MongoDatabase.Create(connection);
            var collection = database.GetCollection<User>("Users");
            var user = new User
                           {
                               UserName = username,
                               Email = email,
                               Password = password,
                               PasswordSalt = "1234",
                               CreatedDate = DateTime.Now,
                               IsActivated = false,
                               IsLockedOut = false,
                               LastLockedOutDate = DateTime.Now,
                               LastLoginDate = DateTime.Now
                           };

            collection.Insert(user);
            return GetUser(username);
        }

        public string GetUserNameByEmail(string email)
        {
            const string connection = "mongodb://localhost/Users";
            var database = MongoDatabase.Create(connection);
            var collection = database.GetCollection<User>("Users");
            var query = Query.EQ("Email", email);
            var result = collection.FindOne(query);
            return result != null ? result.UserName : String.Empty;
        }

        public MembershipUser GetUser(string username)
        {
            const string connection = "mongodb://localhost/Users";
            var database = MongoDatabase.Create(connection);
            var collection = database.GetCollection<User>("Users");
            var query = Query.EQ("UserName", username);
            var user = collection.FindOne(query);
            if (null == user) return null;
            return new MembershipUser("CustomMembershipProvider",
                                                      user.UserName,
                                                      user.Id,
                                                      user.Email,
                                                      "",
                                                      user.Comments,
                                                      user.IsActivated,
                                                      user.IsLockedOut,
                                                      user.CreatedDate,
                                                      user.LastLoginDate,
                                                      DateTime.Now,
                                                      DateTime.Now,
                                                      user.LastLockedOutDate);
        }
    }

    public class User
    {
        public ObjectId Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordSalt { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsActivated { get; set; }

        public bool IsLockedOut { get; set; }

        public DateTime LastLockedOutDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public string Comments { get; set; }
    }
}
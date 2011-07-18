using System;
using System.Collections.Specialized;
using System.Web.Hosting;
using System.Web.Security;
using MongoMvcApplication.Data;
using MongoMvcApplication.Models;

public class MongoMembershipProvider : MembershipProvider
{
    private string _applicationName;
    private bool _enablePasswordReset;
    private bool _enablePasswordRetrieval;
    private int _maxInvalidPasswordAttempts;
    private int _minRequiredNonalphanumericCharacters;
    private int _minRequiredPasswordLength;
    private int _passwordAttemptWindow;
    private MembershipPasswordFormat _passwordFormat = MembershipPasswordFormat.Hashed;
    private string _passwordStrengthRegularExpression;
    private bool _requiresQuestionAndAnswer;
    private bool _requiresUniqueEmail = true;

    public override string ApplicationName
    {
        get { return _applicationName; }
        set { _applicationName = value; }
    }

    public override bool EnablePasswordReset
    {
        get { return _enablePasswordReset; }
    }

    public override bool EnablePasswordRetrieval
    {
        get { return _enablePasswordRetrieval; }
    }

    public override int MaxInvalidPasswordAttempts
    {
        get { return _maxInvalidPasswordAttempts; }
    }

    public override int MinRequiredNonAlphanumericCharacters
    {
        get { return _minRequiredNonalphanumericCharacters; }
    }

    public override int MinRequiredPasswordLength
    {
        get { return _minRequiredPasswordLength; }
    }

    public override int PasswordAttemptWindow
    {
        get { return _passwordAttemptWindow; }
    }

    public override MembershipPasswordFormat PasswordFormat
    {
        get { return _passwordFormat; }
    }

    public override string PasswordStrengthRegularExpression
    {
        get { return _passwordStrengthRegularExpression; }
    }

    public override bool RequiresQuestionAndAnswer
    {
        get { return _requiresQuestionAndAnswer; }
    }

    public override bool RequiresUniqueEmail
    {
        get { return _requiresUniqueEmail; }
    }

    public override void Initialize(string name, NameValueCollection config)
    {
        if (config == null)
            throw new ArgumentNullException("config");

        if (name.Length == 0)
            name = "CustomMembershipProvider";

        if (String.IsNullOrEmpty(config["description"]))
        {
            config.Remove("description");
            config.Add("description", "Custom Membership Provider");
        }

        base.Initialize(name, config);

        _applicationName = GetConfigValue(config["applicationName"],
                                          HostingEnvironment.ApplicationVirtualPath);
        _maxInvalidPasswordAttempts = Convert.ToInt32(
            GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
        _passwordAttemptWindow = Convert.ToInt32(
            GetConfigValue(config["passwordAttemptWindow"], "10"));
        _minRequiredNonalphanumericCharacters = Convert.ToInt32(
            GetConfigValue(config["minRequiredNonalphanumericCharacters"], "1"));
        _minRequiredPasswordLength = Convert.ToInt32(
            GetConfigValue(config["minRequiredPasswordLength"], "6"));
        _enablePasswordReset = Convert.ToBoolean(
            GetConfigValue(config["enablePasswordReset"], "true"));
        _passwordStrengthRegularExpression = Convert.ToString(
            GetConfigValue(config["passwordStrengthRegularExpression"], ""));
    }

    private static string GetConfigValue(string configValue, string defaultValue)
    {
        return string.IsNullOrEmpty(configValue) ? defaultValue : configValue;
    }

    public override bool ChangePassword(string username, string oldPassword, string newPassword)
    {
        throw new NotImplementedException();
    }

    public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
    {
        return false;
    }

    public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion,
                                              string passwordAnswer, bool isApproved, object providerUserKey,
                                              out MembershipCreateStatus status)
    {
         var args = new ValidatePasswordEventArgs(username, password, true);

         OnValidatingPassword(args);

         if (args.Cancel)
         {
              status = MembershipCreateStatus.InvalidPassword;
              return null;
         }
         if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
         {
              status = MembershipCreateStatus.DuplicateEmail;
              return null;
         }
         var u = GetUser(username, false);
         if (u == null)
         {
              var repository = new UserRepository();
              repository.CreateUser(username, password, email);
              status = MembershipCreateStatus.Success;
              return GetUser(username, false);
         }
         status = MembershipCreateStatus.DuplicateUserName;
         return null;
    }

    public override bool DeleteUser(string username, bool deleteAllRelatedData)
    {
        throw new NotImplementedException();
    }

    public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize,
                                                              out int totalRecords)
    {
        throw new NotImplementedException();
    }

    public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize,
                                                             out int totalRecords)
    {
        throw new NotImplementedException();
    }

    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    {
        throw new NotImplementedException();
    }

    public override int GetNumberOfUsersOnline()
    {
        throw new NotImplementedException();
    }

    public override string GetPassword(string username, string answer)
    {
        throw new NotImplementedException();
    }

    public override MembershipUser GetUser(string username, bool userIsOnline)
    {
        var repository = new UserRepository();
        return repository.GetUser(username);
    }

    public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
    {
        throw new NotImplementedException();
    }

    public override string GetUserNameByEmail(string email)
    {
        var repository = new UserRepository();
        return repository.GetUserNameByEmail(email);
    }

    public override string ResetPassword(string username, string answer)
    {
        throw new NotImplementedException();
    }

    public override bool UnlockUser(string userName)
    {
        throw new NotImplementedException();
    }

    public override void UpdateUser(MembershipUser user)
    {
        throw new NotImplementedException();
    }

    public override bool ValidateUser(string username, string password)
    {
        var repository = new UserRepository();
        return repository.ValidateUser(username, password);
    }
}
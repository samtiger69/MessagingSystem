namespace Domain.Entities
{
    public enum MessageStatus
    {
        New,
        Read,
        Trashed
    }

    public enum PermissionType
    {
        UNSPECIFIED = -1,
        SendLocal = 1,
        AdminLocal = 2,
        SendGlobal = 4,
        AdminGlobal = 8
    }

    public enum NotificationType
    {
        Error = 1,
        Success = 2
    }

    public enum UserTypes
    {
        SuperAdmin = 2,
        Admin = 3,
        User = 4
    }

    public static class StoredProcedures
    {

        // User Stored Procedures.
        public const string GET_USERS_BY_SENDER_ID = "Get_Users_By_SenderId";
        public const string GET_USERS = "Get_Users";
        public const string INSERT_USER = "Insert_User";
        public const string GET_MESSAGE_RECEIVERS = "Get_Message_Receivers";
        public const string UPDATE_USER = "Update_User";
        public const string USER_LOGIN = "User_Login";
        public const string GET_USERS_BY_ADMIN_ID = "Get_Users_By_AdminId";


        // UserType Stored Procedures.
        public const string DELETE_USER_TYPE = "Delete_UserType";
        public const string GET_USER_TYPES = "Get_UserTypes";
        public const string INSERT_USER_TYPE = "Insert_UserType";
        public const string UPDATE_USER_TYPE = "Update_UserType";

        // UserPermission Stored Procedures.
        public const string DELETE_USER_PERMISSION = "Delete_UserPermission";
        public const string GET_USER_PERMISSIONS = "Get_UserPermissions";
        public const string GET_SENDER_USER_PERMISSIONS = "Get_Sending_UserPermissions";
        public const string GET_ADMIN_USER_PERMISSIONS = "Get_Admin_UserPermissions";
        public const string INSERT_USER_PERMISSION = "Insert_UserPermission";
        public const string UPDATE_USER_PERMISSION = "Update_UserPermission";

        // UserMessage Stored Procedures.
        public const string GET_INBOX = "Get_Inbox";
        public const string UPDATE_USER_MESSAGE = "Update_UserMessage";

        // Organization Stored Procedures.
        public const string DELETE_ORGANIZATION = "Delete_Organization";
        public const string GET_ORGANIZATIONS = "Get_Organizations";
        public const string GET_USER_ORGANIZATIONS_PERMISSIONS = "Get_User_Organizations_Permissions";
        public const string GET_ORGANIZATION_CHILDREN = "Get_Organization_Children";
        public const string INSERT_ORGANIZATION = "Insert_Organization";
        public const string UPDATE_ORGANIZATION = "Update_Organization";

        // Message Stored Procedures.
        public const string UPDATE_MESSAGE = "Update_Message";
        public const string INSERT_MESSAGE = "Insert_Message";
        public const string SEND_MESSAGE = "Send_Message";
        public const string GET_MESSAGE = "Get_Message";


    }
}

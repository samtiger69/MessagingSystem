using Core.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using System.Data.SqlClient;

namespace Core.Implementation
{
    public class UserMessageService : IUserMessageService
    {
        #region Fields
        private const int UNSPECIFIED = -1;
        private IDatabaseExecuter _databaseExecuter;
        private IMessageService _messageService;
        #endregion

        #region Constructors
        public UserMessageService()
        {
            _messageService = new MessageService();
            _databaseExecuter = DatabaseExecuter.GetInstance();
        }
        #endregion

        #region Actions
        public IList<UserMessage> GetInbox(int userId, bool includeDeleted = false)
        {
            IList<UserMessage> result = new List<UserMessage>();
            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_INBOX,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@UserId", userId));
                        command.Parameters.Add(new SqlParameter("@MessageId", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@MessageStatus", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@IncludeDeleted", includeDeleted));
                    },
                    delegate (SqlDataReader reader)
                    {
                        result = FillInboxList(reader);
                    }
                    );
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public IList<UserMessage> GetTrashUserMessagesByUserId(int userId, bool includeDeleted = false)
        {
            IList<UserMessage> result = new List<UserMessage>();

            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_INBOX,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@UserId", userId));
                        command.Parameters.Add(new SqlParameter("@MessageId", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@MessageStatus", MessageStatus.Trashed));
                        command.Parameters.Add(new SqlParameter("@IncludeDeleted", includeDeleted));
                    },
                    delegate (SqlDataReader reader)
                    {
                        result = FillInboxList(reader);
                    }
                    );
            }
            catch (Exception e)
            {
                // Error logging.
            }

            return result;
        }

        public UserMessage GetUserMessage(int messageId, int userId, bool includeDeleted = false)
        {
            IList<UserMessage> result = new List<UserMessage>();

            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_INBOX,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@UserId", userId));
                        command.Parameters.Add(new SqlParameter("@MessageId", messageId));
                        command.Parameters.Add(new SqlParameter("@MessageStatus", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@IncludeDeleted", includeDeleted));
                    },
                    delegate (SqlDataReader reader)
                    {
                        result = FillInboxList(reader);
                    }
                    );
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result.FirstOrDefault();
        }

        public void MakeMessageRead(UserMessage message)
        {
            message.messageStatus = message.messageStatus | MessageStatus.Read;
            UpdateUserMessage(message);
        }

        public void MoveMessageToTrash(UserMessage message)
        {
            message.messageStatus = message.messageStatus | MessageStatus.Trashed;
            UpdateUserMessage(message);
        }

        public void DeleteUserMessage(UserMessage message)
        {
            message.IsDeleted = true;
            UpdateUserMessage(message);
        }

        private void UpdateUserMessage(UserMessage message)
        {
            try
            {
                _databaseExecuter.ExecuteChange(StoredProcedures.UPDATE_USER_MESSAGE,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@UserId", message.userId));
                        command.Parameters.Add(new SqlParameter("@MessageId", message.messageId));
                        command.Parameters.Add(new SqlParameter("@MessageStatus", message.messageStatus));
                        command.Parameters.Add(new SqlParameter("@IsDeleted", message.IsDeleted));
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
        }

        public void RestoreUserMessageToInbox(UserMessage message)
        {
            message.messageStatus = ((int)message.messageStatus - MessageStatus.Trashed);
            UpdateUserMessage(message);
        }

        public IList<UserMessage> GetUserMessagesByMessageId(int messageId)
        {
            IList<UserMessage> result = new List<UserMessage>();

            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_INBOX,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@UserId", UNSPECIFIED));
                        command.Parameters.Add(new SqlParameter("@MessageId", messageId));
                        command.Parameters.Add(new SqlParameter("@IncludeDeleted", true));
                        command.Parameters.Add(new SqlParameter("@MessageStatus", UNSPECIFIED));
                    },
                    delegate (SqlDataReader reader)
                    {
                        result = FillInboxList(reader);
                    }
                    );
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        private IList<UserMessage> FillInboxList(SqlDataReader reader)
        {

            IList<UserMessage> result = new List<UserMessage>();
            while (reader.Read())
            {
                result.Add(new UserMessage
                {
                    Message = _messageService.GetMessageById(Convert.ToInt16(reader[0]), true),
                    messageId = Convert.ToInt16(reader[0]),
                    userId = Convert.ToInt16(reader[1]),
                    messageStatus = (MessageStatus)reader[2]
                });
            }

            return result;
        }
        #endregion
    }
}

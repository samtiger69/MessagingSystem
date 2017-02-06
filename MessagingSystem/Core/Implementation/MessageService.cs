using Core.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using System.Data.SqlClient;

namespace Core.Implementation
{
    public class MessageService : IMessageService
    {
        #region Fields
        private IUserService _UserService;
        private const int UNSPECIFIED = -1;
        private IDatabaseExecuter _databaseExecuter;
        #endregion

        #region Constructors
        public MessageService()
        {
            _databaseExecuter = DatabaseExecuter.GetInstance();
            _UserService = new UserService();
        }
        #endregion

        #region Actions
        // Marks a message as Deleted in the database.
        public int DeleteMessage(Message message)
        {
            message.isDeleted = true;
            var result = 0;

            try
            {
                result = _databaseExecuter.ExecuteChange(StoredProcedures.UPDATE_MESSAGE,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Id", message.Id));
                        command.Parameters.Add(new SqlParameter("@IsDeleted", message.isDeleted));
                    });
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        // Inserts a message, and then send it to it's receiver/s.
        public int InsertMessage(Message message, IList<User> receivers)
        {
            var messageId = 0;

            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.INSERT_MESSAGE,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@Title", message.Title));
                        command.Parameters.Add(new SqlParameter("@Content", message.Content));
                        command.Parameters.Add(new SqlParameter("@SendDate", message.SendDate));
                        command.Parameters.Add(new SqlParameter("@IsDeleted", false));
                        command.Parameters.Add(new SqlParameter("@SenderUserId", message.SenderUserId));
                    },
                    delegate (SqlDataReader reader)
                    {
                        reader.Read();
                        messageId = Convert.ToInt16(reader[0]);
                    });

                foreach (var receiver in receivers)
                {
                    _databaseExecuter.ExecuteChange(StoredProcedures.SEND_MESSAGE,
                    delegate (SqlCommand command)
                    {
                        command.Parameters.Add(new SqlParameter("@MessageId", messageId));
                        command.Parameters.Add(new SqlParameter("@UserId", receiver.Id));
                        command.Parameters.Add(new SqlParameter("@MessageStatus", MessageStatus.New));
                    });
                }
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return messageId;
        }

        public IList<Message> GetAllMessagesByUserId(int userId, bool includeDeleted = false)
        {
            IList<Message> result = new List<Message>();

            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_MESSAGE,
                delegate (SqlCommand command)
                {
                    command.Parameters.Add(new SqlParameter("@UserId", userId));
                    command.Parameters.Add(new SqlParameter("@Id", -1));
                    command.Parameters.Add(new SqlParameter("@IncludeDeleted", includeDeleted));
                },
                delegate (SqlDataReader reader)
                {
                    result = FillMessageList(reader);
                }
                );
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result;
        }

        public Message GetMessageById(int messageId, bool includeDeleted = false)
        {
            IList<Message> result = new List<Message>();

            try
            {
                _databaseExecuter.ExecuteReader(StoredProcedures.GET_MESSAGE,
                delegate (SqlCommand command)
                {
                    command.Parameters.Add(new SqlParameter("@Id", messageId));
                    command.Parameters.Add(new SqlParameter("@IncludeDeleted", includeDeleted));
                    command.Parameters.Add(new SqlParameter("@UserId", -1));
                },
                delegate (SqlDataReader reader)
                {
                    result = FillMessageList(reader);
                }
                );
            }
            catch (Exception e)
            {
                // Error logging.
            }
            return result.FirstOrDefault();
        }

        // Fills DataReader data into Message List.
        private IList<Message> FillMessageList(SqlDataReader reader)
        {
            IList<Message> result = new List<Message>();
            while (reader.Read())
            {
                result.Add(new Message
                {
                    Id = Convert.ToInt16(reader[0]),
                    Title = reader[1].ToString(),
                    Content = reader[2].ToString(),
                    SendDate = Convert.ToDateTime(reader[3]),
                    isDeleted = Convert.ToBoolean(reader[4]),
                    SenderUserId = Convert.ToInt16(reader[5]),
                    Sender = _UserService.GetUserById(Convert.ToInt16(reader[5])),
                    messageReceivers = _UserService.GetMessageReceiversByMessageId(Convert.ToInt16(reader[0]))
                });
            }
            return result;
        }
        #endregion
    }
}

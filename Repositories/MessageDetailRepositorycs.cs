using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using test2.DatabaseContext;
using test2.DatabaseContext.Models;

namespace test2.Repositories
{
    public class MessageDetailRepository
    {
        LockerDbContext _dbContext;

        public MessageDetailRepository(LockerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /* Add message => Create new message                                                                                *
         * input = _message                                                                                                 *
         *      Attrinbutes = int Id_message, DateTime Date, DateTime Time, bool IsShow, int id_content, string Id_account  */
        public bool AddMessage(MessageDetail _message)
        {

            try
            {
                if(CheckId_content(_message.Id_content))
                {
                    return false;
                }
                if(CheckId_account(_message.Id_account))
                {
                    return false;
                }
                _dbContext.MessageDetails.Add(_message);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Exception error");
                return false;
            }
        }

        /* CheckId_content that has id_content in Contents database     *
         *  input = int id_content                                      *
         * return true if there aren't id_content in database           */
        public bool CheckId_content (int id_content)
        {
            return _dbContext.Contents.FirstOrDefault(x => x.Id_content == id_content) == null;
        }

        /* CheckId_account that has id_account in Accounts database     *
         *  input = int id_account                                      *
         * return true if there aren't id_account in database           */
        public bool CheckId_account (string id_account)
        {
            return _dbContext.Accounts.FirstOrDefault(x => x.Id_account == id_account) == null;
        }

        /* DeleteMessage            *
         *  input = int id          *
         *      set IsShow = false  */
        public bool DeleteMessage(int id)
        {
            try
            {
                _dbContext.MessageDetails.FirstOrDefault(x => x.Id_message == id).IsShow = false;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Exception error");
                return false;
            }
        }

        /* Get all message                      *
         * return all message detail to string  */
        public List<MessageDetail> GetMessage()
        {
            return _dbContext.MessageDetails.ToList();
        }

        /* Get specific message                             *
         *  input = int id_message                          *
         * return message that has Id_message = id_message  */
        public List<MessageDetail> GetMessage(int id_message)
        {
            var ownmessagelist = from messagelist in _dbContext.MessageDetails
                                 where messagelist.Id_message == id_message
                                 select messagelist;
            return ownmessagelist.ToList();

        }
        
        /* Get active message for one user                  *
         *  input = string id_account                       *
         * return idActivemessage to list                   */
        public List<MessageDetail> GetActiveMessage (string id_account)
        {
            var idActivemessage = from messagelist in _dbContext.MessageDetails
                                  where messagelist.Id_account == id_account && messagelist.IsShow == true
                                  select messagelist;
            return idActivemessage.ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using test2.DatabaseContext;
using test2.DatabaseContext.Models;

namespace test2.Repositories
{
    public class ContentRepository
    {
        LockerDbContext _dbContext;

        public ContentRepository(LockerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /* Add new content                                                                  *
         * input = content                                                                  *
         *          Attributes => int Id_content, string plaintext, bool IsActive           *
         *              Default IsActive = "true"                                           */                        
        public bool AddContent(Content _content)
        {

            try
            {
                _dbContext.Contents.Add(_content);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Exception error");
                return false;
            }
        }

        /* Delete Content                                   *
         * Input = Id_content                               *
         *      set that Id_content has IsActive = false                        */
        public bool DeleteContent(int id)
        {
            try
            {
                _dbContext.Contents.FirstOrDefault(x => x.Id_content == id).IsActive = false;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Exception error");
                return false;
            }
        }

        /* Set Active => set content can use.                   *
         * Input = Id_content                                   *
         *      set that Id_content has IsActive = true         */
        public bool SetActive(int id)
        {
            try
            {
                if (_dbContext.Contents.FirstOrDefault(x => x.Id_content == id) != null)
                {
                    _dbContext.Contents.FirstOrDefault(x => x.Id_content == id).IsActive = true;
                    _dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /* Get All Content          *
         * return all Content       */
        public List<Content> GetContent()
        {
            return _dbContext.Contents.ToList();
        }

        /* Get Content                                  *
         * Input = Id_content                           *
         * return content that has Id_content = Input   */
        public List<Content> GetContent(int id)
        {

            return _dbContext.Contents.Where(x => x.Id_content == id).ToList();
        }
    }
}

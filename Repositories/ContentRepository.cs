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
                _dbContext.contents.Add(_content);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot add content");
                return false;
            }
        }

        /* Delete Content                                   *
         * Input = Id_content                               *
         *      set that Id_content has IsActive = false    */
        public bool DeleteContent(int id)
        {
            try
            {
                _dbContext.contents.FirstOrDefault(x => x.Id_content == id).IsActive = false;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot delete content");
                return false;
            }
        }

        /* Restore content => set content can use.                   *
         * Input = Id_content                                   *
         *      set that Id_content has IsActive = true         */
        public bool RestoreContent(int id)
        {
            try
            {
                if (_dbContext.contents.FirstOrDefault(x => x.Id_content == id) != null)
                {
                    _dbContext.contents.FirstOrDefault(x => x.Id_content == id).IsActive = true;
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
        public List<Content> GetAllContent()
        {
            return _dbContext.contents.ToList();
        }

        /* Get all Content                                  *
         * Input = Id_content                           *
         * return content that has Id_content = Input   */

        public List<Content> GetContent()
        {
            return _dbContext.contents.Where(x => x.IsActive == true).ToList();
        }

        /* Get specific Content                                  *
        * Input = Id_content                           *
        * return content that has Id_content = Input   */
        public List<Content> GetContent(int id)
        {
            var content = from contentlist in _dbContext.contents
                          where contentlist.Id_content == id && contentlist.IsActive == true
                          select contentlist;
            return content.ToList();
        }

      
    }
}

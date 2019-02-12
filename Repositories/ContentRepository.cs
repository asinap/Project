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
                _dbContext.Contents.FirstOrDefault(x => x.Id_content == id).IsActive = false;
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
        public List<Content> GetAllContent()
        {
            return _dbContext.Contents.ToList();
        }

        /* Get Content                                  *
         * Input = Id_content                           *
         * return content that has Id_content = Input   */

        public List<Content> GetContent()
        {
            return _dbContext.Contents.Where(x => x.IsActive == true).ToList();
        }

        public List<Content> GetContent(int id)
        {
            var content = from contentlist in _dbContext.Contents
                          where contentlist.Id_content == id && contentlist.IsActive == true
                          select contentlist;
            return content.ToList();
        }

        /*Delete*/
        public bool Delete()
        {
            try
            {
                var data = from list in _dbContext.Contents select list;
                _dbContext.Contents.RemoveRange(data);
                _dbContext.SaveChanges();
                return true;

            }
            catch (Exception)
            {
                Console.Write("Cannot delete all contents database");
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                if (_dbContext.Contents.Where(x => x.Id_content == id) == null)
                {
                    return false;
                }
                var data = from list in _dbContext.Contents
                           where list.Id_content == id
                           select list;
                _dbContext.Contents.RemoveRange(data);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                Console.Write("Cannot delete %s", id);
                return false;
            }
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface iDatingRepository
    {
        void Add<T>(T entity) where T: class;
        void Delete<T>(T entity) where T: class;
        Task<bool> SaveAll();         // metodo per salvare modifiche al nostro DB
        Task<IEnumerable<User>> GetUsers();  // metodo che fa tornare un elenco di utenti dal nostro DB
        Task<User> GetUser(int id);       // metodo che fa tornare un singolo utente dal nostro DB

        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMainPhotoForUser(int userId);
    }
}
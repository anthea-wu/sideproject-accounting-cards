using System.Threading.Tasks;
using accounting_cards.Models;
using Google.Cloud.Firestore;

namespace accounting_cards
{
    public interface IUserRepository
    {
        Task<DocumentSnapshot> GetUser(string id);
        DocumentReference CreateUser();
        Task<WriteResult> UpdateUser(DocumentReference user, User newUser);
    }
}
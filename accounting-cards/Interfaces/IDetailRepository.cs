using System.Threading.Tasks;
using accounting_cards.Models;
using Google.Cloud.Firestore;

namespace accounting_cards
{
    public interface IDetailRepository
    {
        Task<QuerySnapshot> Get(string userId, string cardId);
        Task Create(Detail newDetail);
        Task Delete(Detail deleteDetail);
    }
}
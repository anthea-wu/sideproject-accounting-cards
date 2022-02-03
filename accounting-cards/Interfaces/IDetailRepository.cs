using System.Threading.Tasks;
using accounting_cards.Models;
using Google.Cloud.Firestore;

namespace accounting_cards
{
    public interface IDetailRepository
    {
        Task<QuerySnapshot> Get(string userId, string cardId);
        Task<DocumentSnapshot> Get(string userId, string cardId, string detailId);
        Task<string> Create(Detail newDetail);
        Task Delete(Detail deleteDetail);
    }
}
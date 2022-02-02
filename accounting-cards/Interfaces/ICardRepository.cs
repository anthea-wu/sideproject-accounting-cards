using System.Threading.Tasks;
using accounting_cards.Models;
using Google.Cloud.Firestore;

namespace accounting_cards
{
    public interface ICardRepository
    {
        Task<QuerySnapshot> GetList(string id);
        Task<DocumentSnapshot> GetCard(string userId, string cardId);
        Task<WriteResult> UpdateCard(Card newCard, DocumentReference cardCollection);
        Task<WriteResult> UpdateCard(Card updateCard);
        DocumentReference CreateNewCard(Card newCard);
        Task<WriteResult> DeleteCard(Card deleteCard);
    }
}
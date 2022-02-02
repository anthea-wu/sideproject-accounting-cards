using System.IO;
using System.Threading.Tasks;
using accounting_cards.Models;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using WriteResult = Google.Cloud.Firestore.WriteResult;

namespace accounting_cards
{
    public class CardRepository : ICardRepository
    {
        private static readonly string _jsonPath = Properties.Settings.Default.FirebaseJsonPath;
        static readonly string _jsonStr = File.ReadAllText(_jsonPath);
        private static readonly FirestoreClientBuilder _builder = new FirestoreClientBuilder(){JsonCredentials = _jsonStr};
        private readonly FirestoreDb _db = FirestoreDb.Create("accounting-cards", _builder.Build());

        public Task<QuerySnapshot> GetList(string id)
        {
            return _db
                .Collection("users").Document(id)
                .Collection("cards").GetSnapshotAsync();
        }
        
        public Task<DocumentSnapshot> GetCard(string userId, string cardId)
        {
            return _db
                .Collection("users").Document(userId)
                .Collection("cards").Document(cardId)
                .GetSnapshotAsync();
        }

        public Task<WriteResult> UpdateCard(Card newCard, DocumentReference cardCollection)
        {
            return cardCollection.SetAsync(newCard);
        }
        
        public Task<WriteResult> UpdateCard(Card updateCard)
        {
            return _db
                .Collection("users").Document(updateCard.UserId)
                .Collection("cards").Document(updateCard.Id).SetAsync(updateCard);
        }


        public DocumentReference CreateCard(string userId)
        {
            return _db
                .Collection("users").Document(userId)
                .Collection("cards").Document();
        }
        
        public Task<WriteResult> DeleteCard(Card deleteCard)
        {
            return _db
                .Collection("users").Document(deleteCard.UserId)
                .Collection("cards").Document(deleteCard.Id).DeleteAsync();
        }

    }
}
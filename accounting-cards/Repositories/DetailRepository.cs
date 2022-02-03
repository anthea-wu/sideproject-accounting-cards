using System.IO;
using System.Threading.Tasks;
using accounting_cards.Models;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

namespace accounting_cards
{
    public class DetailRepository : IDetailRepository
    {
        private static readonly string _jsonPath = Properties.Settings.Default.FirebaseJsonPath;
        static readonly string _jsonStr = File.ReadAllText(_jsonPath);
        private static readonly FirestoreClientBuilder _builder = new FirestoreClientBuilder(){JsonCredentials = _jsonStr};
        private readonly FirestoreDb _db = FirestoreDb.Create("accounting-cards", _builder.Build());
        
        public async Task<QuerySnapshot> Get(string userId, string cardId)
        {
            var results = await _db
                .Collection("users").Document(userId)
                .Collection("cards").Document(cardId)
                .Collection("details").GetSnapshotAsync();
            return results;
        }

        public async Task<DocumentSnapshot> Get(string userId, string cardId, string detailId)
        {
            var result = await _db
                .Collection("users").Document(userId)
                .Collection("cards").Document(cardId)
                .Collection("details").Document(detailId)
                .GetSnapshotAsync();
            return result;
        }
        
        public async Task<string> Create(Detail newDetail)
        {
            var document = _db
                .Collection("users").Document(newDetail.UserId)
                .Collection("cards").Document(newDetail.CardId)
                .Collection("details").Document();

            newDetail.Id = document.Id;
            await document.CreateAsync(newDetail);

            return newDetail.Id;
        }
        
        public async Task Delete(Detail deleteDetail)
        {
            await _db
                .Collection("users").Document(deleteDetail.UserId)
                .Collection("cards").Document(deleteDetail.CardId)
                .Collection("details").Document(deleteDetail.Id)
                .DeleteAsync();
        }
    }
}
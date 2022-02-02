using System.IO;
using System.Threading.Tasks;
using accounting_cards.Models;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using WriteResult = Google.Cloud.Firestore.WriteResult;

namespace accounting_cards
{
    public class UserRepository : IUserRepository
    {
        private static readonly string _jsonPath = Properties.Settings.Default.FirebaseJsonPath;
        static readonly string _jsonStr = File.ReadAllText(_jsonPath);
        private static readonly FirestoreClientBuilder _builder = new FirestoreClientBuilder(){JsonCredentials = _jsonStr};
        private readonly FirestoreDb _db = FirestoreDb.Create("accounting-cards", _builder.Build());
        
        public Task<DocumentSnapshot> GetUser(string id)
        {
            return _db.Collection("users").Document(id).GetSnapshotAsync();
        }
        
        public DocumentReference CreateUser()
        {
            return _db.Collection("users").Document();
        }
        
        public Task<WriteResult> UpdateUser(DocumentReference user, User newUser)
        {
            return user.SetAsync(newUser);
        }


    }
}
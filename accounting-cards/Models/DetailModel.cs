using System;
using Google.Cloud.Firestore;

namespace accounting_cards.Models
{
    [FirestoreData]
    public class Detail
    {
        [FirestoreProperty]
        public string Id { get; set; }
        
        [FirestoreProperty]
        public string CardId { get; set; }
        
        [FirestoreProperty]
        public string UserId { get; set; }
        
        [FirestoreProperty]
        public string Name { get; set; }
        
        [FirestoreProperty]
        public int Count { get; set; }
        
        [FirestoreProperty]
        public DateTimeOffset CreateTime { get; set; }
    }
}
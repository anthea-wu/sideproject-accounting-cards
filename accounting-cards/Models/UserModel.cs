using System;
using Google.Cloud.Firestore;

namespace accounting_cards.Models
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty]
        public DateTimeOffset CreateTime { get; set; }
        
        [FirestoreProperty]
        public string Id { get; set; }
    }
}
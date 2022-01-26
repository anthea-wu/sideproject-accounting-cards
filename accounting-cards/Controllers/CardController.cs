using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Newtonsoft.Json;

namespace accounting_cards.Controllers
{
    [RoutePrefix("api/card")]
    public class CardController : ApiController
    {
        private static readonly string _jsonPath = Properties.Settings.Default.FirebaseJsonPath;
        static readonly string _jsonStr = File.ReadAllText(_jsonPath);
        private static readonly FirestoreClientBuilder _builder = new FirestoreClientBuilder(){JsonCredentials = _jsonStr};
        private readonly FirestoreDb _db = FirestoreDb.Create("accounting-cards", _builder.Build());
        
        private static readonly List<Card> _defaultCard = new List<Card>()
        {
            new Card()
            {
                Guid = Guid.NewGuid(),
                Name = "未分類",
                Total = 0
            },
            new Card()
            {
                Guid = Guid.NewGuid(),
                Name = "飲食",
                Total = 0
            }
        };

        [HttpGet]
        [Route("{id}/list")]
        public async Task<IHttpActionResult> List(string id)
        {
            var cards = new List<UserCard>();
            
            var results = await _db
                .Collection("users").Document(id)
                .Collection("cards").OrderBy("CreateTime")
                .GetSnapshotAsync();
            foreach (var result in results)
            {
                var card = result.ConvertTo<UserCard>();
                cards.Add(card);
            }
            return Ok(cards);
        }

        [HttpGet]
        [Route("{userId}/{cardId}")]
        public async Task<IHttpActionResult> Item(string userId, string cardId)
        {
            var result =  await _db
                .Collection("users").Document(userId)
                .Collection("cards").Document(cardId)
                .GetSnapshotAsync();

            var card = result.ConvertTo<UserCard>();

            return Ok(card);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Add(UserCard newCard)
        {
            var cardCollection = _db.Collection("users").Document(newCard.UserId).Collection("cards").Document();
            newCard.Id = cardCollection.Id;
            newCard.CreateTime = DateTimeOffset.Now;
            await cardCollection.SetAsync(newCard);
            
            var results =  await _db
                .Collection("users").Document(newCard.UserId)
                .Collection("cards").OrderBy("CreateTime")
                .GetSnapshotAsync();

            var cards = new List<UserCard>();
            foreach (var result in results)
            {
                var card = result.ConvertTo<UserCard>();
                cards.Add(card);
            }
            return Ok(cards);
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(UserCard deleteCard)
        {
            await _db
                .Collection("users").Document(deleteCard.UserId)
                .Collection("cards").Document(deleteCard.Id).DeleteAsync();
            
            var results =  await _db
                .Collection("users").Document(deleteCard.UserId)
                .Collection("cards").OrderBy("CreateTime")
                .GetSnapshotAsync();

            var cards = new List<UserCard>();
            foreach (var result in results)
            {
                var card = result.ConvertTo<UserCard>();
                cards.Add(card);
            }
            return Ok(cards);
        }

        [HttpPut]
        public async Task<IHttpActionResult> Update(UserCard updateCard)
        {
            await _db
                .Collection("users").Document(updateCard.UserId)
                .Collection("cards").Document(updateCard.Id).SetAsync(updateCard);

            var results =  await _db
                .Collection("users").Document(updateCard.UserId)
                .Collection("cards").OrderBy("CreateTime")
                .GetSnapshotAsync();

            var cards = new List<UserCard>();
            foreach (var result in results)
            {
                var card = result.ConvertTo<UserCard>();
                cards.Add(card);
            }
            return Ok(cards);
        }
    }

    public class Card
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public int Total { get; set; }
    }
}
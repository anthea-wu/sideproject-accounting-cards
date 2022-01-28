using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using accounting_cards.Models;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

namespace accounting_cards.Controllers
{
    [RoutePrefix("api/card")]
    public class CardController : ApiController
    {
        private static readonly string _jsonPath = Properties.Settings.Default.FirebaseJsonPath;
        static readonly string _jsonStr = File.ReadAllText(_jsonPath);
        private static readonly FirestoreClientBuilder _builder = new FirestoreClientBuilder(){JsonCredentials = _jsonStr};
        private readonly FirestoreDb _db = FirestoreDb.Create("accounting-cards", _builder.Build());

        [HttpGet]
        [Route("{id}/list")]
        public async Task<IHttpActionResult> List(string id)
        {
            var cards = new List<Card>();
            
            var results = await _db
                .Collection("users").Document(id)
                .Collection("cards").OrderBy("CreateTime")
                .GetSnapshotAsync();
            foreach (var result in results)
            {
                var card = result.ConvertTo<Card>();
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

            var card = result.ConvertTo<Card>();

            return Ok(card);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Add(Card newCard)
        {
            var cardCollection = _db.Collection("users").Document(newCard.UserId).Collection("cards").Document();
            newCard.Id = cardCollection.Id;
            newCard.CreateTime = DateTimeOffset.Now;
            await cardCollection.SetAsync(newCard);
            
            var results =  await _db
                .Collection("users").Document(newCard.UserId)
                .Collection("cards").OrderBy("CreateTime")
                .GetSnapshotAsync();

            var cards = new List<Card>();
            foreach (var result in results)
            {
                var card = result.ConvertTo<Card>();
                cards.Add(card);
            }
            return Ok(cards);
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(Card deleteCard)
        {
            await _db
                .Collection("users").Document(deleteCard.UserId)
                .Collection("cards").Document(deleteCard.Id).DeleteAsync();
            
            var results =  await _db
                .Collection("users").Document(deleteCard.UserId)
                .Collection("cards").OrderBy("CreateTime")
                .GetSnapshotAsync();

            var cards = new List<Card>();
            foreach (var result in results)
            {
                var card = result.ConvertTo<Card>();
                cards.Add(card);
            }
            return Ok(cards);
        }

        [HttpPut]
        public async Task<IHttpActionResult> Update(Card updateCard)
        {
            await _db
                .Collection("users").Document(updateCard.UserId)
                .Collection("cards").Document(updateCard.Id).SetAsync(updateCard);

            var results =  await _db
                .Collection("users").Document(updateCard.UserId)
                .Collection("cards").OrderBy("CreateTime")
                .GetSnapshotAsync();

            var cards = new List<Card>();
            foreach (var result in results)
            {
                var card = result.ConvertTo<Card>();
                cards.Add(card);
            }
            return Ok(cards);
        }
    }
}
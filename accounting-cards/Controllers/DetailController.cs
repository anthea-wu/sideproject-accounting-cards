using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Newtonsoft.Json;

namespace accounting_cards.Controllers
{
    [RoutePrefix("api/detail")]
    public class DetailController : ApiController
    {
        private static readonly string _jsonPath = Properties.Settings.Default.FirebaseJsonPath;
        static readonly string _jsonStr = File.ReadAllText(_jsonPath);
        private static readonly FirestoreClientBuilder _builder = new FirestoreClientBuilder(){JsonCredentials = _jsonStr};
        private readonly FirestoreDb _db = FirestoreDb.Create("accounting-cards", _builder.Build());

        [HttpGet]
        [Route("{userId}/{cardId}")]
        public async Task<IHttpActionResult> List(string userId, string cardId)
        {
            var results = await _db
                .Collection("users").Document(userId)
                .Collection("cards").Document(cardId)
                .Collection("details").GetSnapshotAsync();

            var details = new List<UserCardDetail>();
            foreach (var result in results)
            {
                var detail = result.ConvertTo<UserCardDetail>();
                details.Add(detail);
            }
            
            return Ok(details);
        }
        
        [HttpPost]
        [Route("item")]
        public async Task<IHttpActionResult> Add(UserCardDetail newDetail)
        {
            var document = _db
                .Collection("users").Document(newDetail.UserId)
                .Collection("cards").Document(newDetail.CardId)
                .Collection("details").Document();
            
            newDetail.Id = document.Id;
            await document.CreateAsync(newDetail);
            
            var results = await _db
                .Collection("users").Document(newDetail.UserId)
                .Collection("cards").Document(newDetail.CardId)
                .Collection("details").GetSnapshotAsync();
            
            var details = new List<UserCardDetail>();
            foreach (var result in results)
            {
                var detail = result.ConvertTo<UserCardDetail>();
                details.Add(detail);
            }
            
            return Ok(details);
        }

        [HttpDelete]
        [Route("item")]
        public async Task<IHttpActionResult> Delete(UserCardDetail deleteDetail)
        {
            await _db
                .Collection("users").Document(deleteDetail.UserId)
                .Collection("cards").Document(deleteDetail.CardId)
                .Collection("details").Document(deleteDetail.Id)
                .DeleteAsync();
            
            var results = await _db
                .Collection("users").Document(deleteDetail.UserId)
                .Collection("cards").Document(deleteDetail.CardId)
                .Collection("details").GetSnapshotAsync();
            
            var details = new List<UserCardDetail>();
            foreach (var result in results)
            {
                var detail = result.ConvertTo<UserCardDetail>();
                details.Add(detail);
            }
            
            return Ok(details);
        }

        [HttpPut]
        [Route("item")]
        public async Task<IHttpActionResult> Update(UserCardDetail updateDetail, string oldCardId)
        {
            await _db
                .Collection("users").Document(updateDetail.UserId)
                .Collection("cards").Document(oldCardId)
                .Collection("details").Document(updateDetail.Id).DeleteAsync();

            await _db
                .Collection("users").Document(updateDetail.UserId)
                .Collection("cards").Document(updateDetail.CardId)
                .Collection("details").Document(updateDetail.Id).CreateAsync(updateDetail);
            
            var results = await _db
                .Collection("users").Document(updateDetail.UserId)
                .Collection("cards").Document(updateDetail.CardId)
                .Collection("details").GetSnapshotAsync();
            
            var details = new List<UserCardDetail>();
            foreach (var result in results)
            {
                var detail = result.ConvertTo<UserCardDetail>();
                details.Add(detail);
            }
            
            return Ok(details);
        }
    }

    [FirestoreData]
    public class UserCardDetail
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

    public class DetailList
    {
        public Guid CardGuid { get; set; }
        public List<Detail> Details { get; set; }
    }

    public class Detail
    {
        /// <summary> 紀錄卡片類別的唯一碼 </summary>
        public Guid CardGuid { get; set; }
        
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
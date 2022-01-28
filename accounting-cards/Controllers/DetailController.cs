using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using accounting_cards.Models;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

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

            var details = new List<Detail>();
            foreach (var result in results)
            {
                var detail = result.ConvertTo<Detail>();
                details.Add(detail);
            }
            
            return Ok(details);
        }
        
        [HttpPost]
        [Route("item")]
        public async Task<IHttpActionResult> Add(Detail newDetail)
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
            
            var details = new List<Detail>();
            foreach (var result in results)
            {
                var detail = result.ConvertTo<Detail>();
                details.Add(detail);
            }
            
            return Ok(details);
        }

        [HttpDelete]
        [Route("item")]
        public async Task<IHttpActionResult> Delete(Detail deleteDetail)
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
            
            var details = new List<Detail>();
            foreach (var result in results)
            {
                var detail = result.ConvertTo<Detail>();
                details.Add(detail);
            }
            
            return Ok(details);
        }

        [HttpPut]
        [Route("item")]
        public async Task<IHttpActionResult> Update(Detail updateDetail, string oldCardId)
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
            
            var details = new List<Detail>();
            foreach (var result in results)
            {
                var detail = result.ConvertTo<Detail>();
                details.Add(detail);
            }
            
            return Ok(details);
        }
    }
}
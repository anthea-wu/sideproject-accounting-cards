using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using accounting_cards.Models;
using Google.Cloud.Firestore;

namespace accounting_cards.Controllers
{
    [RoutePrefix("api/detail")]
    public class DetailController : ApiController
    {
        private readonly IDetailRepository _detailRepo;

        public DetailController(IDetailRepository detailRepo)
        {
            _detailRepo = detailRepo;
        }

        [HttpGet]
        [Route("{userId}/{cardId}")]
        public async Task<IHttpActionResult> List(string userId, string cardId)
        {
            var results = await _detailRepo.Get(userId, cardId);
            var details = GetReturnDetails(results);

            return Ok(details);
        }

        [HttpPost]
        [Route("item")]
        public async Task<IHttpActionResult> Add(Detail newDetail)
        {
            await _detailRepo.Create(newDetail);
            
            var results = await _detailRepo.Get(newDetail.UserId, newDetail.CardId);
            var details = GetReturnDetails(results);

            return Ok(details);
        }


        [HttpDelete]
        [Route("item")]
        public async Task<IHttpActionResult> Delete(Detail deleteDetail)
        {
            await _detailRepo.Delete(deleteDetail);
            
            var results = await _detailRepo.Get(deleteDetail.UserId, deleteDetail.CardId);
            var details = GetReturnDetails(results);
            
            return Ok(details);
        }


        [HttpPut]
        [Route("item")]
        public async Task<IHttpActionResult> Update(Detail updateDetail, string oldCardId)
        {
            var deleteDetail = new Detail()
            {
                UserId = updateDetail.UserId,
                CardId = oldCardId,
                Id = updateDetail.Id
            };
            
            await _detailRepo.Delete(deleteDetail);
            await _detailRepo.Create(updateDetail);
            
            var results = await _detailRepo.Get(deleteDetail.UserId, deleteDetail.CardId);
            var details = GetReturnDetails(results);

            return Ok(details);
        }

        private static List<Detail> GetReturnDetails(QuerySnapshot results)
        {
            var details = new List<Detail>();
            foreach (var result in results)
            {
                var detail = result.ConvertTo<Detail>();
                details.Add(detail);
            }

            return details;
        }
    }
}
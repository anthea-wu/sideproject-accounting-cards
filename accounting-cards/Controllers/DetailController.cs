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
        private readonly ICardRepository _cardRepo;
        private readonly IDataService _dataService;

        public DetailController(IDetailRepository detailRepo, ICardRepository cardRepo, IDataService dataService)
        {
            _detailRepo = detailRepo;
            _cardRepo = cardRepo;
            _dataService = dataService;
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
            newDetail.Id = await _detailRepo.Create(newDetail);

            await _dataService.UpdateTotal(newDetail);

            var results = await _detailRepo.Get(newDetail.UserId, newDetail.CardId);
            var details = GetReturnDetails(results);

            return Ok(details);
        }


        [HttpDelete]
        [Route("item")]
        public async Task<IHttpActionResult> Delete(Detail deleteDetail)
        {
            await _detailRepo.Delete(deleteDetail);

            deleteDetail.Count = -deleteDetail.Count;
            await _dataService.UpdateTotal(deleteDetail);
            
            var results = await _detailRepo.Get(deleteDetail.UserId, deleteDetail.CardId);
            var details = GetReturnDetails(results);
            
            return Ok(details);
        }


        [HttpPut]
        [Route("item")]
        public async Task<IHttpActionResult> Update(Detail updateDetail, string oldCardId)
        {
            var deleteDetailDoc = await _detailRepo.Get(updateDetail.UserId, oldCardId, updateDetail.Id);
            var deleteDetail = deleteDetailDoc.ConvertTo<Detail>();
            deleteDetail.Count = -deleteDetail.Count;
            
            await _detailRepo.Delete(deleteDetail);
            await _detailRepo.Create(updateDetail);
            
            await _dataService.UpdateTotal(deleteDetail);
            await _dataService.UpdateTotal(updateDetail);

            var results = await _detailRepo.Get(updateDetail.UserId, deleteDetail.CardId);
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
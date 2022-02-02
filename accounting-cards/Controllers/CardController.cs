using System;
using System.Threading.Tasks;
using System.Web.Http;
using accounting_cards.Models;

namespace accounting_cards.Controllers
{
    [RoutePrefix("api/card")]
    public class CardController : ApiController
    {
        private readonly ICardRepository _cardRepo;
        private readonly IDataService _dataService;

        public CardController(ICardRepository cardRepo, IDataService dataService)
        {
            _cardRepo = cardRepo;
            _dataService = dataService;
        }

        [HttpGet]
        [Route("{id}/list")]
        public async Task<IHttpActionResult> List(string id)
        {
            var results = await _cardRepo.GetList(id);
            var cards = _dataService.GetReturnCardsOrderByCreateTime(results);
            
            return Ok(cards);
        }

        [HttpGet]
        [Route("{userId}/{cardId}")]
        public async Task<IHttpActionResult> Item(string userId, string cardId)
        {
            var result =  await _cardRepo.GetCard(userId, cardId);

            var card = result.ConvertTo<Card>();

            return Ok(card);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Add(Card newCard)
        {
            var cardCollection = _cardRepo.CreateNewCard(newCard);
            newCard.Id = cardCollection.Id;
            newCard.CreateTime = DateTimeOffset.Now;
            await _cardRepo.UpdateCard(newCard, cardCollection);
            
            var results = await _cardRepo.GetList(newCard.UserId);
            var cards = _dataService.GetReturnCardsOrderByCreateTime(results);
            
            return Ok(cards);
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(Card deleteCard)
        {
            await _cardRepo.DeleteCard(deleteCard);
            
            var results = await _cardRepo.GetList(deleteCard.UserId);
            var cards = _dataService.GetReturnCardsOrderByCreateTime(results);

            return Ok(cards);
        }
        
        [HttpPut]
        public async Task<IHttpActionResult> Update(Card updateCard)
        {
            await _cardRepo.UpdateCard(updateCard);

            var results = await _cardRepo.GetList(updateCard.UserId);
            var cards = _dataService.GetReturnCardsOrderByCreateTime(results);
            
            return Ok(cards);
        }
    }
}
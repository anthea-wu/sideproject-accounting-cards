using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using accounting_cards.Models;
using Google.Cloud.Firestore;

namespace accounting_cards
{
    public class DataService : IDataService
    {
        private readonly ICardRepository _cardRepo;
        private readonly IDetailRepository _detailRepo;

        public DataService(ICardRepository cardRepo, IDetailRepository detailRepo)
        {
            _cardRepo = cardRepo;
            _detailRepo = detailRepo;
        }

        public List<Card> GetReturnCardsOrderByCreateTime(QuerySnapshot results)
        {
            var cards = new List<Card>();
            
            foreach (var result in results)
            {
                var card = result.ConvertTo<Card>();
                cards.Add(card);
            }

            cards = cards.OrderBy(c => c.CreateTime).ToList();
            return cards;
        }
        
        public async Task UpdateTotal(Detail detail)
        {
            var cardDoc = await _cardRepo.GetCard(detail.UserId, detail.CardId);
            var card = cardDoc.ConvertTo<Card>();
            card.Total += detail.Count;

            _cardRepo.UpdateCard(card);
        }
    }
}
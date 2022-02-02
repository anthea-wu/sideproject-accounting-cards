using System.Collections.Generic;
using System.Linq;
using accounting_cards.Models;
using Google.Cloud.Firestore;

namespace accounting_cards
{
    public class DataService : IDataService
    {
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
    }
}
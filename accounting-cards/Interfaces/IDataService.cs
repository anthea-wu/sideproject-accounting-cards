using System.Collections.Generic;
using accounting_cards.Models;
using Google.Cloud.Firestore;

namespace accounting_cards
{
    public interface IDataService
    {
        List<Card> GetReturnCardsOrderByCreateTime(QuerySnapshot results);
    }
}
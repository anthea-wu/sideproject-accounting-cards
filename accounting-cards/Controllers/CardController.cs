using System;
using System.Collections.Generic;
using System.Web.Http;

namespace accounting_cards.Controllers
{
    [RoutePrefix("api/card")]
    public class CardController : ApiController
    {
        [HttpGet]
        [Route("list")]
        public IHttpActionResult List()
        {
            var result = new List<CardModel>();
            
            var card = new CardModel()
            {
                Guid = Guid.NewGuid(),
                Name = "未分類",
                Total = 0
            };
            result.Add(card);
            
            card = new CardModel()
            {
                Guid = Guid.NewGuid(),
                Name = "飲食",
                Total = 0
            };
            result.Add(card);
            
            return Ok(result);
        }
    }

    public class CardModel
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public int Total { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace accounting_cards.Controllers
{
    [RoutePrefix("api/card")]
    public class CardController : ApiController
    {
        private static readonly CardModel _cardDefault = new CardModel()
        {
            Guid = Guid.NewGuid(),
            Name = "未分類",
            Total = 0
        };
        
        private static readonly CardModel _cardOne = new CardModel()
        {
            Guid = Guid.NewGuid(),
            Name = "飲食",
            Total = 0
        };
        
        [HttpGet]
        [Route("list")]
        public IHttpActionResult List()
        {
            var result = new List<CardModel>
            {
                _cardDefault, _cardOne
            };
            
            return Ok(result);
        }

        [HttpGet]
        [Route("{guid}")]
        public IHttpActionResult Item(Guid guid)
        {
            if (_cardDefault.Guid == guid)
            {
                return Ok(_cardDefault);
            }
            
            if (_cardOne.Guid == guid)
            {
                return Ok(_cardOne);
            }
            
            return BadRequest("查無此張卡片");
        }
    }

    public class CardModel
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public int Total { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace accounting_cards.Controllers
{
    [RoutePrefix("api/detail")]
    public class DetailController : ApiController
    {
        private static readonly DetailModel _detailDefault = new DetailModel()
        {
            Guid = Guid.NewGuid(),
            Name = "範例",
            Count = 0,
            Date = new DateTimeOffset(2022, 1, 4, 08, 45, 00, TimeSpan.Zero)
        };
        
        private static readonly DetailModel _detailAnotherDefault = new DetailModel()
        {
            Guid = Guid.NewGuid(),
            Name = "範例",
            Count = 100,
            Date = new DateTimeOffset(2022, 1, 4, 08, 53, 00, TimeSpan.Zero)
        };
        
        [HttpGet]
        [Route("list/{guid}")]
        public IHttpActionResult List(Guid guid)
        {
            var result = new List<DetailModel>()
            {
                _detailDefault, _detailAnotherDefault
            };
            
            return Ok(result);
        }
    }

    internal class DetailModel
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
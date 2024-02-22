using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelper;

namespace SearchService.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams searchTerms)
        {
            var query = DB.PagedSearch<Item, Item>();
            query.Sort(x => x.Ascending(auc => auc.Make));

            if(!string.IsNullOrEmpty(searchTerms.SearchTerm))
            {
                query.Match(Search.Full, searchTerms.SearchTerm).SortByTextScore();
            }

            query = searchTerms.OrderBy switch
            {
                "make" => query.Sort(x => x.Ascending(a => a.Make)),
                "new" => query.Sort(x => x.Descending(a => a.CreatedAt)),
                _ => query.Sort(x => x.Ascending(a => a.AuctionEnd))
            };

            query = searchTerms.FilterBy switch
            {
                "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
                "endingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6)
                    && x.AuctionEnd > DateTime.UtcNow),
                _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
            };
            
            query.PageNumber(searchTerms.PageNumber);
            query.PageSize(searchTerms.PageSize);

            if (!string.IsNullOrEmpty(searchTerms.Seller))
            {
                query.Match(x => x.Seller == searchTerms.Seller);
            }

            if (!string.IsNullOrEmpty(searchTerms.Winner))
            {
                query.Match(x => x.Winner == searchTerms.Winner);
            }

            var result = await query.ExecuteAsync();
           
            return Ok(new
            {
                results = result.Results,
                pageCount = result.PageCount,
                totalCount = result.TotalCount
            });
        }
    }
}
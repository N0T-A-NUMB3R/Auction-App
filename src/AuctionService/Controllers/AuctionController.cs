using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/auctions")]
    public class AuctionController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;

        public AuctionController(AuctionDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        /*
        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string? date)
        {
            var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

            if(!string.IsNullOrEmpty(date))
            {
                query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()));
            }

            var auctions = await _context.Auctions
            .Include(i => i.Item)
            .OrderBy(i => i.Item.Make)
            .ToListAsync();

            return _mapper.Map<List<AuctionDto>>(auctions);
           
        }
        */

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string? date)
        {
            var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

            if (!string.IsNullOrEmpty(date))
            {
                query = query.Where(x => x.CreatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
            }

            return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid Id)
        {
            var auction = await _context.Auctions
            .Include(i => i.Item)
            .SingleOrDefaultAsync(a => a.Id == Id);

            if(auction is null)
            {
                return NotFound();    
            }

            return _mapper.Map<AuctionDto>(auction);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
        {
            if (auctionDto is null)
            {
                return BadRequest();
            }

            var auction = _mapper.Map<Auction>(auctionDto);
             //TODO: add current user as seller
            auction.Seller = "test";

            _context.Auctions.Add(auction);

            var result = await  _context.SaveChangesAsync() > 0;
            
            if(!result)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetAuctionById),new{auction.Id},auctionDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            if (updateAuctionDto is null)
            {
                return BadRequest();
            }

            var auction = await _context.Auctions.Include(i => i.Item).SingleOrDefaultAsync(a => a.Id ==id);

            if(auction is null)
            {
                return NotFound();
            }
            //todo check seller == username

            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Mileage;

            var result = await _context.SaveChangesAsync () > 0;

            if(result)
            {
                return Ok();
            }

            return BadRequest("Problem on save changes");
        }

        [HttpDelete("{id}")]
        public async Task <ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            
            if(auction is null)
            {
                return NotFound();
            }
            
              //todo: check seller == usernname
            _context.Auctions.Remove (auction);
            
            var result = await _context.SaveChangesAsync () > 0;

            if(!result)
            {
                return BadRequest("Could not update db");
            }

            return Ok();
        }
    }
}
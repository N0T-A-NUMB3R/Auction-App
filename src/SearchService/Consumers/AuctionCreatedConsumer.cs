using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers
{
    public class AuctionCreatedConsumer : IConsumer<Contracts.AuctionCreated>
    {
        private readonly IMapper _mapper;
        public AuctionCreatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<Contracts.AuctionCreated> context)
        {
            Console.WriteLine("--> Consuming Auction Creted: "+ context.Message.Id);
            var item = _mapper.Map<Item>(context.Message);

            await item.SaveAsync();
        }

    }
}
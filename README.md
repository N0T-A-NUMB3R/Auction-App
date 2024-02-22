Auction App

a microservices based app using .Net, IdentityServer, RabbitMQ running on Docker😎
![image](https://github.com/N0T-A-NUMB3R/Carsties/assets/32098270/b302b60c-c90a-4034-9efc-f75092759b5a)

SERVICES OVERVIEW:

AUCTIONSERVICE

Microservice responsible to store the Auctions, Car models and AuctionResults. This service will contain many relationships between entities, and it needs transactional operations. Therefore, a relational database is needed for this solution. PostgreSQL will be used.

SEARCHSERVICE

This service will store some entities commonly used on search operations. It’s going to have high load and needs to support millions of requests per second. It must have High Performance and High Availability. A NoSQL database best suits those requirements. Therefore, MongoDB will be used.

SERVICEBUS SERVICE (MASSTRANSIT PACKAGE USING RABBITMQ) — ASYNCHRONOUS COMMUNICATION

Service responsible to spread the messages across microservices.

The use of Messaging approach makes microservices loosely coupled since microservices know nothing about the other microservice. Basically, the producer “fire and forget” an event and microservices interested on this event subscribes to this event.

Example: When a request is made to [POST] “/auctions”, an Auction is created on “AuctionService”. Somehow the “SearchService” needs to receive the new CreatedAuction in order to register that on the MongoDB.

The normal solution would be to make an HTTP request to SearchService to replicate the Auction there as well.

But imagine we have 10 microservices interested on the Created auction, would be good to make 10 HTTP requests and wait for them? Certainly not.

Imagine we have a new microservice interested on the CreatedAuction, we would have to change the AuctionService to make the HTTP request to the SearchService. It means the AuctionService depends on SearchService. By using RabbitMq we only have to make the new microservice consume the message, so we add functionality with no changes on AuctionService.



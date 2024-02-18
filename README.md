Auction App

a microservices based app using .Net, IdentityServer, RabbitMQ running on Docker and Kubernetes 😎
![image](https://github.com/N0T-A-NUMB3R/Carsties/assets/32098270/b302b60c-c90a-4034-9efc-f75092759b5a)

SERVICES OVERVIEW:

AUCTIONSERVICE

Microservice responsible to store the Auctions, Car models and AuctionResults. This service will contain many relationships between entities, and it needs transactional operations. Therefore, a relational database is needed for this solution. PostgreSQL will be used.

SEARCHSERVICE

This service will store some entities commonly used on search operations. It’s going to have high load and needs to support millions of requests per second. It must have High Performance and High Availability. A NoSQL database best suits those requirements. Therefore, MongoDB will be used.





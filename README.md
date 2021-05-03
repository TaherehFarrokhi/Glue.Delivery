# Glue.Delivery

The repo contains the GlueDelivery API that controls the CRUD operations and state transition for the delivery.

The project has been developed on .Net 5 so in order to build/run the project .net 5.0 SDK/runtime need to be installed. You can download them from https://dotnet.microsoft.com/download/dotnet/5.0 depend on your operating system and also your hardware configuration.

## How to build/run

In order to run the project:

```
git clone https://github.com/TaherehFarrokhi/Glue.Delivery

cd Glue.Delivery\src

dotnet build
dotnet .\Glue.Delivery.WebApi\bin\Debug\net5.0\Glue.Delivery.WebApi.dll 
```

Now the application available to run:

```
# post new delivery info
curl -X 'POST' \
  'http://localhost:5000/Deliveries' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "accessWindow": {
    "startTime": "2021-05-03T09:39:19.665Z",
    "endTime": "2021-05-03T09:39:19.666Z"
  },
  "recipient": {
    "name": "string",
    "address": "string",
    "email": "one@two.com",
    "phoneNumber": "string"
  },
  "order": {
    "orderNumber": "string",
    "sender": "string"
  }
}'

# get delivery info by id
curl https://localhost:5000/deliveries/<delivery-id>

...

```

### Open API EndPoint

The API contains the Open API endpoint which can be used to test the endpoints in the browser. The Open API is available on http://localhost:5000/swagger

### Running in the Docker

An alternative way of running the application is to run docker-compose file in the command line:

```
cd Glue.Delivery\src

docker-compose up -d

```
**NOTE: to be able to run the application in the docker, the docker desktop must be available on your machine. You can download the docker desktop from https://www.docker.com/products/docker-desktop** 

## Extensions Points

- Improve the role validation: The current implementation uses an http header "role" to send the Role to state transition. That is not a nice implementation and it should come from Authorization Header.
- Add OAuth2: Adding Authentication/Authorization to the api using OAuth2 and changing the role from bing in the header to extract the role from JWT token in Authorization Header. Maybe integration with a social identity platform or Identity Server is a good extension.
- Notification: Pub/Sub notification using SignalR or sending notification using SNS/SQS to any consumer/audience would be great.


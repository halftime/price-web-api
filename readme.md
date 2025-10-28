# price-web-api

Because [googlefinance()](https://support.google.com/docs/answer/3093281?hl=en) keeps breaking my spreadsheets.

## Features
- [localhost only endpoint](./LocalOnlyEndpoint) to add (httpPOST) fund & pricing data
- [remote open endpoint](./RemoteEndPoint) to fetch (httpGET) pricing information (to be used in googlespreadsheet)
- Ready for containerization

## Todo
- [IMPORTXML](https://support.google.com/docs/answer/3093342) support

## Quick start

Run using Docker
```
docker build --pull --rm -f "Dockerfile" -t pricewebapi:latest .
docker run -d -p 8080:8080 --name pricewebapi pricewebapi:latest
```

## API
Base URL: http://localhost:8080

### Remote GET api

| Function  | url endpoint |
| ------------- | ------------- |
| Get fund information | /funds/{ticker}  |
| Get all prices for a fund  | /prices/{ticker}  |
| Get daily fund price  | /pricerecord/{ticker}/{date}  |


### Localhost only POST api

| Function  | url endpoint |
| ------------- | ------------- |
| Add new fund | /addfund  |
| Add historical price record | /addpricerecord |
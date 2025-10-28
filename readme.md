# price-web-api

Because [googlefinance()](https://support.google.com/docs/answer/3093281?hl=en) keeps breaking my spreadsheets.

## Features
- [localhost only endpoint](./LocalOnlyEndpoint) to add (httpPOST) fund & pricing data
- [remote open endpoint](./RemoteEndPoint) to fetch (httpGET) pricing information (to be used in googlespreadsheet)
- XML output supporting [IMPORTXML](https://support.google.com/docs/answer/3093342)
- Ready for containerization

## Quick start

Run using Docker
```
docker build --pull --rm -f "Dockerfile" -t pricewebapi:latest .
docker run -d -p 8080:8080 --name pricewebapi pricewebapi:latest
```

## API @ localhost:8080

### [Example in python](https://github.com/halftime/price-fetcher-py)

### Remote GET api

| Function  | url endpoint |
| ------------- | ------------- |
| Get ETF/ETC details from ticker | /funds/{ticker}  |
| Get all daily data for a ticker  | /prices/{ticker}  |
| Get daily data record  | /pricerecord/{ticker}/{date}  |
| Get daily data record in XML | /pricerecord.xml/{ticker}/{date}  |


### Localhost only POST api

| Function  | url endpoint |
| ------------- | ------------- |
| Add new fund | /addfund  |
| Add historical price record | /addpricerecord |
# price-web-api

Because [googlefinance()](https://support.google.com/docs/answer/3093281?hl=en) keeps breaking my spreadsheets.

## Features
- localhost only endpoint to add (httpPOST) fund & pricing data
- open remote endpoint to fetch (httpGET) pricing information (to be used in googlespreadsheet)
- Ready for containerization

## Quick start

Run using Docker
```
docker build --pull --rm -f "Dockerfile" -t pricewebapi:latest .
docker run -d -p 8080:8080 --name pricewebapi pricewebapi:latest
```

## API
Base URL: http://localhost:8080
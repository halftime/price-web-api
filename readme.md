# price-web-api

Because [googlefinance()](https://support.google.com/docs/answer/3093281?hl=en) keeps breaking my spreadsheets.

## Features
- [localhost only endpoint](./LocalOnlyEndpoint) to add (httpPOST) fund & pricing data
- [remote open endpoint](./RemoteEndPoint) to fetch (httpGET) pricing information (to be used in googlespreadsheet)
- XML output supported [IMPORTXML](https://support.google.com/docs/answer/3093342)
- Ready for containerization

## Changes
    - 01/2026 use NAV instead of close

## API @ localhost
### [Example integration in python](https://github.com/halftime/price-fetcher-py)

### Remote GET api
- http://localhost:8080
- http://ignc.dev:8080
- https port:8081

| Function  | url endpoint |
| ------------- | ------------- |
| Get ETF/ETC details from ticker | /fund/{ticker}  |
| Get all daily data for a ticker  | /prices/{ticker}  |
| Get daily data record  | /pricerecord/{ticker}/{date}  |
| Get daily data record in XML | /pricerecord.xml/{ticker}/{date}  |

### [Googlesheets example](https://docs.google.com/spreadsheets/d/1gQmmdZbtQsx1dtLFz7-i8w_4bGx3qFvNYnwueFmimN4/edit?usp=sharing)
- *TICKERCELL* being a ticker e.g: V3AA
- *DATECELL* being a date, YYYY-MM-DD format e.g: 2025-12-31
- returned floats according to "en_US" (dot as comma) standard
```
=IMPORTXML("http://ignc.dev:8080/pricerecord.xml/" & TICKERCELL & "/" & text(DATECELL;"YYYY-MM-DD");"/PriceRecord/close"; "en_US")
```

### Localhost only POST api

| Function  | url endpoint |
| ------------- | ------------- |
| Add new fund | /addfund  |
| Add historical price record | /addpricerecord |
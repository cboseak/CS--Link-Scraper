# CS--Link-Scraper
A web browser that will scrape all links from websites that you browse to. In auto mode it will spider from all links in the parent url, collecting links from all child urls.

# To Do:
* add logic to filter out non-links. Current logic seems to get most of them but there are a few "mailto:" links that slip through.
* add mode to save output to file.

# Recently added:
* added a spidering mode that spiders from one url to pull urls
* changed from webbrowser to webclient to inprove the performance.
* Added multi-threading and asyncronous processing to ensure a responsive ui

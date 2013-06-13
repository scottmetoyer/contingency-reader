contingency-reader
==================

Contingency Reader is a host-it-yourself Google Reader replacement.

Eventually it will be extended to have close to the functionality of Google Reader. Right now it is limited.

Requirements:

IIS 7+ and .NET 4.5 to host the website, SQL Server 2008+ for the database

A place to to install and schedule the fetcher console app. 

Installation:

Run the Reader.Domain/DatabaseScripts/CreateDatabase.sql script against a SQL Server 2008+ instance to create and initialize the database.

Rename Reader.Web/Default.config to User.config and specify your configuration values (connection string, username, password, services authorization token).
Deploy the web app to your IIS server.

Rename Reader.Fetcher/Default.config to User.config and specify your configuration values (service url - wherever you deployed the web app + /api/, ie. http://mycontingencyreader/api/, services authorization token). Setup a scheduled task to run the fetcher console app at an interval (once every day is good).

Run the fetcher app to fill the database with news items, and start reading!

# Database-Replication
Replicate MySQL database to Microsoft SQL Server and vice-versa

## Setup
1. Setup the App.config with the respective connection strings to run it locally
2. On MySQL, to enable the query statements going to a MySQL table, commands are: SET GLOBAL log_output = "TABLE"; SET GLOBAL general_log = 'ON';

## Improvements
Can be improved in terms of abstract out logging, constants, add unit tests, comments, etc. Also search for string "Improvements" in the code to find few comments from me.

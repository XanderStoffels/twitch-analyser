****Requirements****

 - Docker
 - Docker-Compose

****How to run****

 - Check the ChatBot/appsettings.json and fill in the required fields (Twitch username, access token, etc).
   
 -  Check the docker-compose.yml file and choose
   a password for the MSSQL Server. This password also needs to be
   entered in the Persistor/appsettings.json connection string.

 - Open a command line tool in the root folder and execute 
 `docker-compose build`.

 - After that's done, execute 
 `docker-compose up`.

You can connect to the RabbitMQ Management portal on your local host.
You can connect to the MSSQL Server on your local host.

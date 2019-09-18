
****Requirements****

 - Docker
 - Docker-Compose

****How to run****

 - Check the ChatBot/appsettings.json and fill in the required fields (Twitch username, access token, etc).
   
 -  Check the docker-compose.yml file and choose
   a password for the MSSQL Server. This password also needs to be
   entered in the Persistor/appsettings.json connection string.
   
 - <details>
   <summary>Unix</summary>
   <br>
   Run the run.bash file in the root folder.
   </details>

   <details>
   <summary>Windows</summary>
   <br>
		Open command prompt/powershell in the root folder and execute
		
    `docker-compose build`
    followed by,
    `docker-compose run`.
   </details>
</br>

You can connect to the RabbitMQ Management portal on your local host.</br>
You can connect to the MSSQL Server on your local host.

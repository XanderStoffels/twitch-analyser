<h1>:exclamation: This documentation is outdated</h1>
<p><bold> :exclamation: This documentation is only relevant for release 1.0 and won't help you with the latest build.</bold></p>

<h3>Requirements</h3>

 - Docker
 - Docker-Compose

<h3>How to run</h3>

 - Create the following files: ChatBot/appsettings.json and Persistor/appsettings.json. Copy and fill in the required information from below.
<h6>ChatBot/appsettings.json</h6>

  ```javascript{
  }
    "Username": "[REQUIRED]",
    "AccessToken": "[REQUIRED]",
    "RefreshToken": "[REQUIRED]",
    "ClientId": "[NOT REQUIRED]",
    "StreamPollingInterval" : 1800000,
    "StartupTimeout": 0,
    "RabbitMqChannel" : "rabbitmq",
    "MaxConcurrentChannels" : 30
  }
  ```
  
<h6>Persistor/appsettings.json</h6>

 ```javascript{
 {
  "ConnectionString": "[REQUIRED]",
  "RabbitMqHost" : "rabbitmq"
 }
  ```
   
 -  Check the docker-compose.yml file and choose
   a password for the MSSQL Server. Make sure this password is also used in the Persistor/appsettings.json connection string.

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


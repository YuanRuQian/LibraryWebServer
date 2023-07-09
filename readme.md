# How to set custumized connection string for this project

In a terminal, navigate to the project folder (the project folder, which is one level deeper than the solution folder).

Run

```shell
 cd LibraryWebServer/LibraryWebServer
dotnet user-secrets init
dotnet user-secrets set "MyConn:Library" "Server=cs-db.eng.utah.edu;userid=YOURUID;password=YOURPASSWORD;database=YOURTEAMSDATABASE;" #fill those in the the appropriate values
```
This will store your DB connection string outside of your project's source code.

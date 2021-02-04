# Server

The server was developed using the [Identity Server 4](https://github.com/IdentityServer) + AspNet Identity for authentication.

## First Login

The first time you run the server it will automatically create an admin user:  
user: **adm@localhost**  
password: **Box#123**

# Deploy

Run `dotnet publish`.
Copy the contents of the publish folder to your deploy server.

## To change enviroment Development, Stage and Production

Set the `ASPNETCORE_ENVIRONMENT` variable at the **web.config**.
If none value is setted, the default will be Production.

`<aspNetCore processPath="dotnet" arguments=".\Box.Adm.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout">`
`    <environmentVariables>`
`        <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Stage" />`
`    </environmentVariables>`
`</aspNetCore>`

# To Configure external auths

# To create a new migration
dotnet ef migrations add "name of your migration" --context {NAME-YOUR-CONTEXT} --output-dir Migrations/

# To execute migrations
## Box.Adm
Enter in folder (server\box.adm), there:
dotnet ef database update -c SecurityDbContext
dotnet ef database update -c CMSContext
dotnet ef database update -c SmartGeoIotContext

# To add new classlib
dotnet new classlib -n "name of your class" -f netcoreapp2.1

# To add new migration file
dotnet ef migrations add "name of your migration"

# To remove migration file
dotnet ef migrations remove -f